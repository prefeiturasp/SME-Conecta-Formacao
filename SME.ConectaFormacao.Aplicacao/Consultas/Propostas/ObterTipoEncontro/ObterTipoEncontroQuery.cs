using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTipoEncontroQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterTipoEncontroQuery _instancia;
        public static ObterTipoEncontroQuery Instancia => _instancia ??= new();
    }
}
