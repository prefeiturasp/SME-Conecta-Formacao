using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ParecerPropostaCommandHandler : IRequestHandler<ParecerPropostaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioParecerProposta;

        public ParecerPropostaCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(ParecerPropostaCommand request, CancellationToken cancellationToken)
        {
            await _repositorioProposta.EnviarPropostaParaDf(request.PropostaId);
            return true;
        }
    }
}