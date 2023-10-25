using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegentePorId
{
    public class ObterRegentePorIdQueryHandler :IRequestHandler<ObterRegentePorIdQuery, PropostaRegente>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterRegentePorIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PropostaRegente> Handle(ObterRegentePorIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.ObterPropostaRegentePorId(request.RegenteId);
        }
    }
}