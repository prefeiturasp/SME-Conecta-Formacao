using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarArquivoImagemDivulgacaoPropostaCommand : IRequest<bool>
    {
        public ValidarArquivoImagemDivulgacaoPropostaCommand(long? arquivoImagemDivulgacaoId)
        {
            ArquivoImagemDivulgacaoId = arquivoImagemDivulgacaoId;
        }

        public long? ArquivoImagemDivulgacaoId { get; set; }
    }
}
