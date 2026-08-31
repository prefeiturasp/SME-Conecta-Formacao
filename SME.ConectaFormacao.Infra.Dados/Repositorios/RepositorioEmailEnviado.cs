using Dapper;
using Microsoft.Extensions.Logging;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioEmailEnviado(IContextoAplicacao contexto, IConectaFormacaoConexao conexao, ILogger<RepositorioEmailEnviado> logger) : 
        RepositorioBaseAuditavel<EmailEnviado>(contexto, conexao), IRepositorioEmailEnviado
    {
        public async Task<bool> ExistePorChaveIdempotenciaAsync(string chaveIdempotencia)
        {
            try
            {
                var query = """
                select exists(                
                            select 1 
                            from email_enviado 
                            where chave_idempotencia = @chaveIdempotencia 
                              and not excluido
                          )
                """;

                return await conexao.Obter().ExecuteScalarAsync<bool>(query, new { chaveIdempotencia });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao verificar chave de idempotencia");
                // Fail-Open - caso ocorra algum erro na consulta, assume-se que o email não foi enviado
                return false;
            }
        }

        public async Task<EmailEnviado?> ObterPorChaveIdempotenciaAsync(string chaveIdempotencia)
        {
            var query = """
                select id,
                        chave_idempotencia as ChaveIdempotencia,
                        email_destinatario as EmailDestinatario,
                        nome_destinatario as NomeDestinatario,
                        titulo,
                        conteudo_hash as ConteudoHash,
                        enviado_em as EnviadoEm,
                        notificacao_usuario_id as NotificacaoUsuarioId,
                        tentativas_envio as TentativasEnvio,
                        mensagem_erro as MensagemErro,
                        criado_em as CriadoEm,
                        criado_por as CriadoPor,
                        criado_login as CriadoLogin,
                        alterado_em as AlteradoEm,
                        alterado_por as AlteradoPor,
                        alterado_login as AlteradoLogin
                from email_enviado
                where chave_idempotencia = @chaveIdempotencia
                and not excluido
                """;

            return await conexao.Obter().QueryFirstOrDefaultAsync<EmailEnviado>(query, new { chaveIdempotencia });
        }

        public async Task<IEnumerable<EmailEnviado>> ObterPorEmailDestinatarioAsync(string emailDestinatario)
        {
            var query = """
                select id,
                        chave_idempotencia as ChaveIdempotencia,
                        email_destinatario as EmailDestinatario,
                        nome_destinatario as NomeDestinatario,
                        titulo,
                        conteudo_hash as ConteudoHash,
                        enviado_em as EnviadoEm,
                        notificacao_usuario_id as NotificacaoUsuarioId,
                        tentativas_envio as TentativasEnvio,
                        mensagem_erro as MensagemErro,
                        criado_em as CriadoEm,
                        criado_por as CriadoPor,
                        criado_login as CriadoLogin,
                        alterado_em as AlteradoEm,
                        alterado_por as AlteradoPor,
                        alterado_login as AlteradoLogin
                from email_enviado
                where lower(email_destinatario) = lower(@emailDestinatario)
                and not excluido
                order by enviado_em desc
                """;

            return await conexao.Obter().QueryAsync<EmailEnviado>(query, new { emailDestinatario });
        }

        public async Task<IEnumerable<EmailEnviado>> ObterPorNotificacaoUsuarioIdAsync(long notificacaoUsuarioId)
        {
            var query = """
                select id,                
                        chave_idempotencia as ChaveIdempotencia,
                        email_destinatario as EmailDestinatario,
                        nome_destinatario as NomeDestinatario,
                        titulo,
                        conteudo_hash as ConteudoHash,
                        enviado_em as EnviadoEm,
                        notificacao_usuario_id as NotificacaoUsuarioId,
                        tentativas_envio as TentativasEnvio,
                        mensagem_erro as MensagemErro,
                        criado_em as CriadoEm,
                        criado_por as CriadoPor,
                        criado_login as CriadoLogin,
                        alterado_em as AlteradoEm,
                        alterado_por as AlteradoPor,
                        alterado_login as AlteradoLogin
                from email_enviado
                where notificacao_usuario_id = @notificacaoUsuarioId
                and not excluido
                order by enviado_em desc
                """;

            return await conexao.Obter().QueryAsync<EmailEnviado>(query, new { notificacaoUsuarioId });
        }

        public override Task<long> Inserir(EmailEnviado entidade)
        {
            try
            {
                return base.Inserir(entidade);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inserir na tabela de email enviado");
                // Fail-Open - caso ocorra algum erro na inserção, não para a execução do processo, apenas ignora o erro e retorna 0
                return Task.FromResult(0L);
            }
        }
    }
}
