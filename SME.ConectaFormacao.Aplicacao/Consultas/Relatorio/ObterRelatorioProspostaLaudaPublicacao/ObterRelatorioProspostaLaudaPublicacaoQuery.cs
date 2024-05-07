using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRelatorioProspostaLaudaPublicacaoQuery : IRequest<string>
    {
        public ObterRelatorioProspostaLaudaPublicacaoQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }
}
