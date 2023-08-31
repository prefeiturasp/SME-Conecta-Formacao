using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

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
            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorGrupoIdQuery(grupoUsuarioLogadoId));

            if (propostaDTO.Situacao == Dominio.Enumerados.SituacaoRegistro.Ativo)
                return await mediator.Send(new InserirPropostaCommand(areaPromotora.Id, propostaDTO));

            return await mediator.Send(new InserirPropostaRascunhoCommand(areaPromotora.Id, propostaDTO));
        }
    }
}
