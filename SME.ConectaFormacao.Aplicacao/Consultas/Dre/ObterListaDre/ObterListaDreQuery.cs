using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterListaDreQuery : IRequest<IEnumerable<DreDTO>>
    {
        public ObterListaDreQuery(bool exibirTodos)
        {
            ExibirTodos = exibirTodos;
        }

        public bool ExibirTodos { get; }
    }
}