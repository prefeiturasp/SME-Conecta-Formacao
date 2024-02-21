using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoAlterarProposta : CasoDeUsoAbstrato, ICasoDeUsoAlterarProposta
    {
        public CasoDeUsoAlterarProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<long> Executar(long id, PropostaDTO propostaDTO)
        {

            if (propostaDTO.Situacao == Dominio.Enumerados.SituacaoProposta.Rascunho)
                return await mediator.Send(new AlterarPropostaRascunhoCommand(id, propostaDTO));
            return await mediator.Send(new AlterarPropostaCommand(id, propostaDTO));
        }

    }
}
