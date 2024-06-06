using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTotalRegistroFilaQuery : IRequest<uint>
    {
        public ObterTotalRegistroFilaQuery(string fila)
        {
            Fila = fila;
        }

        public string Fila { get; }
    }
}
