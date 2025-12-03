using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.ReativarInscricao
{
    public class ReativarInscricaoCommand(long id) : IRequest<bool>
    {
        public long Id { get; } = id;
    }
}
