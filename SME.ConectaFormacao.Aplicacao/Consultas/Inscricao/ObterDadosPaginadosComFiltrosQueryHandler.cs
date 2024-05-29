using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDadosPaginadosComFiltrosQueryHandler : IRequestHandler<ObterDadosPaginadosComFiltrosQuery, PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMapper _mapper;

        public ObterDadosPaginadosComFiltrosQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>> Handle(ObterDadosPaginadosComFiltrosQuery request, CancellationToken cancellationToken)
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

                retornoComTurmas.AddRange(MapearTurmasETipoInscricao(formacao, turmas, tiposInscricao));
            }

            return new PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>(retornoComTurmas, totalRegistrosFiltro, request.NumeroRegistros);
        }

        private static IEnumerable<DadosListagemFormacaoComTurmaDTO> MapearTurmasETipoInscricao(IEnumerable<DadosListagemFormacaoComTurmaDTO> formacoes, IEnumerable<ListagemFormacaoComTurmaDTO>? turmasFormacao, IEnumerable<PropostaTipoInscricao> tipoInscricaos)
        {
            foreach (var proposta in formacoes)
            {
                var inscricao = turmasFormacao?.Where(x => x.PropostaId == proposta.Id);
                
                var turmas = inscricao!
                    .GroupBy(i => new { i.NomeTurma, i.QuantidadeVagas })
                    .Select(g => new DadosListagemFormacaoTurma
                    {
                        NomeTurma = g.Key.NomeTurma,
                        QuantidadeVagas = g.Key.QuantidadeVagas,
                        QuantidadeInscricoes = g.Sum(i => i.TotalInscricoes),
                        Confirmadas = g.Where(i => i.Situacao.EhConfirmada()).Sum(i => (int?)i.TotalInscricoes) ?? 0,
                        AguardandoAnalise = g.Where(i => i.Situacao.EhAguardandoAnalise()).Sum(i => (int?)i.TotalInscricoes) ?? 0,
                        EmEspera = g.Where(i => i.Situacao.EhEmEspera()).Sum(i => (int?)i.TotalInscricoes) ?? 0,
                        Data = ObterData(inscricao, g.First()),
                        PodeRealizarSorteio = g.Key.QuantidadeVagas > 0 && g.Sum(i => (int?)i.TotalInscricoes) > g.Key.QuantidadeVagas
                    })
                    .DistinctBy(x => x.NomeTurma)
                    .ToList();

                proposta.Turmas = turmas;
                proposta.TiposInscricoes = tipoInscricaos.Where(t => t.PropostaId == proposta.Id).Select(s => s.TipoInscricao);
            }

            return formacoes;
        }

        private static string ObterData(IEnumerable<ListagemFormacaoComTurmaDTO>? inscricao, ListagemFormacaoComTurmaDTO i)
        {
            return inscricao!.Any(x => x.NomeTurma.Equals(i.NomeTurma) && x.Datas.NaoEhNulo()) 
                ? string.Join(", ", inscricao!.Where(x => x.NomeTurma.Equals(i.NomeTurma)).Select(x => x.Datas).Distinct()) 
                : string.Empty;
        }
    }
}