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
    public class ObterPropostaTurmasComVagasPorIdQueryHandler(
        IRepositorioProposta repositorioProposta,
        IRepositorioPropostaEncontro repositorioPropostaEncontro,
        IMapper mapper, 
        ICacheDistribuido cacheDistribuido) : IRequestHandler<ObterPropostaTurmasComVagasPorIdQuery, IEnumerable<RetornoListagemDTO>>
    {
        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterPropostaTurmasComVagasPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await repositorioProposta.ObterPorId(request.PropostaId) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            IEnumerable<PropostaTurma> turmas;
            if (proposta.Situacao == Dominio.Enumerados.SituacaoProposta.Publicada && proposta.FormacaoHomologada == Dominio.Enumerados.FormacaoHomologada.Sim)
                turmas = await repositorioProposta.ObterTurmasPorId(proposta.Id);
            else
            {
                turmas = await repositorioProposta.ObterTurmasComVagaPorId(request.PropostaId, request.CodigoDre);
                if (turmas.NaoPossuiElementos())
                    throw new NegocioException(MensagemNegocio.NENHUMA_TURMA_COM_VAGA_DISPONIVEL, System.Net.HttpStatusCode.NotFound);
            }

            foreach (var turma in turmas)
                turma.Nome += await ObterPeríodoEncontrosTurma(turma.Id);

            var lista = mapper.Map<IEnumerable<RetornoListagemDTO>>(turmas);
            lista = lista.OrderBy(x => x.Descricao);
            return lista;
        }


        private async Task<string> ObterPeríodoEncontrosTurma(long turmaId)
        {
            var datasInicio = new List<DateTime>();
            var datasFim = new List<DateTime>();

            var encontros = await cacheDistribuido.ObterAsync(CacheDistribuidoNomes.PropostaTurmaEncontro.Parametros(turmaId), 
                () => repositorioPropostaEncontro.ObterEncontrosPorPropostaTurmaAsync(turmaId));

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
