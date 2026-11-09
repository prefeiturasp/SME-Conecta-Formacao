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
                    operacao: operacao,
                    propostaId: id,
                    valorBancoAnterior: null,
                    valorRecebidoDto: integrarNoSgaDto,
                    valorFinalPersistido: proposta.IntegrarNoSGA,
                    nomeFormacao: proposta.NomeFormacao,
                    areaPromotoraId: proposta.AreaPromotoraId,
                    situacao: proposta.Situacao.ToString(),
                    usuario: proposta.CriadoLogin
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
                    operacao: operacao,
                    propostaId: propostaBanco.Id,
                    valorBancoAnterior: propostaBanco.IntegrarNoSGA,
                    valorRecebidoDto: integrarNoSgaDto,
                    valorFinalPersistido: propostaDepois.IntegrarNoSGA,
                    nomeFormacao: propostaDepois.NomeFormacao,
                    areaPromotoraId: propostaDepois.AreaPromotoraId,
                    situacao: propostaDepois.Situacao.ToString(),
                    usuario: propostaDepois.AlteradoLogin ?? propostaDepois.CriadoLogin
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
