using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmaDresPorPropostaTurmaIdQueryHandler : IRequestHandler<ObterPropostaTurmaDresPorPropostaTurmaIdQuery, IEnumerable<PropostaTurmaDre>>
    {
        private readonly ICacheDistribuido _cacheDistribuido;
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaTurmaDresPorPropostaTurmaIdQueryHandler(ICacheDistribuido cacheDistribuido, IRepositorioProposta repositorioProposta)
        {
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<PropostaTurmaDre>> Handle(ObterPropostaTurmaDresPorPropostaTurmaIdQuery request, CancellationToken cancellationToken)
        {
            var retorno = new List<PropostaTurmaDre>();

            foreach (var propostaTurmaId in request.PropostaTurmaIds)
            {
                var nomeChave = CacheDistribuidoNomes.PropostaTurmaDre.Parametros(propostaTurmaId);
                var dres = await _cacheDistribuido.ObterAsync(nomeChave, () => _repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(propostaTurmaId));
                retorno.AddRange(dres);
            }

            return retorno;
        }
    }
}
