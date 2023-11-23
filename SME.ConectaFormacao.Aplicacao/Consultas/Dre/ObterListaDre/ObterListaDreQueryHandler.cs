using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterListaDreQueryHandler : IRequestHandler<ObterListaDreQuery, IEnumerable<RetornoListagemDTO>>
    {
        public ObterListaDreQueryHandler(IMapper mapper, IRepositorioDre repositorioDre)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioDre = repositorioDre ?? throw new ArgumentNullException(nameof(repositorioDre));
        }

        private readonly IMapper _mapper;
        private readonly IRepositorioDre _repositorioDre;

        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterListaDreQuery request, CancellationToken cancellationToken)
        {
            var dres = (await _repositorioDre.ObterTodos()).Where(w => !w.Excluido);
            return _mapper.Map<IEnumerable<RetornoListagemDTO>>(dres);
        }
    }
}