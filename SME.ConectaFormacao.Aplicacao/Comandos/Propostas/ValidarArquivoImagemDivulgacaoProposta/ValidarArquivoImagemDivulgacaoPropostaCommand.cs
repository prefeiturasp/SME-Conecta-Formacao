using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ValidarArquivoImagemDivulgacaoPropostaCommand : IRequest<bool>
    {
        public ValidarArquivoImagemDivulgacaoPropostaCommand(long? arquivoImagemDivulgacaoId)
        {
            ArquivoImagemDivulgacaoId = arquivoImagemDivulgacaoId;
        }

        public long? ArquivoImagemDivulgacaoId { get; set; }
    }
}
