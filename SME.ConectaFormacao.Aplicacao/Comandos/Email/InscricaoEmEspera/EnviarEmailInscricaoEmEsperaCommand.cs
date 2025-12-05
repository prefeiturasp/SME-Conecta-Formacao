using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Email.InscricaoEmEspera
{
    public class EnviarEmailInscricaoEmEsperaCommand(long inscricaoId) : IRequest<bool>
    {
        public long InscricaoId { get; set; } = inscricaoId;
    }
}