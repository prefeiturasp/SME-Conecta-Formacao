using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarPropostaParaDfCommandHandler : IRequestHandler<EnviarPropostaParaDfCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public EnviarPropostaParaDfCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(EnviarPropostaParaDfCommand request, CancellationToken cancellationToken)
        {
            await _repositorioProposta.EnviarPropostaParaDf(request.PropostaId);
            return true;
        }
    }
}