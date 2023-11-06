using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverPropostaRegenteCommandHandler : IRequestHandler<RemoverPropostaRegenteCommand,bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public RemoverPropostaRegenteCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(RemoverPropostaRegenteCommand request, CancellationToken cancellationToken)
        {
            await _repositorioProposta.ExcluirPropostaRegente(request.RegenteId);
            var turmas = await _repositorioProposta.ObterRegenteTurmasPorRegenteId(request.RegenteId);
            if (turmas.Count() > 0)
                await _repositorioProposta.ExcluirPropostaRegenteTurmas(turmas);
            return true;
        }
    }
}