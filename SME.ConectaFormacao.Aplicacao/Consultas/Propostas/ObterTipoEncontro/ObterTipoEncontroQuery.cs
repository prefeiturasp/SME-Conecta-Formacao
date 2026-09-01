using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterTipoEncontroQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterTipoEncontroQuery _instancia;
        public static ObterTipoEncontroQuery Instancia => _instancia ??= new();
    }
}
