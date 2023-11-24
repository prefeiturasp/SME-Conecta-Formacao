using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGrupoUsuarioEDresUsuarioLogadoQuery : IRequest<(Guid,IEnumerable<string>)>
    {
        private static ObterGrupoUsuarioEDresUsuarioLogadoQuery? _instancia;
        public static ObterGrupoUsuarioEDresUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
