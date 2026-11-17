using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Utilitarios
{
    public sealed class InserirPropostaTransacaoParametros
    {
        public ITransacao TransacaoManager { get; init; } = null!;
        public IRepositorioProposta RepositorioProposta { get; init; } = null!;
        public IMediator Mediator { get; init; } = null!;
        public IServicoLogs? ServicoLogs { get; init; }
        public Proposta Proposta { get; init; } = null!;
        public bool? IntegrarNoSgaDto { get; init; }
        public string Operacao { get; init; } = string.Empty;
        public CancellationToken CancellationToken { get; init; }
    }

    public sealed class AtualizarPropostaTransacaoParametros
    {
        public ITransacao TransacaoManager { get; init; } = null!;
        public IRepositorioProposta RepositorioProposta { get; init; } = null!;
        public IMediator Mediator { get; init; } = null!;
        public IServicoLogs? ServicoLogs { get; init; }
        public Proposta PropostaBanco { get; init; } = null!;
        public Proposta PropostaDepois { get; init; } = null!;
        public bool? IntegrarNoSgaDto { get; init; }
        public string Operacao { get; init; } = string.Empty;
        public CancellationToken CancellationToken { get; init; }
    }

    public static class PropostaTransacaoHelper
    {
        public static async Task<long> InserirComTransacaoAsync(
            InserirPropostaTransacaoParametros parametros)
        {
            var transacao = parametros.TransacaoManager.Iniciar();
            try
            {
                var id = await parametros.RepositorioProposta.Inserir(parametros.Proposta);

                await LogIntegrarSgaHelper.RegistrarLogSgaAsync(
                    parametros.ServicoLogs,
                    new LogIntegrarSgaParametros
                    {
                        Operacao = parametros.Operacao,
                        PropostaId = id,
                        ValorBancoAnterior = null,
                        ValorRecebidoDto = parametros.IntegrarNoSgaDto,
                        ValorFinalPersistido = parametros.Proposta.IntegrarNoSGA,
                        NomeFormacao = parametros.Proposta.NomeFormacao,
                        AreaPromotoraId = parametros.Proposta.AreaPromotoraId,
                        Situacao = parametros.Proposta.Situacao.ToString(),
                        Usuario = parametros.Proposta.CriadoLogin
                    }
                );

                await parametros.Mediator.Send(new SalvarPropostaCommand(id, parametros.Proposta, null), parametros.CancellationToken);

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
            AtualizarPropostaTransacaoParametros parametros)
        {
            var transacao = parametros.TransacaoManager.Iniciar();
            try
            {
                await parametros.RepositorioProposta.Atualizar(parametros.PropostaDepois);

                await LogIntegrarSgaHelper.RegistrarLogSgaAsync(
                    parametros.ServicoLogs,
                    new LogIntegrarSgaParametros
                    {
                        Operacao = parametros.Operacao,
                        PropostaId = parametros.PropostaBanco.Id,
                        ValorBancoAnterior = parametros.PropostaBanco.IntegrarNoSGA,
                        ValorRecebidoDto = parametros.IntegrarNoSgaDto,
                        ValorFinalPersistido = parametros.PropostaDepois.IntegrarNoSGA,
                        NomeFormacao = parametros.PropostaDepois.NomeFormacao,
                        AreaPromotoraId = parametros.PropostaDepois.AreaPromotoraId,
                        Situacao = parametros.PropostaDepois.Situacao.ToString(),
                        Usuario = parametros.PropostaDepois.AlteradoLogin ?? parametros.PropostaDepois.CriadoLogin
                    }
                );

                await parametros.Mediator.Send(new SalvarPropostaCommand(parametros.PropostaDepois.Id, parametros.PropostaDepois, parametros.PropostaBanco.ArquivoImagemDivulgacaoId), parametros.CancellationToken);

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
