using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCriterioValidacaoInscricaoQueryHandler : IRequestHandler<ObterCriterioValidacaoInscricaoQuery, IEnumerable<CriterioValidacaoInscricaoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioCriterioAvaliacaoInscricao _repositorioCriterioAvaliacaoInscricao;

        public ObterCriterioValidacaoInscricaoQueryHandler(IMapper mapper, IRepositorioCriterioAvaliacaoInscricao repositorioCriterioAvaliacaoInscricao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioCriterioAvaliacaoInscricao = repositorioCriterioAvaliacaoInscricao ?? throw new ArgumentNullException(nameof(repositorioCriterioAvaliacaoInscricao));
        }

        public async Task<IEnumerable<CriterioValidacaoInscricaoDTO>> Handle(ObterCriterioValidacaoInscricaoQuery request, CancellationToken cancellationToken)
        {
            var criterios = await _repositorioCriterioAvaliacaoInscricao.ObterTodos();
            return _mapper.Map<IEnumerable<CriterioValidacaoInscricaoDTO>>(criterios);
        }
    }
}
