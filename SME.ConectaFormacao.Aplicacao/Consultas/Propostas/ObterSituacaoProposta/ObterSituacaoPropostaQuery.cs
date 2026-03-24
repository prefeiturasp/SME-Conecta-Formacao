using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterSituacaoPropostaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterSituacaoPropostaQuery _instancia;
        public static ObterSituacaoPropostaQuery Instancia => _instancia ??= new();
    }
}
