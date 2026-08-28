using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Polly;
using Polly.Retry;
using SME.ConectaFormacao.Dominio.Dtos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace SME.ConectaFormacao.Infra.Servicos.Emails
{
    public class ServicoEnvioEmail : IServicoEnvioEmail
    {
        private readonly IServicoAcessos _servicoAcessos;
        private readonly ILogger<ServicoEnvioEmail> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ISmtpClientFactory _smtpClientFactory;
        private static readonly SemaphoreSlim _semaphore = new(2, 2);
        private static readonly ConcurrentDictionary<string, byte> _chavesEnviadas = new();

        public ServicoEnvioEmail(
            IServicoAcessos servicoAcessos,
            ISmtpClientFactory smtpClientFactory,
            ILogger<ServicoEnvioEmail> logger)
        {
            _servicoAcessos = servicoAcessos;
            _logger = logger;
            _smtpClientFactory = smtpClientFactory;

            _retryPolicy = Policy
                .Handle<SmtpCommandException>(ex => ex.StatusCode == SmtpStatusCode.TransactionFailed || ex.ErrorCode == SmtpErrorCode.UnexpectedStatusCode)
                .Or<SocketException>()
                .Or<IOException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception,
                            "Falha temporária no envio de e-mail. Tentativa {Tentativa}/3. Aguardando {Segundos}s. Erro: {MensagemErro}",
                            retryCount,
                            timeSpan.TotalSeconds,
                            exception.Message);
                    }
                );
        }

        public async Task EnviarAsync(MimeMessage mensagem, CancellationToken cancellationToken)
        {
            // Aguarda até que haja uma vaga para enviar o email, garantindo que no máximo 2 envios ocorram simultaneamente
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var configuracaoEmail = await _servicoAcessos.ObterConfiguracaoEmail();
                var envioRealizado = false;

                await _retryPolicy.ExecuteAsync(async (token) =>
                {
                    using var client = _smtpClientFactory.Criar();
                    try
                    {
                        client.Timeout = 10000; // Define um timeout de 10 segundos para as operações SMTP

                        await client.ConnectAsync(configuracaoEmail.Smtp, configuracaoEmail.Porta, configuracaoEmail.TLS,
                            token);
                        await client.AuthenticateAsync(configuracaoEmail.Usuario, configuracaoEmail.Senha, token);

                        await client.SendAsync(mensagem, token);
                        envioRealizado = true; // Marca que o envio foi bem-sucedido

                        await client.DisconnectAsync(true, token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao enviar e-mail para {Destinatario}. Tentativa falhou.", mensagem.To);

                        // Tenta desconectar se ainda estiver conectado
                        if (client.IsConnected)
                        {
                            try
                            {
                                await client.DisconnectAsync(false, token);
                            }
                            catch (Exception exDisconnect)
                            {
                                _logger.LogWarning(exDisconnect, "Erro ao desconectar cliente SMTP após falha no envio");
                            }
                        }

                        // Só relança a exceção se o envio não foi realizado com sucesso
                        if (!envioRealizado)
                            throw;
                    }
                }, cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<ResultadoEnvioEmail> EnviarComIdempotenciaAsync(
            MimeMessage mensagem,
            string chaveIdempotencia,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(chaveIdempotencia))
                throw new ArgumentException("Chave de idempotência é obrigatória", nameof(chaveIdempotencia));

            try
            {
                // Verificação rápida em memória para evitar duplicatas na mesma execução
                if (!_chavesEnviadas.TryAdd(chaveIdempotencia, 0))
                {
                    _logger.LogInformation(
                        "E-mail com chave de idempotência {ChaveIdempotencia} já foi enviado nesta execução. Pulando reenvio.",
                        chaveIdempotencia);

                    return ResultadoEnvioEmail.JaEnviadoAnteriormente(chaveIdempotencia);
                }

                await EnviarAsync(mensagem, cancellationToken);

                _logger.LogInformation(
                    "E-mail enviado com sucesso. Chave: {ChaveIdempotencia}, Destinatário: {Destinatario}",
                    chaveIdempotencia,
                    mensagem.To);

                return ResultadoEnvioEmail.Sucesso(chaveIdempotencia);
            }
            catch (Exception ex)
            {
                var mensagemErro = $"Erro ao enviar e-mail: {ex.Message}";

                _logger.LogError(ex, "Falha no envio de e-mail com idempotência. Chave: {ChaveIdempotencia}", chaveIdempotencia);

                _chavesEnviadas.TryRemove(chaveIdempotencia, out _);

                return ResultadoEnvioEmail.Erro(mensagemErro, chaveIdempotencia);
            }
        }
    }
}