using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterNomeUsuarioLogadoQuery : IRequest<string>
    {
        private static ObterNomeUsuarioLogadoQuery _instancia;
        public static ObterNomeUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
