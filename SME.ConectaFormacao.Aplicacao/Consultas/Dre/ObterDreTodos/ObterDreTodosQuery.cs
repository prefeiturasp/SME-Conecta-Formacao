using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Dre.ObterDreTodos
{
    public class ObterDreTodosQuery : IRequest<Dominio.Entidades.Dre>
    {
        private static ObterDreTodosQuery _instancia;

        public static ObterDreTodosQuery Instancia => _instancia ??= new();
    }
}
