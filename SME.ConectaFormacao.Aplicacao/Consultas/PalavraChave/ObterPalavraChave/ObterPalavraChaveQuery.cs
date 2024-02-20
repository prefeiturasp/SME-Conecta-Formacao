using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPalavraChaveQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterPalavraChaveQuery _instancia;
        public static ObterPalavraChaveQuery Instancia => _instancia ??= new();
    }
}
