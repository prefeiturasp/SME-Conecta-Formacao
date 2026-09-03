using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterEmailUsuarioLogadoQuery : IRequest<string>
    {
        private static ObterEmailUsuarioLogadoQuery? _instancia;
        public static ObterEmailUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
