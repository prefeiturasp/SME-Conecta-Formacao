using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegenteTurmaPorRegenteId
{
    public class ObterRegenteTurmaPorRegenteIdQueryHandler : IRequestHandler<ObterRegenteTurmaPorRegenteIdQuery, IEnumerable<PropostaRegenteTurma>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterRegenteTurmaPorRegenteIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<PropostaRegenteTurma>> Handle(ObterRegenteTurmaPorRegenteIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.ObterRegenteTurmasPorRegenteId(request.RegenteId);
        }
    }
}