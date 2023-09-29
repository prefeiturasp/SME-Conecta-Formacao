using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPalavraChaveQuery : IRequest<IEnumerable<PalavraChaveDTO>>
    {
        private static ObterPalavraChaveQuery _instance;
        public static ObterPalavraChaveQuery Instance => _instance ??= new();
    }
}
