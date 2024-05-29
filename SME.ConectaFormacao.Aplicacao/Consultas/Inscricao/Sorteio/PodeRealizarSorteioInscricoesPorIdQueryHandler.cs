using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class PodeRealizarSorteioInscricoesPorIdQueryHandler : IRequestHandler<PodeRealizarSorteioInscricoesPorIdQuery, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public PodeRealizarSorteioInscricoesPorIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(PodeRealizarSorteioInscricoesPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.PodeRealizarSorteioDasInscricoesPorPropostaId(request.PropostaId);
        }
    }
}