using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNomeCpfProfissionalPorRegistroFuncionalQueryHandler : IRequestHandler<ObterNomeCpfProfissionalPorRegistroFuncionalQuery, RetornoUsuarioDTO>
    {
        private readonly IServicoEol _servicoEol;
        private readonly IMapper _mapper;

        public ObterNomeCpfProfissionalPorRegistroFuncionalQueryHandler(IServicoEol servicoEol, IMapper mapper)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<RetornoUsuarioDTO> Handle(ObterNomeCpfProfissionalPorRegistroFuncionalQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<RetornoUsuarioDTO>(await _servicoEol.ObterNomeCpfProfissionalPorRegistroFuncional(request.RegistroFuncional));
        }
    }
}