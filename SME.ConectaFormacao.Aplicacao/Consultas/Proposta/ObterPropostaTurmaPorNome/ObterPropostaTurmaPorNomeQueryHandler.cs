using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmaPorNomeQueryHandler : IRequestHandler<ObterPropostaTurmaPorNomeQuery, PropostaTurma>
    {
        private readonly ICacheDistribuido _cacheDistribuido;
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaTurmaPorNomeQueryHandler(ICacheDistribuido cacheDistribuido, IRepositorioProposta repositorioProposta)
        {
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PropostaTurma> Handle(ObterPropostaTurmaPorNomeQuery request, CancellationToken cancellationToken)
        {
            var nomeTurma = $"{request.PropostaTurmaNome.RemoverEspacoEmBranco()}_{request.PropostaId}";
            
            var nomeChave = CacheDistribuidoNomes.PropostaTurma.Parametros(nomeTurma);
            
            return await _cacheDistribuido.ObterAsync(nomeChave, () => _repositorioProposta.ObterTurmaPorNome(request.PropostaTurmaNome, request.PropostaId));
        }
    }
}
