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


            int quantidadeAssociado = 0;
            IEnumerable<CursistaServicoEol> cursistas = inscricaoAutomaticaTratarTurmas.CursistasEOL;

            var turmasAgrupadas = inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostasTurmas.GroupBy(g => g.Id).Select(s => new { Id = s.Key, Dres = s.Select(x => x.CodigoDre) });

            foreach (var turma in turmasAgrupadas)
            {
                quantidadeAssociado += AssociarCursistasATurma(quantidadeAssociado, inscricaoAutomaticaPropostaTurmaCursistasDTO, cursistas, turma.Id, turma.Dres, inscricaoAutomaticaTratarTurmas.QtdeCursistasSuportadosPorTurma, possuiDres);
            }

            var contadorDaTurma = 2; //Parte 2...Parte 3...Parte 4
            while (quantidadeAssociado < cursistas.Count())
            {
                var ultimaTurmaId = possuiDres ?
                    inscricaoAutomaticaTratarTurmas?.PropostaInscricaoAutomatica?.PropostasTurmas?.LastOrDefault(w => w.CodigoDre == cursistas.FirstOrDefault()?.DreCodigo)?.Id :
                    inscricaoAutomaticaTratarTurmas?.PropostaInscricaoAutomatica?.PropostasTurmas?.LastOrDefault()?.Id;

                // caso não exista turma para a dre dos cursistas, não realiza a inscrição.
                if (possuiDres && !ultimaTurmaId.HasValue)
                    cursistas = cursistas.Where(w => w.DreCodigo == cursistas.FirstOrDefault()?.DreCodigo);

                long propostaTurmaAdicionalId = await InserirTurmaAdicional(inscricaoAutomaticaTratarTurmas, possuiDres, contadorDaTurma, ultimaTurmaId.GetValueOrDefault());

                var dres = inscricaoAutomaticaTratarTurmas?.PropostaInscricaoAutomatica?.PropostasTurmas?.Where(t => t.Id == ultimaTurmaId).Select(s => s.CodigoDre);

                quantidadeAssociado += AssociarCursistasATurma(quantidadeAssociado, inscricaoAutomaticaPropostaTurmaCursistasDTO, cursistas, propostaTurmaAdicionalId, dres, inscricaoAutomaticaTratarTurmas.QtdeCursistasSuportadosPorTurma, possuiDres);

                contadorDaTurma++;
            }

            var inseririnscricao = new InserirInscricaoDTO()
            {
                PropostaId = inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostaId,
                InscricaoAutomaticaPropostaTurmaCursistasDTO = inscricaoAutomaticaPropostaTurmaCursistasDTO
            };

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaTratarCursistas, inseririnscricao));

            return true;
        }

        private async Task<long> InserirTurmaAdicional(InscricaoAutomaticaTratarTurmasDTO inscricaoAutomaticaTratarTurmas, bool possuiDres, int contadorDaTurma, long ultimaTurmaId)
        {
            var ultimaPropostaTurma = await mediator.Send(new ObterPropostaTurmaPorIdQuery(ultimaTurmaId));

            var propostaTurmaAdicional = ultimaPropostaTurma.Clone();
            propostaTurmaAdicional.Nome += $" - Parte {contadorDaTurma}";

            if (possuiDres)
            {
                propostaTurmaAdicional.Dres = inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostasTurmas
                    .Where(w => w.Id == ultimaTurmaId)
                    .Select(s => new PropostaTurmaDre { DreId = s.DreId }).Distinct();
            }
            else
            {
                var dreTodos = await mediator.Send(new ObterDreTodosQuery());
                propostaTurmaAdicional.Dres = new List<PropostaTurmaDre>() { new PropostaTurmaDre { DreId = dreTodos.Id } };
            }

            var propostaTurmaIdInserida = await mediator.Send(new InserirPropostaTurmaEDreCommand(propostaTurmaAdicional));

            return propostaTurmaIdInserida;
        }

        private static int AssociarCursistasATurma(int quantidadeAssociado, List<InscricaoAutomaticaPropostaTurmaCursistasDTO> inscricaoAutomaticaPropostaTurmaCursistasDTO, IEnumerable<CursistaServicoEol> cursistas, long propostaTurmaId, IEnumerable<string> dres, int quantidadeMaximaPorTurma, bool possuiDres)
        {
            IEnumerable<CursistaServicoEol> cursistasTurma;
            if (possuiDres)
                cursistasTurma = cursistas.Where(w => dres.Contains(w.DreCodigo)).Skip(quantidadeAssociado).Take(quantidadeMaximaPorTurma);
            else
                cursistasTurma = cursistas.Skip(quantidadeAssociado).Take(quantidadeMaximaPorTurma);

            if (cursistasTurma.NaoPossuiElementos())
                return quantidadeMaximaPorTurma;

            inscricaoAutomaticaPropostaTurmaCursistasDTO.Add(new InscricaoAutomaticaPropostaTurmaCursistasDTO
            {
                Id = propostaTurmaId,
                Cursistas = cursistasTurma
            });

            return quantidadeMaximaPorTurma;
        }
    }
}
