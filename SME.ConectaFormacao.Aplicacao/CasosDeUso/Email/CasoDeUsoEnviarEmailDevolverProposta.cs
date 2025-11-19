using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Email;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Email
{
    public class CasoDeUsoEnviarEmailDevolverProposta : CasoDeUsoAbstrato, ICasoDeUsoEnviarEmailDevolverProposta
    {
        public CasoDeUsoEnviarEmailDevolverProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var enviarEmail = param.ObterObjetoMensagem<EnviarEmailDevolverPropostaDto>() ?? throw new NegocioException(MensagemNegocio.DADOS_ENVIO_EMAIL_NAO_LOCALIZADO);

            await mediator.Send(new EnviarEmailCommand(enviarEmail.NomeDestinatario, enviarEmail.EmailDestinatario,
                enviarEmail.Titulo, ObterMensagemHtml(enviarEmail.Texto, enviarEmail.Motivo)));

            return true;
        }

        private static string ObterMensagemHtml(string texto, string motivo)
        {
            var caminho = $"{Directory.GetCurrentDirectory()}/wwwroot/ModelosEmail/DevolverProposta.txt";
            var textoArquivo = File.ReadAllText(caminho);
            return textoArquivo
                .Replace("#TEXTO", texto)
                .Replace("#MOTIVO", motivo);
        }
    }
}