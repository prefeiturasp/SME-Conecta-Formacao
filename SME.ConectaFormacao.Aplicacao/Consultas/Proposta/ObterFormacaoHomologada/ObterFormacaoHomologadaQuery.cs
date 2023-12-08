using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFormacaoHomologadaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterFormacaoHomologadaQuery _instancia;

        public static ObterFormacaoHomologadaQuery Instancia => _instancia ??= new();
    }
}
