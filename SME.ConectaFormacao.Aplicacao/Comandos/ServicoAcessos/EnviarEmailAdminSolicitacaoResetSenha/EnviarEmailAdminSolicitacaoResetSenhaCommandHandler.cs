using MediatR;
using Microsoft.Extensions.Hosting;
using MimeKit;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Emails.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.EnviarEmailAdminSolicitacaoResetSenha
{
    public class EnviarEmailAdminSolicitacaoResetSenhaCommandHandler(
        IRepositorioUsuario repositorioUsuario,
        IServicoEnvioEmail servicoEnvioEmail,
        IServicoAcessos servicoAcessos,
        IHostEnvironment hostEnvironment)
                : IRequestHandler<EnviarEmailAdminSolicitacaoResetSenhaCommand, bool>
    {
        public async Task<bool> Handle(
            EnviarEmailAdminSolicitacaoResetSenhaCommand request,
            CancellationToken cancellationToken)
        {
            var usuario = await repositorioUsuario.ObterPorLogin(request.Login);

            var configuracaoEmail = await servicoAcessos.ObterConfiguracaoEmail();

            if (usuario == null)
                throw new NegocioException(MensagemNegocio.LOGIN_NAO_ENCONTRADO);

            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Conecta Formação", configuracaoEmail.Email));
            mensagem.To.Add(new MailboxAddress("Administrador", "conectaformacao@sme.prefeitura.sp.gov.br"));
            mensagem.Subject = "SOLICITAÇÃO DE RESET DE SENHA";
            mensagem.Body = new TextPart("html") { Text = MontarCorpo(usuario) };

            if (!hostEnvironment.IsDevelopment())
                await servicoEnvioEmail.EnviarAsync(mensagem, cancellationToken);


            return true;
        }

        private static string MontarCorpo(Usuario usuario)
        {
            var dataHora = DateTime.Now;

            return $@"
                <h2>SOLICITAÇÃO DE RESET DE SENHA</h2>
                <p>O usuário (a) a seguir solicitou reset de senha no Conecta:</p>
                <table style='border-collapse: collapse; width: 100%; max-width: 500px;'>
                    <tr><td><strong>Nome:</strong></td><td>{usuario.Nome}</td></tr>
                    <tr><td><strong>E-mail:</strong></td><td>{usuario.Email}</td></tr>
                    <tr><td><strong>E-mail educacional:</strong></td><td>{usuario.EmailEducacional ?? "-"}</td></tr>
                    <tr><td><strong>Telefone:</strong></td><td>{FormatarTelefone(usuario.Telefone)}</td></tr>
                    <tr><td><strong>RF/CPF:</strong></td><td>{usuario.Login}</td></tr>
                    <tr><td><strong>Data/Hora:</strong></td><td>Em {dataHora:dd/MM/yyyy} às {dataHora:HH:mm}</td></tr>
                </table>";
        }
        private static string FormatarTelefone(string? telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return "-";

            var numeros = new string(telefone.Where(char.IsDigit).ToArray());

            return numeros.Length switch
            {
                11 => $"({numeros[..2]}) {numeros[2..7]}-{numeros[7..]}",  // (99) 99999-9999
                10 => $"({numeros[..2]}) {numeros[2..6]}-{numeros[6..]}",  // (99) 9999-9999
                _ => telefone
            };
        }
    }
}