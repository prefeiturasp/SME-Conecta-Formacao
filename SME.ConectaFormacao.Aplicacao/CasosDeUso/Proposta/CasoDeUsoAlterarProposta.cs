using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoAlterarProposta : CasoDeUsoAbstrato, ICasoDeUsoAlterarProposta
    {
        public CasoDeUsoAlterarProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(long id, PropostaDTO propostaDTO)
        {
            if (propostaDTO.Situacao.EhParaSalvarRascunho())
                return await mediator.Send(new AlterarPropostaRascunhoCommand(id, propostaDTO));

            AlteraSituacaoParaPublicada(propostaDTO);

            await SalvarMovimentacao(id, propostaDTO.Situacao);

            return await mediator.Send(new AlterarPropostaCommand(id, propostaDTO));
        }

        private void AlteraSituacaoParaPublicada(PropostaDTO propostaDTO)
        {
            if (propostaDTO.Situacao.EhAprovada() &&
                propostaDTO.FormacaoHomologada.EstaHomologada() &&
                propostaDTO.NumeroHomologacao.GetValueOrDefault() != 0)
                propostaDTO.Situacao = SituacaoProposta.Publicada;
        }

        private async Task SalvarMovimentacao(long propostaId, SituacaoProposta situacao)
        {
            await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, situacao));
        }
    }
}
