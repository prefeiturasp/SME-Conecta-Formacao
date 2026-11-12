using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Utilitarios
{
    public static class PropostaTransacaoHelper
    {
        public static async Task<long> InserirComTransacaoAsync(
            ITransacao transacaoManager,
            IRepositorioProposta repositorioProposta,
            IMediator mediator,
            IServicoLogs? servicoLogs,
            Proposta proposta,
            bool? integrarNoSgaDto,
            string operacao,
            CancellationToken cancellationToken)
        {
            var transacao = transacaoManager.Iniciar();
            try
            {
                var id = await repositorioProposta.Inserir(proposta);

                await LogIntegrarSgaHelper.RegistrarLogSgaAsync(
                    servicoLogs,
                    new LogIntegrarSgaParametros
                    {
                        Operacao = operacao,
                        PropostaId = id,
                        ValorBancoAnterior = null,
                        ValorRecebidoDto = integrarNoSgaDto,
                        ValorFinalPersistido = proposta.IntegrarNoSGA,
                        NomeFormacao = proposta.NomeFormacao,
                        AreaPromotoraId = proposta.AreaPromotoraId,
                        Situacao = proposta.Situacao.ToString(),
                        Usuario = proposta.CriadoLogin
                    }
                );

                await mediator.Send(new SalvarPropostaCommand(id, proposta, null), cancellationToken);

                transacao.Commit();
                return id;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }

        public static async Task AtualizarComTransacaoAsync(
            ITransacao transacaoManager,
            IRepositorioProposta repositorioProposta,
            IMediator mediator,
            IServicoLogs? servicoLogs,
            Proposta propostaBanco,
            Proposta propostaDepois,
            bool? integrarNoSgaDto,
            string operacao,
            CancellationToken cancellationToken)
        {
            var transacao = transacaoManager.Iniciar();
            try
            {
                await repositorioProposta.Atualizar(propostaDepois);

                await LogIntegrarSgaHelper.RegistrarLogSgaAsync(
                    servicoLogs,
                    new LogIntegrarSgaParametros
                    {
                        Operacao = operacao,
                        PropostaId = propostaBanco.Id,
                        ValorBancoAnterior = propostaBanco.IntegrarNoSGA,
                        ValorRecebidoDto = integrarNoSgaDto,
                        ValorFinalPersistido = propostaDepois.IntegrarNoSGA,
                        NomeFormacao = propostaDepois.NomeFormacao,
                        AreaPromotoraId = propostaDepois.AreaPromotoraId,
                        Situacao = propostaDepois.Situacao.ToString(),
                        Usuario = propostaDepois.AlteradoLogin ?? propostaDepois.CriadoLogin
                    }
                );

                await mediator.Send(new SalvarPropostaCommand(propostaDepois.Id, propostaDepois, propostaBanco.ArquivoImagemDivulgacaoId), cancellationToken);

                transacao.Commit();
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}
