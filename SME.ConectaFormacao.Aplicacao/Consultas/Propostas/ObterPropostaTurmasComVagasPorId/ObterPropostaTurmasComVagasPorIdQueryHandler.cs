using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Servicos.Interfaces;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Globalization;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmasComVagasPorIdQueryHandler(
        IRepositorioProposta repositorioProposta,
        IMapper mapper,
        IServicoPeriodoEncontroProposta servicoPeriodoEncontroProposta,
        IRepositorioPropostaGrupoPeriodoInscricao repositorioPropostaGrupoPeriodoInscricao) : IRequestHandler<ObterPropostaTurmasComVagasPorIdQuery, IEnumerable<PropostaTurmaInscricaoDTO>>
    {
        public async Task<IEnumerable<PropostaTurmaInscricaoDTO>> Handle(ObterPropostaTurmasComVagasPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await repositorioProposta.ObterPorId(request.PropostaId) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            IEnumerable<PropostaTurma> turmas;
            turmas = await repositorioProposta.ObterTurmasComVagaPorId(request.PropostaId, request.CodigoDre);

            if (turmas.NaoPossuiElementos())
                throw new NegocioException(MensagemNegocio.NENHUMA_TURMA_COM_VAGA_DISPONIVEL, System.Net.HttpStatusCode.NotFound);

            if (request.ApenasPeriodoInscricaoVigente)
            {
                var gruposInscricao = await repositorioPropostaGrupoPeriodoInscricao.ObterPorPropostaIdAsync(request.PropostaId);
                turmas = [.. turmas.Where(t => proposta.TurmaEstaEmPeriodoDeInscricao(t.Id, gruposInscricao))];
            }

            foreach (var turma in turmas)
                turma.Nome += await servicoPeriodoEncontroProposta.ObterPeriodoEncontrosTurmaAsync(turma.Id);

            var lista = mapper.Map<IEnumerable<PropostaTurmaInscricaoDTO>>(turmas);

            StringComparer numComparer = StringComparer.Create(CultureInfo.CurrentCulture, CompareOptions.NumericOrdering);
            return [.. lista.OrderBy(x => x.Descricao, numComparer)];
        }
    }
}
