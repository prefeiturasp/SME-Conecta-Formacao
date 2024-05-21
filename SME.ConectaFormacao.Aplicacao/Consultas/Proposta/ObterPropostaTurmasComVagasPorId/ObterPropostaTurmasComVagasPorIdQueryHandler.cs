using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmasComVagasPorIdQueryHandler : IRequestHandler<ObterPropostaTurmasComVagasPorIdQuery, IEnumerable<RetornoListagemDTO>>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterPropostaTurmasComVagasPorIdQueryHandler(IRepositorioProposta repositorioProposta, IMapper mapper, ICacheDistribuido cacheDistribuido)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterPropostaTurmasComVagasPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.PropostaId) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            IEnumerable<PropostaTurma> turmas;
            if (proposta.Situacao == Dominio.Enumerados.SituacaoProposta.Publicada && proposta.FormacaoHomologada == Dominio.Enumerados.FormacaoHomologada.Sim)
                turmas = await _repositorioProposta.ObterTurmasPorId(proposta.Id);
            else
            {
                turmas = await _repositorioProposta.ObterTurmasComVagaPorId(request.PropostaId);
                if (turmas.NaoPossuiElementos())
                    throw new NegocioException(MensagemNegocio.NENHUMA_TURMA_COM_VAGA_DISPONIVEL, System.Net.HttpStatusCode.NotFound);
            }

            foreach (var turma in turmas)
                turma.Nome += await ObterPeríodoEncontrosTurma(turma.Id);

            var lista = _mapper.Map<IEnumerable<RetornoListagemDTO>>(turmas);
            lista = lista.OrderBy(x => x.Descricao);
            return lista;
        }


        private async Task<string> ObterPeríodoEncontrosTurma(long turmaId)
        {
            var datasInicio = new List<DateTime>();
            var datasFim = new List<DateTime>();

            var encontros = await _cacheDistribuido.ObterAsync(CacheDistribuidoNomes.PropostaTurmaEncontro.Parametros(turmaId), () => _repositorioProposta.ObterEncontrosPorPropostaTurmaId(turmaId));

            foreach (var encontro in encontros)
            {
                foreach (var data in encontro.Datas)
                {
                    if (data.DataInicio.HasValue)
                        datasInicio.Add(data.DataInicio.Value);

                    if (data.DataFim.HasValue)
                        datasFim.Add(data.DataFim.Value);
                }
            }

            var menorDataInicio = datasInicio.OrderBy(o => o.Date).FirstOrDefault();
            DateTime? maiorDataFim = null;
            if (datasFim.NaoPossuiElementos() && datasInicio.Count > 1)
            {
                maiorDataFim = datasInicio.OrderBy(o => o.Date).LastOrDefault();
            }
            else if (datasFim.PossuiElementos())
            {
                maiorDataFim = datasFim.OrderBy(o => o.Date).LastOrDefault();
            }

            return maiorDataFim != null ? $" {menorDataInicio:dd/MM/yyyy} até {maiorDataFim:dd/MM/yyyy}" : $" {menorDataInicio:dd/MM/yyyy}";
        }
    }
}
