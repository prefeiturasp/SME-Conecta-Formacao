using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterSituacaoPropostaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterSituacaoPropostaQuery _instancia;
        public static ObterSituacaoPropostaQuery Instancia => _instancia ??= new();
    }
}
