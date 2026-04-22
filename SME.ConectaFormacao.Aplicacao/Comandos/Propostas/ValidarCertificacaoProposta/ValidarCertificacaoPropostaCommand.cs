using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarCertificacaoPropostaCommand : IRequest<IEnumerable<string>>
    {
        public ValidarCertificacaoPropostaCommand(PropostaDTO propostaDto)
        {
            PropostaDto = propostaDto;
        }

        public PropostaDTO PropostaDto { get; set; }
    }
}