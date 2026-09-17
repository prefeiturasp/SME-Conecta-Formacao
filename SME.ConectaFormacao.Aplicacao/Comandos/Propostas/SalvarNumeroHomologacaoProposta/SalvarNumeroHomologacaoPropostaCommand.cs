using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarNumeroHomologacaoProposta
{
    public class SalvarNumeroHomologacaoPropostaCommand(long propostaId, long? numeroHomologacao) : IRequest<bool>
    {
        public long PropostaId { get; set; } = propostaId;
        public long? NumeroHomologacao { get; set; } = numeroHomologacao;
    }
}
