using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNomeUsuarioLogadoQuery : IRequest<string>
    {
        private static ObterNomeUsuarioLogadoQuery _instancia;
        public static ObterNomeUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
