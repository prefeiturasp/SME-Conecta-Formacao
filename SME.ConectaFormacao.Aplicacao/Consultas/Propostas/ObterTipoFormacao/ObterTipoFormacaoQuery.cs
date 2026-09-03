using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterTipoFormacaoQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterTipoFormacaoQuery _instancia;
        public static ObterTipoFormacaoQuery Instancia => _instancia ??= new();
    }
}
