using MediatR;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaInscricaoAutomaticaPorIdQueryHandler : IRequestHandler<ObterPropostaInscricaoAutomaticaPorIdQuery, PropostaInscricaoAutomatica>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaInscricaoAutomaticaPorIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public Task<PropostaInscricaoAutomatica> Handle(ObterPropostaInscricaoAutomaticaPorIdQuery request, CancellationToken cancellationToken)
        {
            return _repositorioProposta.ObterPropostaInscricaoPorId(request.PropostaId);
        }
    }
}