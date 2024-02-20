using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ExisteCargoFuncaoOutrosNaPropostaQueryHandler : IRequestHandler<ExisteCargoFuncaoOutrosNaPropostaQuery, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ExisteCargoFuncaoOutrosNaPropostaQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(ExisteCargoFuncaoOutrosNaPropostaQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.ExisteCargoFuncaoOutrosNaProposta(request.PropostaId);
        }
    }
}