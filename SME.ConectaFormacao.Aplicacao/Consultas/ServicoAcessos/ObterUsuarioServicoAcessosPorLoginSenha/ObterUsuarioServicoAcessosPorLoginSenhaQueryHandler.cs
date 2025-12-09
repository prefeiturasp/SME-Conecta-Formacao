using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioServicoAcessosPorLoginSenhaQueryHandler : IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery, UsuarioAutenticacaoRetornoDto>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;

        public ObterUsuarioServicoAcessosPorLoginSenhaQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<UsuarioAutenticacaoRetornoDto> Handle(ObterUsuarioServicoAcessosPorLoginSenhaQuery request, CancellationToken cancellationToken)
        {
            var usuarioAutenticacaoRetorno = await _servicoAcessos.Autenticar(request.Login, request.Senha);
            return _mapper.Map<UsuarioAutenticacaoRetornoDto>(usuarioAutenticacaoRetorno);
        }
    }
}
