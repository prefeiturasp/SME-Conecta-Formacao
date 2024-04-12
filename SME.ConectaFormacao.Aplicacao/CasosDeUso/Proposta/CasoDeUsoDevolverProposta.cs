using MediatR;
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

            await mediator.Send(new AlterarSituacaoDaPropostaCommand(propostaId, SituacaoProposta.Devolvida));

            await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, SituacaoProposta.Devolvida, devolverPropostaDto.Justificativa));

            return await EnviarEmailAreaPromotora(proposta.AreaPromotoraId, proposta.NomeFormacao, devolverPropostaDto.Justificativa);
        }

        private async Task<bool> EnviarEmailAreaPromotora(long areaPromotoraId, string nomeFormacao, string justificativa)
        {
            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorIdQuery(areaPromotoraId));
            if (areaPromotora.EhNulo() || areaPromotora.Excluido)
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA);

            if (string.IsNullOrEmpty(areaPromotora.Email))
                throw new NegocioException(MensagemNegocio.EMAIL_AREA_PROMOTORA_NAO_CADASTRADO_ENVIO_EMAIL);

            var titulo = $"Proposta de {nomeFormacao} foi devolvida.";
            var texto = $"A proposta {nomeFormacao} foi devolvida, realize os ajustes necessários para envia-la novamente.";

            var enviarEmail = new EnviarEmailDevolverPropostaDto
            {
                NomeDestinatario = areaPromotora.Nome,
                EmailDestinatario = areaPromotora.Email,
                Titulo = titulo,
                Texto = texto,
                Motivo = justificativa 
            };

            return await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmailDevolverProposta, enviarEmail));
        }
    }
}