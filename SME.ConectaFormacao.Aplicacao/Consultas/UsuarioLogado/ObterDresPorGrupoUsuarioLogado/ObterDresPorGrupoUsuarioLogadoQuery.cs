using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDresPorGrupoUsuarioLogadoQuery : IRequest<IEnumerable<Dre>>
    {
        private static ObterDresPorGrupoUsuarioLogadoQuery? _instancia;
        public static ObterDresPorGrupoUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
