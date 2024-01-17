using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoRealizarInscricaoAutomaticaTratar : CasoDeUsoAbstrato, ICasoDeUsoRealizarInscricaoAutomaticaTratar
    {
        private readonly IMapper _mapper;
        
        public CasoDeUsoRealizarInscricaoAutomaticaTratar(IMediator mediator,IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var inscricaoCursistaDto = param.ObterObjetoMensagem<InscricaoCursistaDTO>();

            var propostasTurmasCursistas = new List<PropostaTurmaCursistasDTO>();

            await AssociarCursistasAsTurmas(inscricaoCursistaDto, propostasTurmasCursistas);

            await RealizarInscricaoAutomatica(propostasTurmasCursistas, inscricaoCursistaDto);

            return true;
        }

        private async Task RealizarInscricaoAutomatica(List<PropostaTurmaCursistasDTO> propostasTurmasCursistas, InscricaoCursistaDTO inscricaoCursistaDto)
        {
            foreach (var propostaTurmaCursista in propostasTurmasCursistas)
            {
                foreach (var cursista in propostaTurmaCursista.Cursistas)
                {
                    var usuario = await ObterUsuarioPorLogin(cursista);
                    
                    var cargosFuncoesEol = await ObterCargosFuncoesDresPorRf(cursista.Rf);

                    var cargoBaseSobreposto = ObterCargoESobreposto(cargosFuncoesEol);

                    var funcaoAtividade = ObterFuncaoAtividade(cargosFuncoesEol);
                    
                    var inscricaoAutomaticaDTO = new InscricaoAutomaticaDTO()
                    {
                        UsuarioId = usuario.Id,
                        PropostaId = inscricaoCursistaDto.FormacaoResumida.PropostaId,
                        PropostaTurmaId = propostaTurmaCursista.Id,
                        
                        CargoCodigo = cargoBaseSobreposto?.Codigo,
                        CargoDreCodigo = cargoBaseSobreposto?.DreCodigo,
                        CargoUeCodigo = cargoBaseSobreposto?.UeCodigo,
                        
                        FuncaoCodigo =  funcaoAtividade?.Codigo,
                        FuncaoDreCodigo = funcaoAtividade?.DreCodigo,
                        FuncaoUeCodigo = funcaoAtividade?.UeCodigo
                    };
                    await mediator.Send(new SalvarInscricaoAutomaticaCommand(inscricaoAutomaticaDTO));
                }
            }
        }

        private async Task AssociarCursistasAsTurmas(InscricaoCursistaDTO inscricaoCursistaDto, List<PropostaTurmaCursistasDTO> propostasTurmasCursistas)
        {
            var dres = await mediator.Send(new ObterListaDreQuery(false));

            var agrupamentoPropostaTurmaComDres = inscricaoCursistaDto.FormacaoResumida.
                PropostasTurmas.GroupBy(g => new { g.Id }, (key, group) => 
                    new { key.Id, Result = group.Select(s=> s.CodigoDre).ToList()});
            
            var cursistasAssociados = 0;
            
            foreach (var propostaTurmaComDres in agrupamentoPropostaTurmaComDres)
            {
                var cursistasDaDre = inscricaoCursistaDto.CursistasEOL.Where(w => propostaTurmaComDres.Result.Contains(w.DreCodigo));

                if (cursistasDaDre.NaoPossuiElementos())
                    continue;
                
                cursistasAssociados = AssociarCursistasNasTurmasExistentes(inscricaoCursistaDto.QtdeCursistasSuportadosPorTurma, propostasTurmasCursistas, propostaTurmaComDres.Id, cursistasDaDre, cursistasAssociados);

                await AssociarCursistasNasTurmasNovas(inscricaoCursistaDto.QtdeCursistasSuportadosPorTurma, propostasTurmasCursistas, cursistasDaDre, cursistasAssociados, propostaTurmaComDres.Result, propostaTurmaComDres.Id, dres);
            }
        }

        private async Task AssociarCursistasNasTurmasNovas(int qtdeCursistasSuportadosPorTurma, List<PropostaTurmaCursistasDTO> propostasTurmasCursistas, 
            IEnumerable<FuncionarioRfNomeDreCodigoDTO> cursistasDaDre, int cursistasAssociados, List<string> codigosDres, long propostaTurmaId, IEnumerable<DreDTO> dres)
        {
            if (cursistasDaDre.Count() > cursistasAssociados)
            {
                var qtdeCursistasSemTurma = cursistasDaDre.Count() - cursistasAssociados;
                    
                var qtdeTurmasAdicionais = Math.Ceiling((double)qtdeCursistasSemTurma / qtdeCursistasSuportadosPorTurma);

                var ultimaPropostaTurmaComCursistasAssociados = await mediator.Send(new ObterPropostaTurmaPorIdQuery(propostaTurmaId));
                    
                var contadorDaTurma = 2; //Parte 2...Parte 3...Parte 4

                for (int i = 1; i <= qtdeTurmasAdicionais; i++)
                {
                    var propostaTurmaAdicional = ultimaPropostaTurmaComCursistasAssociados.Clone();
                    propostaTurmaAdicional.Nome += $" - Parte {contadorDaTurma}";
                    propostaTurmaAdicional.Dres = ObterPropostaTurmaDres(propostaTurmaAdicional.Id, dres, codigosDres);
                    var propostaTurmaIdInserida = await mediator.Send(new InserirPropostaTurmaEDreCommand(propostaTurmaAdicional));

                    var cursistasSelecionados =_mapper.Map<IEnumerable<FuncionarioRfNomeDTO>>(cursistasDaDre.Skip(cursistasAssociados).Take(qtdeCursistasSuportadosPorTurma));

                    propostasTurmasCursistas.Add(new PropostaTurmaCursistasDTO()
                    {
                        Id = propostaTurmaIdInserida,
                        Cursistas = cursistasSelecionados
                    });
                    contadorDaTurma++;
                    cursistasAssociados += cursistasSelecionados.Count();
                }
            }
        }

        private int AssociarCursistasNasTurmasExistentes(int qtdeCursistasSuportadosPorTurma, List<PropostaTurmaCursistasDTO> propostasTurmasCursistas, 
            long propostaTurmaId, IEnumerable<FuncionarioRfNomeDreCodigoDTO> cursistasDaDre, int cursistasAssociados)
        {
            var cursistasSelecionados = _mapper.Map<IEnumerable<FuncionarioRfNomeDTO>>(cursistasDaDre.Skip(cursistasAssociados).Take(qtdeCursistasSuportadosPorTurma));

            if (cursistasSelecionados.NaoPossuiElementos())
                return cursistasAssociados; //Tem mais turmas que cursistas
                
            propostasTurmasCursistas.Add(new PropostaTurmaCursistasDTO()
            {
                Id = propostaTurmaId,
                Cursistas = cursistasSelecionados
            });
            cursistasAssociados += cursistasSelecionados.Count();

            return cursistasAssociados;
        }

        private PropostaTurmaDre[] ObterPropostaTurmaDres(long propostaTurmaId, IEnumerable<DreDTO> dres, List<string> codigosDres)
        {
            var propostasTurmasDres = new List<PropostaTurmaDre>();

            foreach (var codigoDre in codigosDres)
                propostasTurmasDres.Add(new PropostaTurmaDre()
                {
                    PropostaTurmaId = propostaTurmaId,
                    DreId = dres.FirstOrDefault(f => f.Codigo.Equals(codigoDre)).Id
                });

            return propostasTurmasDres.ToArray();
        }

        private CargoSobrepostoFuncaoEol ObterCargoESobreposto(IEnumerable<CargoFuncionarioConectaDTO> cargosFuncoesEol)
        {
            if (cargosFuncoesEol.Any(f=> f.CdCargoSobreposto.HasValue))
            {
                var cargoSobreposto = cargosFuncoesEol.FirstOrDefault(f => f.CdCargoSobreposto.HasValue);
                return new CargoSobrepostoFuncaoEol()
                {
                    Codigo = cargoSobreposto?.CdCargoSobreposto.ToString(),
                    DreCodigo = cargoSobreposto?.CdDreCargoSobreposto,
                    UeCodigo = cargoSobreposto?.CdUeCargoSobreposto
                };
            }
                    
           var cargoBase = cargosFuncoesEol.FirstOrDefault(f => f.CdCargoBase.HasValue);
           return new CargoSobrepostoFuncaoEol()
           {
               Codigo = cargoBase?.CdCargoBase.ToString(),
               DreCodigo = cargoBase?.CdDreCargoBase,
               UeCodigo = cargoBase?.CdUeCargoBase
           };
        }
        
        private CargoSobrepostoFuncaoEol ObterFuncaoAtividade(IEnumerable<CargoFuncionarioConectaDTO> cargosFuncoesEol)
        {
            if (cargosFuncoesEol.Any(f=> f.CdFuncaoAtividade.HasValue))
            {
                var funcaoAtividade = cargosFuncoesEol.FirstOrDefault(f => f.CdFuncaoAtividade.HasValue);
                return new CargoSobrepostoFuncaoEol()
                {
                    Codigo = funcaoAtividade?.CdFuncaoAtividade.ToString(),
                    DreCodigo = funcaoAtividade?.CdDreFuncaoAtividade,
                    UeCodigo = funcaoAtividade?.CdUeFuncaoAtividade
                };
            }
            return default;
        }
        
        private async Task<IEnumerable<CargoFuncionarioConectaDTO>> ObterCargosFuncoesDresPorRf(string codigoRf)
        {
            var cargosFuncoesEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(codigoRf));

            if (cargosFuncoesEol.EhNulo())
                throw new NegocioException(MensagemNegocio.CARGO_SOBREPOSTO_FUNCAO_ATIVIDADE_NAO_ENCONTRADO);
            
            return cargosFuncoesEol;
        }

        private async Task<Dominio.Entidades.Usuario> ObterUsuarioPorLogin(FuncionarioRfNomeDTO funcionarioRfNomeDto)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(funcionarioRfNomeDto.Rf));

            if (usuario.NaoEhNulo()) 
                return usuario;

            usuario = _mapper.Map<Dominio.Entidades.Usuario>(funcionarioRfNomeDto);
            
            await mediator.Send(new SalvarUsuarioCommand(usuario));
            return usuario;
        }
    }
}
