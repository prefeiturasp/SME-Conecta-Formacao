using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterModalidadeQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterModalidadeQuery _instancia;
        public static ObterModalidadeQuery Instancia => _instancia ??= new();
    }
}
