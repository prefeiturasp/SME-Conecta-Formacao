﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
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

            var possuiDres = inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostasTurmas.Any(t => !string.IsNullOrEmpty(t.CodigoDre));

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
                AssociarCursistasATurma(inscricaoAutomaticaPropostaTurmaCursistasDTO, cursistas, turma.Id, turma.Dres, inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.QuantidadeVagasTurmas, possuiDres);
            }

            List<PropostaInscricaoAutomaticaTurma> turmas = inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostasTurmas.ToList();

            while (cursistas.Any(t => !t.Associado))
            {
                turmas = turmas
                    .OrderBy(o => o.Id)
                    .ThenBy(t => t.CodigoDre)
                    .ToList();

                var ultimaTurmaId = turmas.LastOrDefault()?.Id;

                if (possuiDres)
                {
                    var primeiraCursistaNaoAssociado = cursistas.Where(t => !t.Associado).First();

                    var dreCursista = primeiraCursistaNaoAssociado.FuncaoDreCodigo.EstaPreenchido() ? primeiraCursistaNaoAssociado.FuncaoDreCodigo : primeiraCursistaNaoAssociado.CargoDreCodigo;

                    ultimaTurmaId = turmas.LastOrDefault(w => w.CodigoDre == dreCursista)?.Id;

                    // caso não exista turma para a dre do cursista, não realiza a inscrição.
                    if (!ultimaTurmaId.HasValue)
                    {
                        primeiraCursistaNaoAssociado.Associado = true;
                        continue;
                    }
                }

                long propostaTurmaAdicionalId = await mediator.Send(new InserirPropostaTurmaAdicionalCommand(ultimaTurmaId.GetValueOrDefault(), inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.QuantidadeVagasTurmas));

                var dres = turmas
                    .Where(t => t.Id == ultimaTurmaId)
                    .Select(s => s.CodigoDre).ToList();

                AssociarCursistasATurma(inscricaoAutomaticaPropostaTurmaCursistasDTO, cursistas, propostaTurmaAdicionalId, dres, inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.QuantidadeVagasTurmas, possuiDres);

                foreach (var dre in dres)
                {
                    turmas.Add(new PropostaInscricaoAutomaticaTurma
                    {
                        Id = propostaTurmaAdicionalId,
                        CodigoDre = dre
                    });
                }
            }

            var inseririnscricao = new InserirInscricaoDTO()
            {
                PropostaId = inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostaId,
                InscricaoAutomaticaPropostaTurmaCursistasDTO = inscricaoAutomaticaPropostaTurmaCursistasDTO
            };


            await RemoverCaches(inscricaoAutomaticaTratarTurmas.PropostaInscricaoAutomatica.PropostaId);

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaTratarCursistas, inseririnscricao));

            return true;
        }

        private async Task RemoverCaches(long propostaId)
        {
            await mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.FormacaoResumida.Parametros(propostaId)));
            await mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.FormacaoDetalhada.Parametros(propostaId)));

            await mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.Proposta.Parametros(propostaId)));
            await mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.PropostaPublicoAlvo.Parametros(propostaId)));
            await mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.PropostaFuncaoEspecifica.Parametros(propostaId)));
        }

        private static void AssociarCursistasATurma(List<InscricaoAutomaticaPropostaTurmaCursistasDTO> inscricaoAutomaticaPropostaTurmaCursistasDTO, IEnumerable<CursistaServicoEol> cursistas, long propostaTurmaId, IEnumerable<string> dres, int quantidadeMaximaPorTurma, bool possuiDres)
        {
            IEnumerable<CursistaServicoEol> cursistasTurma;
            if (possuiDres)
                cursistasTurma = cursistas
                    .Where(w => !w.Associado && (w.FuncaoDreCodigo.EstaPreenchido() ? dres.Contains(w.FuncaoDreCodigo) : dres.Contains(w.CargoDreCodigo)))
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
