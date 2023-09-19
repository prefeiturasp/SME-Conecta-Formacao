using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterArquivosPorCodigosQuery : IRequest<IEnumerable<Arquivo>>
    {
        public ObterArquivosPorCodigosQuery(Guid[] codigos)
        {
            Codigos = codigos;
        }

        public Guid[] Codigos { get; }
    }
}
