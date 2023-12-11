using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPorIdQueryHandler : IRequestHandler<ObterPropostaPorIdQuery, Proposta>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaPorIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<Proposta> Handle(ObterPropostaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.ObterPorId(request.Id);
        }
    }
}