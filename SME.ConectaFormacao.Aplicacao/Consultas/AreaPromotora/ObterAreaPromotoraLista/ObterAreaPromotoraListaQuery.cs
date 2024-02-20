using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraListaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterAreaPromotoraListaQuery _instancia;
        public static ObterAreaPromotoraListaQuery Instancia => _instancia ??= new();
    }
}
