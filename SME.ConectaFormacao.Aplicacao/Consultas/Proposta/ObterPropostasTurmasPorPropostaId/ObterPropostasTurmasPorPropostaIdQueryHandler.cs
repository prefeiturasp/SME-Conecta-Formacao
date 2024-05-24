using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasTurmasPorPropostaIdQueryHandler : IRequestHandler<ObterPropostasTurmasPorPropostaIdQuery, IEnumerable<long>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostasTurmasPorPropostaIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<long>> Handle(ObterPropostasTurmasPorPropostaIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.PropostasTurmaIdsPorPropostaId(request.PropostaId);
        }
    }
}