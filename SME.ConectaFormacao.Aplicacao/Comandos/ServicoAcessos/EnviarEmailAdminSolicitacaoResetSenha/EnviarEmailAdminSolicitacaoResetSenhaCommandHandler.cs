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
    public class EnviarEmailAdminSolicitacaoResetSenhaCommandHandler
        : IRequestHandler<EnviarEmailAdminSolicitacaoResetSenhaCommand, bool>
    {
        private readonly IRepositorioUsuario repositorioUsuario;
        private readonly IServicoEnvioEmail servicoEnvioEmail;
        private readonly IServicoAcessos servicoAcessos;
        private readonly IHostEnvironment hostEnvironment;

        public EnviarEmailAdminSolicitacaoResetSenhaCommandHandler(
            IRepositorioUsuario repositorioUsuario,
            IServicoEnvioEmail servicoEnvioEmail,
            IServicoAcessos servicoAcessos,
            IHostEnvironment hostEnvironment)
        {
            this.repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            this.servicoEnvioEmail = servicoEnvioEmail ?? throw new ArgumentNullException(nameof(servicoEnvioEmail));
            this.servicoAcessos = servicoAcessos;
            this.hostEnvironment = hostEnvironment;
        }

        public async Task<bool> Handle(
            EnviarEmailAdminSolicitacaoResetSenhaCommand request,
            CancellationToken cancellationToken)
        {
            var usuario = await repositorioUsuario.ObterPorLogin(request.Login);

            var configuracaoEmail = await servicoAcessos.ObterConfiguracaoEmail();

            if (usuario == null)
                throw new NegocioException(MensagemNegocio.LOGIN_NAO_ENCONTRADO);

            if (hostEnvironment.IsProduction())
            {
                var mensagem = new MimeMessage();
                mensagem.From.Add(new MailboxAddress("Conecta Formação", configuracaoEmail.Email));
                mensagem.To.Add(new MailboxAddress("Administrador", "priscila.o@sme.prefeitura.sp.gov.br"));
                mensagem.Subject = "SOLICITAÇÃO DE RESET DE SENHA";
                mensagem.Body = new TextPart("html") { Text = MontarCorpo(usuario) };

                await servicoEnvioEmail.EnviarAsync(mensagem, cancellationToken);
            }

            return true;
        }

        private string MontarCorpo(Usuario usuario)
        {
            var dataHora = DateTime.Now;

            //<tr><td><strong>Telefone:</strong></td><td>{usuario.Telefone ?? "-"}</td></tr>
            
            return $@"
                <h2>SOLICITAÇÃO DE RESET DE SENHA</h2>
                <p>O usuário (a) a seguir solicitou reset de senha no Conecta:</p>
                <table style='border-collapse: collapse; width: 100%; max-width: 500px;'>
                    <tr><td><strong>Nome:</strong></td><td>{usuario.Nome}</td></tr>
                    <tr><td><strong>E-mail:</strong></td><td>{usuario.Email}</td></tr>
                    <tr><td><strong>E-mail educacional:</strong></td><td>{usuario.EmailEducacional ?? "-"}</td></tr>
                    <tr><td><strong>RF/CPF:</strong></td><td>{usuario.Login}</td></tr>
                    <tr><td><strong>Data/Hora:</strong></td><td>Em {dataHora:dd/MM/yyyy} às {dataHora:HH:mm}</td></tr>
                </table>";
        }
    }
}