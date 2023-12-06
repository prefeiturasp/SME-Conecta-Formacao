using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTodosOsAnosQuery : IRequest<IEnumerable<Ano>>
    {
        private static ObterTodosOsAnosQuery _instance;
        public static ObterTodosOsAnosQuery Instance => _instance ??= new ObterTodosOsAnosQuery();
    }
}
