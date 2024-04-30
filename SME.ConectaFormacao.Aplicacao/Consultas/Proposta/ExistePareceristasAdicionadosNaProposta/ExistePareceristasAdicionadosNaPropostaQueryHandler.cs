using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ExistePareceristasAdicionadosNaPropostaQueryHandler : IRequestHandler<ExistePareceristasAdicionadosNaPropostaQuery, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ExistePareceristasAdicionadosNaPropostaQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(ExistePareceristasAdicionadosNaPropostaQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.ExistePareceristasAdicionadosNaProposta(request.ProspotaId);
        }
    }
}
