using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarDetalhamentoDaPropostaCommand :IRequest
    {
        public ValidarDetalhamentoDaPropostaCommand(PropostaDTO propostaDto)
        {
            PropostaDto = propostaDto;
        }

        public PropostaDTO PropostaDto { get; }
    }
}