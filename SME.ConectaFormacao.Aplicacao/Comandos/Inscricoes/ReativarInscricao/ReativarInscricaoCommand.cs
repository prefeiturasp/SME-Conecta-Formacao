using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricao.ReativarInscricao
{
    public class ReativarInscricaoCommand : IRequest<bool>
    {
        public long Id { get; }
        public ReativarInscricaoCommand(long id)
        {
            Id = id;
        }
    }
}
