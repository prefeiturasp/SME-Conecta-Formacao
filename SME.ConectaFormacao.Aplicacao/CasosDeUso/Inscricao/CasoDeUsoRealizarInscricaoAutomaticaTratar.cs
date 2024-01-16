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
                foreach (var cursistaRf in propostaTurmaCursista.Cursistas)
                {
                    var usuario = await ObterUsuarioPorLogin(cursistaRf);
                    
                    var cargosFuncoesEol = await ObterCargosFuncoesDresPorRf(cursistaRf);

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

            var agrupamentoDresComTurmas = inscricaoCursistaDto.FormacaoResumida.
                PropostasTurmas.GroupBy(g => new { g.CodigoDre }, (key, group) => 
                    new { key.CodigoDre, Result = group.Select(s=> s.Id).ToList()});
            
            foreach (var dreComTurmas in agrupamentoDresComTurmas)
            {
                var cursistasDaDre = inscricaoCursistaDto.CursistasEOL.Where(w => w.DreCodigo.Equals(dreComTurmas.CodigoDre));

                if (cursistasDaDre.NaoPossuiElementos())
                    continue;

                var cursistasAssociados = 0;
                
                cursistasAssociados = AssociarCursistasNasTurmasExistentes(inscricaoCursistaDto.QtdeCursistasSuportadosPorTurma, propostasTurmasCursistas, dreComTurmas.Result, cursistasDaDre, cursistasAssociados);

                await AssociarCursistasNasTurmasNovas(inscricaoCursistaDto.QtdeCursistasSuportadosPorTurma, propostasTurmasCursistas, cursistasDaDre, cursistasAssociados, dreComTurmas.CodigoDre, dreComTurmas.Result, dres);
            }
        }

        private async Task AssociarCursistasNasTurmasNovas(int qtdeCursistasSuportadosPorTurma, List<PropostaTurmaCursistasDTO> propostasTurmasCursistas, 
            IEnumerable<FuncionarioRfDreCodigoDTO> cursistasDaDre, int cursistasAssociados, string codigoDre, List<long> dreComTurmas, IEnumerable<DreDTO> dres)
        {
            if (cursistasDaDre.Count() > cursistasAssociados)
            {
                var qtdeCursistasSemTurma = cursistasDaDre.Count() - cursistasAssociados;
                    
                var qtdeTurmasAdicionais = Math.Ceiling((double)qtdeCursistasSemTurma / qtdeCursistasSuportadosPorTurma);

                var ultimaPropostaTurmaComCursistasAssociados = await mediator.Send(new ObterPropostaTurmaPorIdQuery(Enumerable.LastOrDefault<long>(dreComTurmas)));
                    
                var contadorDaTurma = 2; //Parte 2...Parte 3...Parte 4

                for (int i = 1; i <= qtdeTurmasAdicionais; i++)
                {
                    var propostaTurmaAdicional = ultimaPropostaTurmaComCursistasAssociados.Clone();
                    propostaTurmaAdicional.Nome += $" - Parte {contadorDaTurma}";
                    propostaTurmaAdicional.Dres = ObterPropostaTurmaDres(propostaTurmaAdicional.Id, dres, codigoDre);
                    var propostaTurmaIdInserida = await mediator.Send(new InserirPropostaTurmaEDreCommand(propostaTurmaAdicional));

                    var cursistasSelecionados = cursistasDaDre.Skip(cursistasAssociados).Take(qtdeCursistasSuportadosPorTurma).Select(s => s.Rf);

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
            List<long> turmasDaDre, IEnumerable<FuncionarioRfDreCodigoDTO> cursistasDaDre, int cursistasAssociados)
        {
            foreach (var turmaDaDre in turmasDaDre)
            {
                var cursistasSelecionados = cursistasDaDre.Skip(cursistasAssociados).Take(qtdeCursistasSuportadosPorTurma).Select(s => s.Rf);

                if (cursistasSelecionados.NaoPossuiElementos())
                    continue; //Tem mais turmas que cursistas
                    
                propostasTurmasCursistas.Add(new PropostaTurmaCursistasDTO()
                {
                    Id = turmaDaDre,
                    Cursistas = cursistasSelecionados
                });
                cursistasAssociados += cursistasSelecionados.Count();
            }

            return cursistasAssociados;
        }

        private PropostaTurmaDre[] ObterPropostaTurmaDres(long propostaTurmaId, IEnumerable<DreDTO> dres, string codigoDre)
        {
            return new [] 
            {
                new PropostaTurmaDre()
                {
                    PropostaTurmaId = propostaTurmaId,
                    DreId = dres.FirstOrDefault(f => f.Codigo.Equals(codigoDre)).Id
                }
            };
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

        private async Task<Dominio.Entidades.Usuario> ObterUsuarioPorLogin(string codigoRf)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(codigoRf));

            if (usuario.NaoEhNulo()) 
                return usuario;
            
            usuario = new Dominio.Entidades.Usuario() { Login = codigoRf, Nome = "Cadastrado automaticamente - Inscrições automáticas" };
            await mediator.Send(new SalvarUsuarioCommand(usuario));
            return usuario;
        }
    }
}
