using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDadosPaginadosComFiltrosQueryHandler : IRequestHandler<ObterDadosPaginadosComFiltrosQuery, PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly int QUANTIDADE_MINIMA_PARA_PAGINAR = 10;
        public ObterDadosPaginadosComFiltrosQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao =
                repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private readonly IMapper _mapper;

        public async Task<PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>> Handle(ObterDadosPaginadosComFiltrosQuery request, CancellationToken cancellationToken)
        {
            var retornoComTurmas = new List<DadosListagemFormacaoComTurmaDTO>();
            var totalRegistrosFiltro = await _repositorioInscricao.ObterDadosPaginadosComFiltrosTotalRegistros(request.CodigoFormacao, request.NomeFormacao);
            if (totalRegistrosFiltro > 0)
            {
                var propostasTurmas = await _repositorioInscricao.ObterDadosPaginadosComFiltros(request.CodigoFormacao, request.NomeFormacao, request.NumeroPagina, request.NumeroRegistros, totalRegistrosFiltro);
                var mapeamentoDto = _mapper.Map<IEnumerable<DadosListagemFormacaoComTurmaDTO>>(propostasTurmas);
                var codigosFormacao = propostasTurmas.Select(x => x.Id).ToArray();
                var turmas = await _repositorioInscricao.DadosListagemFormacaoComTurma(codigosFormacao);
                retornoComTurmas.AddRange(ObterTurmas(turmas, mapeamentoDto));
                totalRegistrosFiltro = (totalRegistrosFiltro - ((request.NumeroPagina - 1) * request.NumeroRegistros)) >= QUANTIDADE_MINIMA_PARA_PAGINAR ? totalRegistrosFiltro : propostasTurmas.Count();
            }
            return new PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>(retornoComTurmas, totalRegistrosFiltro, request.NumeroRegistros);
        }

        private IEnumerable<DadosListagemFormacaoComTurmaDTO> ObterTurmas(IEnumerable<ListagemFormacaoComTurmaDTO>? inscricoes, IEnumerable<DadosListagemFormacaoComTurmaDTO> propostas)
        {

            var retorno = propostas;
            foreach (var proposta in propostas)
            {

                var inscricao = inscricoes.Where(x => x.PropostaId == proposta.Id);
                var turmas = inscricao.Select(i => new DadosListagemFormacaoTurma
                {
                    NomeTurma = i.NomeTurma,
                    QuantidadeVagas = i.QuantidadeVagas,
                    QuantidadeInscricoes = i.TotalInscricoes,
                    Data = inscricao.Where(x => x.NomeTurma == i.NomeTurma).Where(x => x.Datas != null).Any() ?
                           string.Join(", ", inscricao.Where(x => x.NomeTurma == i.NomeTurma).Select(x => x.Datas)) : string.Empty
                }).DistinctBy(x => x.NomeTurma);
                retorno.FirstOrDefault(x => x.Id == proposta.Id)!.Turmas = turmas;
            }

            return retorno;
        }
    }
}