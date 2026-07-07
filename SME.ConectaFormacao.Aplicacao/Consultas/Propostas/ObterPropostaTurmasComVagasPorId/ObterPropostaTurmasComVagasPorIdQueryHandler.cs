using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Servicos.Interfaces;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmasComVagasPorIdQueryHandler(
        IRepositorioProposta repositorioProposta,
        IMapper mapper,
        IServicoPeriodoEncontroProposta servicoPeriodoEncontroProposta) : IRequestHandler<ObterPropostaTurmasComVagasPorIdQuery, IEnumerable<RetornoListagemDTO>>
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
                turma.Nome += await servicoPeriodoEncontroProposta.ObterPeriodoEncontrosTurmaAsync(turma.Id);

            var lista = mapper.Map<IEnumerable<RetornoListagemDTO>>(turmas);
            lista = lista.OrderBy(x => x.Descricao);
            return lista;
        }
    }
}