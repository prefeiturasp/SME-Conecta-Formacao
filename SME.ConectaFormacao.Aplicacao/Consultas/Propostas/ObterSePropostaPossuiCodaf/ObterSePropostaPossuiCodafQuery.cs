using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterSePropostaPossuiCodaf
{
    public class ObterSePropostaPossuiCodafQuery(long propostaId) : IRequest<bool>
    {
        public long PropostaId { get; set; } = propostaId;
    }
}
