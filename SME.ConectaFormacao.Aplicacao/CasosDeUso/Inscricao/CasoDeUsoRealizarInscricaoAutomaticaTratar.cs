using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Entidades;
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

            var inseririnscricao = new InserirInscricaoDTO()
            {
                PropostaId = inscricaoCursistaDto.FormacaoResumida.PropostaId,
                PropostasTurmasCursistas = propostasTurmasCursistas
            };

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaTratarInserir, inseririnscricao, Guid.NewGuid(), null));
            
            return true;
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
            IEnumerable<FuncionarioRfNomeDreCodigoCargoFuncaoDTO> cursistasDaDre, int cursistasAssociados, List<string> codigosDres, long propostaTurmaId, IEnumerable<DreDTO> dres)
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

                    var cursistasSelecionados =_mapper.Map<IEnumerable<FuncionarioRfNomeDreCodigoCargoFuncaoDTO>>(cursistasDaDre.Skip(cursistasAssociados).Take(qtdeCursistasSuportadosPorTurma));

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
            long propostaTurmaId, IEnumerable<FuncionarioRfNomeDreCodigoCargoFuncaoDTO> cursistasDaDre, int cursistasAssociados)
        {
            var cursistasSelecionados = _mapper.Map<IEnumerable<FuncionarioRfNomeDreCodigoCargoFuncaoDTO>>(cursistasDaDre.Skip(cursistasAssociados).Take(qtdeCursistasSuportadosPorTurma));

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
    }
}
