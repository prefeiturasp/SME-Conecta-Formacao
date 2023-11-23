using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPalavraChaveQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterPalavraChaveQuery _instance;
        public static ObterPalavraChaveQuery Instance => _instance ??= new();
    }
}
