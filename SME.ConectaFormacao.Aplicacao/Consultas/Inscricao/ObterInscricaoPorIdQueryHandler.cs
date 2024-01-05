using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoPorIdQueryHandler : IRequestHandler<ObterInscricaoPorIdQuery, IEnumerable<DadosListagemInscricaoDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMapper _mapper;

        public ObterInscricaoPorIdQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<DadosListagemInscricaoDTO>> Handle(ObterInscricaoPorIdQuery request, CancellationToken cancellationToken)
        {
            var inscricao = await _repositorioInscricao.ObterInscricaoPorIdComFiltros(request.InscricaoId, request.filtros.RegistroFuncional, request.filtros.Cpf, request.filtros.NomeCursista);
            return _mapper.Map<IEnumerable<DadosListagemInscricaoDTO>>(inscricao);
        }
    }
}