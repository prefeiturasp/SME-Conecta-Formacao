using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterListaDreQueryHandler : IRequestHandler<ObterListaDreQuery, IEnumerable<DreDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioDre _repositorioDre;

        public ObterListaDreQueryHandler(IMapper mapper, IRepositorioDre repositorioDre)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioDre = repositorioDre ?? throw new ArgumentNullException(nameof(repositorioDre));
        }

        public async Task<IEnumerable<DreDTO>> Handle(ObterListaDreQuery request, CancellationToken cancellationToken)
        {
            var dres = await _repositorioDre.ObterIgnorandoExcluidos(request.ExibirTodos);
            return _mapper.Map<IEnumerable<DreDTO>>(dres);
        }
    }
}