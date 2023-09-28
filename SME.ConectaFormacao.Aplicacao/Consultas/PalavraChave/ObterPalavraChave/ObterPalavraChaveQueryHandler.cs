using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPalavraChaveQueryHandler : IRequestHandler<ObterPalavraChaveQuery, IEnumerable<PalavraChaveDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioPalavraChave _repositorioPalavraChave;

        public ObterPalavraChaveQueryHandler(IMapper mapper, IRepositorioPalavraChave repositorioPalavraChave)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioPalavraChave = repositorioPalavraChave ?? throw new ArgumentNullException(nameof(repositorioPalavraChave));
        }

        public async Task<IEnumerable<PalavraChaveDTO>> Handle(ObterPalavraChaveQuery request, CancellationToken cancellationToken)
        {
            var palavrasChaves = (await _repositorioPalavraChave.ObterTodos()).Where(w=> !w.Excluido);
            return _mapper.Map<IEnumerable<PalavraChaveDTO>>(palavrasChaves);
        }
    }
}
