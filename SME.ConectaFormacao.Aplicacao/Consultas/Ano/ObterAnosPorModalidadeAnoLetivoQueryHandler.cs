using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAnosPorModalidadeAnoLetivoQueryHandler : IRequestHandler<ObterAnosPorModalidadeAnoLetivoQuery, IEnumerable<RetornoListagemTodosDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAno _repositorioAno;
        
        public ObterAnosPorModalidadeAnoLetivoQueryHandler(IMapper mapper, IRepositorioAno repositorioAno)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioAno = repositorioAno ?? throw new ArgumentNullException(nameof(repositorioAno));
        }

        public async Task<IEnumerable<RetornoListagemTodosDTO>> Handle(ObterAnosPorModalidadeAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            var anos = await _repositorioAno.ObterAnosPorModalidadeAnoLetivo(request.Modalidade, request.AnoLetivo);
            return _mapper.Map<IEnumerable<RetornoListagemTodosDTO>>(anos);
        }
    }
}
