using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGrupoUsuarioLogadoQuery : IRequest<Guid>
    {
        private static ObterGrupoUsuarioLogadoQuery? _instancia;
        public static ObterGrupoUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
