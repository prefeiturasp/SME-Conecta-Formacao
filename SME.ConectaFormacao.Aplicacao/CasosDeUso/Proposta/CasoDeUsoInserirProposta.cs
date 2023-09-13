using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoInserirProposta : CasoDeUsoAbstrato, ICasoDeUsoInserirProposta
    {
        public CasoDeUsoInserirProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<long> Executar(PropostaDTO propostaDTO)
        {
            var grupoUsuarioLogadoId = await mediator.Send(ObterGrupoUsuarioLogadoQuery.Instancia());
            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorGrupoIdQuery(grupoUsuarioLogadoId)) ??
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO, System.Net.HttpStatusCode.NotFound);

            if (propostaDTO.Situacao == Dominio.Enumerados.SituacaoProposta.Ativo)
                return await mediator.Send(new InserirPropostaCommand(areaPromotora.Id, propostaDTO));

            return await mediator.Send(new InserirPropostaRascunhoCommand(areaPromotora.Id, propostaDTO));
        }
    }
}
