using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDresUsuarioLogadoQuery : IRequest<IEnumerable<string>>
    {
        private static ObterDresUsuarioLogadoQuery? _instancia;
        public static ObterDresUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
