using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarInformacoesGeraisCommand : IRequest<IEnumerable<string>>
    {
        public ValidarInformacoesGeraisCommand(PropostaDTO propostaDto)
        {
            PropostaDTO = propostaDto;
        }

        public PropostaDTO PropostaDTO { get; }
    }
}