using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterEmailUsuarioLogadoQuery : IRequest<string>
    {
        private static ObterEmailUsuarioLogadoQuery? _instancia;
        public static ObterEmailUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
