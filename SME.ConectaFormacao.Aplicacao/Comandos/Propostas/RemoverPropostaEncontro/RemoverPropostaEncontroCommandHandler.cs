using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverPropostaEncontroCommandHandler(IRepositorioPropostaEncontro repositorioPropostaEncontro) : IRequestHandler<RemoverPropostaEncontroCommand, bool>
    {
        public async Task<bool> Handle(RemoverPropostaEncontroCommand request, CancellationToken cancellationToken)
        {
            var encontro = await repositorioPropostaEncontro.ObterEncontroPorIdAsync(request.Id);
            if (encontro == null)
                return true;
            await repositorioPropostaEncontro.RemoverEncontrosAsync([encontro]);
            return true;
        }
    }
}
