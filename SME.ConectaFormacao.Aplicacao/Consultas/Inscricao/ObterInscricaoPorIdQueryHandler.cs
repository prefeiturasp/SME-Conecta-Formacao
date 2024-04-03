using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoPorIdQueryHandler : IRequestHandler<ObterInscricaoPorIdQuery, PaginacaoResultadoDTO<DadosListagemInscricaoDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMapper _mapper;

        public ObterInscricaoPorIdQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper,IRepositorioProposta repositorioProposta)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDTO<DadosListagemInscricaoDTO>> Handle(ObterInscricaoPorIdQuery request, CancellationToken cancellationToken)
        {
            var mapeamento = new List<DadosListagemInscricaoDTO>();
            
            var totalDeRegistros = await _repositorioInscricao.ObterInscricaoPorIdComFiltrosTotalRegistros(request.PropostaId, request.filtros.RegistroFuncional, request.filtros.Cpf, request.filtros.NomeCursista, request.filtros.TurmaId);
            if (totalDeRegistros > 0)
            {
                var inscricoes = await _repositorioInscricao.ObterInscricaoPorIdComFiltros(request.PropostaId, request.filtros.RegistroFuncional, request.filtros.Cpf, request.filtros.NomeCursista,
                    request.filtros.TurmaId, request.NumeroPagina, request.NumeroRegistros);
                mapeamento.AddRange(_mapper.Map<IEnumerable<DadosListagemInscricaoDTO>>(inscricoes));
            }

            return new PaginacaoResultadoDTO<DadosListagemInscricaoDTO>(mapeamento, totalDeRegistros, request.NumeroRegistros);
        }
    }
}