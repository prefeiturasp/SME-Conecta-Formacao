using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTodosOsComponentesCurricularesQuery : IRequest<IEnumerable<ComponenteCurricular>>
    {
        private static ObterTodosOsComponentesCurricularesQuery _instance;
        public static ObterTodosOsComponentesCurricularesQuery Instance => _instance ??= new ObterTodosOsComponentesCurricularesQuery();
    }
}
