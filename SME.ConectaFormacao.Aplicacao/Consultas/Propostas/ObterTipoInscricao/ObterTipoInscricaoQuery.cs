using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterTipoInscricaoQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterTipoInscricaoQuery _instancia;
        public static ObterTipoInscricaoQuery Instancia => _instancia ??= new();
    }
}
