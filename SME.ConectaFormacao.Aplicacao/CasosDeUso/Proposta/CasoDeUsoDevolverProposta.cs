using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoDevolverProposta : CasoDeUsoAbstrato, ICasoDeUsoDevolverProposta
    {
        public CasoDeUsoDevolverProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId, DevolverPropostaDTO devolverPropostaDto)
        {
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));
            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            if (string.IsNullOrEmpty(devolverPropostaDto.Justificativa))
                throw new NegocioException(MensagemNegocio.JUSTIFICATIVA_NAO_INFORMADA);

            var codigoFormacao = proposta.Id.ToString();
            var nomeFormacao = proposta.NomeFormacao;
            var justificativa = devolverPropostaDto.Justificativa;
            const SituacaoProposta situacaoDevolvida = SituacaoProposta.Devolvida;

            var retorno = await mediator.Send(new AlterarSituacaoDaPropostaCommand(propostaId, situacaoDevolvida));
            retorno = retorno && await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, situacaoDevolvida, justificativa));
            retorno = retorno && await EnviarEmailUsuario(codigoFormacao, nomeFormacao, justificativa);
            retorno = retorno && await EnviarEmailAreaPromotora(proposta.AreaPromotoraId, codigoFormacao, nomeFormacao, justificativa);

            return retorno;
        }

        private async Task<bool> EnviarEmailUsuario(string codigoFormacao, string nomeFormacao, string justificativa)
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());
            if (usuarioLogado.EhNulo() || usuarioLogado.Excluido)
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            if (string.IsNullOrEmpty(usuarioLogado.Email))
                throw new NegocioException(MensagemNegocio.EMAIL_USUARIO_NAO_CADASTRADO_ENVIO_EMAIL);

            var enviarEmail = MontarEmail(usuarioLogado.Nome, usuarioLogado.Email, codigoFormacao, nomeFormacao, justificativa);
            return await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmailDevolverProposta, enviarEmail));
        }

        private async Task<bool> EnviarEmailAreaPromotora(long areaPromotoraId, string codigoFormacao, string nomeFormacao, string justificativa)
        {
            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorIdQuery(areaPromotoraId));
            if (areaPromotora.EhNulo() || areaPromotora.Excluido)
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA);

            if (string.IsNullOrEmpty(areaPromotora.Email))
                throw new NegocioException(MensagemNegocio.EMAIL_AREA_PROMOTORA_NAO_CADASTRADO_ENVIO_EMAIL);

            var enviarEmail = MontarEmail(areaPromotora.Nome, areaPromotora.Email, codigoFormacao, nomeFormacao, justificativa);
            return await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmailDevolverProposta, enviarEmail));
        }

        private static EnviarEmailDevolverPropostaDto MontarEmail(string destinatario, string emailDestinatario,
            string codigoFormacao, string nomeFormacao, string justificativa)
        {
            var titulo = $"Proposta de {codigoFormacao} - {nomeFormacao} foi devolvida.";
            var texto = $"A proposta <strong>{codigoFormacao} - {nomeFormacao}</strong> foi devolvida, realize os ajustes necessários para envia-la novamente.";

            return new EnviarEmailDevolverPropostaDto
            {
                NomeDestinatario = destinatario,
                EmailDestinatario = emailDestinatario,
                Titulo = titulo,
                Texto = texto,
                Motivo = justificativa
            };
        }
    }
}