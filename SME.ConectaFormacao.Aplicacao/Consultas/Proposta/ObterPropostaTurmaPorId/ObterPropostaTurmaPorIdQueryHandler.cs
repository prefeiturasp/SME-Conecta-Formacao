using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmaPorIdQueryHandler : IRequestHandler<ObterPropostaTurmaPorIdQuery, PropostaTurma>
    {
        private readonly ICacheDistribuido _cacheDistribuido;
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaTurmaPorIdQueryHandler(ICacheDistribuido cacheDistribuido, IRepositorioProposta repositorioProposta)
        {
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PropostaTurma> Handle(ObterPropostaTurmaPorIdQuery request, CancellationToken cancellationToken)
        {
            var nomeChave = CacheDistribuidoNomes.PropostaTurma.Parametros(request.PropostaTurmaId);
            return await _cacheDistribuido.ObterAsync(nomeChave, () => _repositorioProposta.ObterTurmaPorId(request.PropostaTurmaId));
        }
    }
}
