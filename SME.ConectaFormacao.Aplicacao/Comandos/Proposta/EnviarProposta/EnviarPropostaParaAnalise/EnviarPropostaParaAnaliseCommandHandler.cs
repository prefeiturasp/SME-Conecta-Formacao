using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarPropostaParaAnaliseCommandHandler : IRequestHandler<EnviarPropostaParaAnaliseCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public EnviarPropostaParaAnaliseCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(EnviarPropostaParaAnaliseCommand request, CancellationToken cancellationToken)
        {
            await _repositorioProposta.AtualizarSituacao(request.PropostaId, request.Situacao);
            return true;
        }
    }
}