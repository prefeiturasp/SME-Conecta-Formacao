using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarPropostaCommandHandler : IRequestHandler<EnviarPropostaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public EnviarPropostaCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(EnviarPropostaCommand request, CancellationToken cancellationToken)
        {
            await _repositorioProposta.AtualizarSituacao(request.PropostaId, request.Situacao);
            return true;
        }
    }
}