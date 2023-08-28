using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargoFuncaoQueryHandler : IRequestHandler<ObterCargoFuncaoQuery, IEnumerable<CargoFuncaoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioCargoFuncao _repositorioCargoFuncao;

        public ObterCargoFuncaoQueryHandler(IMapper mapper, IRepositorioCargoFuncao repositorioCargoFuncao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioCargoFuncao = repositorioCargoFuncao ?? throw new ArgumentNullException(nameof(repositorioCargoFuncao));
        }

        public async Task<IEnumerable<CargoFuncaoDTO>> Handle(ObterCargoFuncaoQuery request, CancellationToken cancellationToken)
        {
            var cargosFuncoes = await _repositorioCargoFuncao.ObterPorTipo(request.Tipo);
            return _mapper.Map<IEnumerable<CargoFuncaoDTO>>(cargosFuncoes);
        }
    }
}
