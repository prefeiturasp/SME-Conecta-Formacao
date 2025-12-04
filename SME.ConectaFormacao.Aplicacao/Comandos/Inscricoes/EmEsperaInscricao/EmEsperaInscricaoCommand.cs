using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.EmEsperaInscricao
{
    public class EmEsperaInscricaoCommand(long id) : IRequest<bool>
    {
        public long Id { get; } = id;
    }
}
