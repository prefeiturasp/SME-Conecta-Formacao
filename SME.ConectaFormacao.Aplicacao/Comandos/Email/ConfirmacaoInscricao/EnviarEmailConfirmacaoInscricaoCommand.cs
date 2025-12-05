using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailConfirmacaoInscricaoCommand(long inscricaoId) : IRequest<bool>
    {
        public long InscricaoId { get; set; } = inscricaoId;
    }
}