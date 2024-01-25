using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Dre.ObterDreTodos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoRealizarInscricaoAutomaticaTratarTurmas : CasoDeUsoAbstrato, Interfaces.Inscricao.ICasoDeUsoRealizarInscricaoAutomaticaTratarTurmas
    {
        public CasoDeUsoRealizarInscricaoAutomaticaTratarTurmas(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var inscricaoAutomaticaTratarTurmas = param.ObterObjetoMensagem<InscricaoAutomaticaTratarTurmasDTO>();

            var inscricaoAutomaticaPropostaTurmaCursistasDTO = new List<InscricaoAutomaticaPropostaTurmaCursistasDTO>();

            var possuiDres = inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostasTurmas.Any(t => t.DreId.HasValue);

            IEnumerable<CursistaServicoEol> cursistas = inscricaoAutomaticaTratarTurmas.CursistasEOL;
            var turmasAgrupadas = inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostasTurmas
                .GroupBy(g => g.Id)
                .Select(s => new
                {
                    Id = s.Key,
                    Dres = s.Select(x => x.CodigoDre)
                });

            foreach (var turma in turmasAgrupadas)
            {
                AssociarCursistasATurma(inscricaoAutomaticaPropostaTurmaCursistasDTO, cursistas, turma.Id, turma.Dres, inscricaoAutomaticaTratarTurmas.QtdeCursistasSuportadosPorTurma, possuiDres);
            }

            while (cursistas.Any(t => !t.Associado))
            {
                var ultimaTurmaId = inscricaoAutomaticaTratarTurmas?.PropostaInscricaoAutomatica?.PropostasTurmas?.LastOrDefault()?.Id;

                if (possuiDres)
                {
                    var primeiraCursistaNaoAssociado = cursistas.Where(t => !t.Associado).First();

                    ultimaTurmaId = inscricaoAutomaticaTratarTurmas?.PropostaInscricaoAutomatica?.PropostasTurmas?
                        .LastOrDefault(w => w.CodigoDre == primeiraCursistaNaoAssociado.DreCodigo)?.Id;

                    // caso não exista turma para a dre do cursista, não realiza a inscrição.
                    if (!ultimaTurmaId.HasValue)
                    {
                        primeiraCursistaNaoAssociado.Associado = true;
                        continue;
                    }
                }

                long propostaTurmaAdicionalId = await mediator.Send(new InserirPropostaTurmaAdicionalCommand(ultimaTurmaId.GetValueOrDefault()));

                var dres = inscricaoAutomaticaTratarTurmas?.PropostaInscricaoAutomatica?.PropostasTurmas?.Where(t => t.Id == ultimaTurmaId).Select(s => s.CodigoDre);

                AssociarCursistasATurma(inscricaoAutomaticaPropostaTurmaCursistasDTO, cursistas, propostaTurmaAdicionalId, dres, inscricaoAutomaticaTratarTurmas.QtdeCursistasSuportadosPorTurma, possuiDres);
            }

            var inseririnscricao = new InserirInscricaoDTO()
            {
                PropostaId = inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostaId,
                InscricaoAutomaticaPropostaTurmaCursistasDTO = inscricaoAutomaticaPropostaTurmaCursistasDTO
            };

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaTratarCursistas, inseririnscricao));

            return true;
        }

        private static void AssociarCursistasATurma(List<InscricaoAutomaticaPropostaTurmaCursistasDTO> inscricaoAutomaticaPropostaTurmaCursistasDTO, IEnumerable<CursistaServicoEol> cursistas, long propostaTurmaId, IEnumerable<string> dres, int quantidadeMaximaPorTurma, bool possuiDres)
        {
            IEnumerable<CursistaServicoEol> cursistasTurma;
            if (possuiDres)
                cursistasTurma = cursistas
                    .Where(w => !w.Associado && dres.Contains(w.DreCodigo))
                    .Take(quantidadeMaximaPorTurma);
            else
                cursistasTurma = cursistas
                    .Where(w => !w.Associado)
                    .Take(quantidadeMaximaPorTurma);

            if (cursistasTurma.NaoPossuiElementos())
                return;

            inscricaoAutomaticaPropostaTurmaCursistasDTO.Add(new InscricaoAutomaticaPropostaTurmaCursistasDTO
            {
                Id = propostaTurmaId,
                Cursistas = cursistasTurma.ToList()
            });

            foreach (var cur in cursistasTurma)
                cur.Associado = true;
        }
    }
}
