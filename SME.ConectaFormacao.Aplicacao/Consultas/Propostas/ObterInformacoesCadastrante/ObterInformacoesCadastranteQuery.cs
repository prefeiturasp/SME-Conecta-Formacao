using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInformacoesCadastranteQuery : IRequest<PropostaInformacoesCadastranteDTO>
    {
        public ObterInformacoesCadastranteQuery(long? propostaId)
        {
            PropostaId = propostaId;
        }

        public long? PropostaId { get; }
    }
}
