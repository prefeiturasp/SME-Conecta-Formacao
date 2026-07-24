using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDadosPaginadosComFiltrosQueryHandler : IRequestHandler<ObterDadosPaginadosComFiltrosQuery, PaginacaoResultadoDto<DadosListagemFormacaoComTurmaDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMapper _mapper;

        public ObterDadosPaginadosComFiltrosQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDto<DadosListagemFormacaoComTurmaDTO>> Handle(ObterDadosPaginadosComFiltrosQuery request, CancellationToken cancellationToken)
        {
            var retornoComTurmas = new List<DadosListagemFormacaoComTurmaDTO>();
            var totalRegistrosFiltro = await _repositorioInscricao.ObterDadosPaginadosComFiltrosTotalRegistros(request.AreaPromotoraIdUsuarioLogado, request.CodigoFormacao, request.NomeFormacao, request.NumeroHomologacao);
            if (totalRegistrosFiltro > 0)
            {
                var propostasTurmas = await _repositorioInscricao.ObterDadosPaginadosComFiltros(request.AreaPromotoraIdUsuarioLogado, request.CodigoFormacao, request.NomeFormacao, request.NumeroPagina, request.NumeroRegistros, request.NumeroHomologacao);

                var formacao = _mapper.Map<IEnumerable<DadosListagemFormacaoComTurmaDTO>>(propostasTurmas);
                var codigosFormacao = propostasTurmas.Select(x => x.Id).ToArray();

                var turmas = await _repositorioInscricao.DadosListagemFormacaoComTurma(codigosFormacao);
                var tiposInscricao = await _repositorioInscricao.ObterTiposInscricaoPorPropostaIds(codigosFormacao);

                retornoComTurmas.AddRange(MapearTurmasETipoInscricao(formacao, turmas, tiposInscricao, request.ApenasSemCodaf));
            }

            return new PaginacaoResultadoDto<DadosListagemFormacaoComTurmaDTO>(retornoComTurmas, totalRegistrosFiltro, request.NumeroRegistros);
        }

        private static IEnumerable<DadosListagemFormacaoComTurmaDTO> MapearTurmasETipoInscricao(IEnumerable<DadosListagemFormacaoComTurmaDTO> formacoes, IEnumerable<ListagemFormacaoComTurmaDTO>? turmasFormacao, IEnumerable<PropostaTipoInscricao> tipoInscricaos, bool? apenasSemCodaf)
        {
            foreach (var proposta in formacoes)
            {
                var inscricao = turmasFormacao?.Where(x => x.PropostaId == proposta.Id) ?? Enumerable.Empty<ListagemFormacaoComTurmaDTO>();

                var turmas = inscricao
                    .Where(i => (apenasSemCodaf != true) || !i.CodafId.HasValue)
                    .Select(i => new DadosListagemFormacaoTurma
                    {
                        PropostaTurmaId = i.PropostaTurmaId,
                        NomeTurma = i.NomeTurma,
                        QuantidadeVagas = i.QuantidadeVagas,
                        QuantidadeInscricoes = i.TotalInscricoes,
                        Data = ObterData(inscricao, i),
                        QuantidadeConfirmada = i.Confirmadas,
                        QuantidadeAguardandoAnalise = i.AguardandoAnalise,
                        QuantidadeEmEspera = i.EmEspera,
                        QuantidadeCancelada = i.Cancelada,
                        QuantidadeDisponivel = i.Disponiveis,
                        QuantidadeExcedida = i.Excedidas,
                        Permissao = new DadosListagemFormacaoTurmaPermissao
                        {
                            PodeRealizarSorteio = i.PermiteSorteio.GetValueOrDefault() && i.Disponiveis > 0 && i.Excedidas > 0 && i.AguardandoAnalise > 0
                        }
                    }).DistinctBy(x => x.NomeTurma)
                    .ToList();

                proposta.Turmas = turmas;
                proposta.TiposInscricoes = tipoInscricaos.Where(t => t.PropostaId == proposta.Id).Select(s => s.TipoInscricao);
            }

            return formacoes;
        }

        private static string ObterData(IEnumerable<ListagemFormacaoComTurmaDTO>? inscricoes, ListagemFormacaoComTurmaDTO turmaReferencia)
        {
            if (inscricoes is null || string.IsNullOrWhiteSpace(turmaReferencia.NomeTurma))
                return string.Empty;

            var datasValidas = inscricoes
                .Where(x => x.NomeTurma == turmaReferencia.NomeTurma && !string.IsNullOrWhiteSpace(x.Datas))
                .Select(x => x.Datas)
                .Distinct();

            return string.Join(", ", datasValidas);
        }
    }
}