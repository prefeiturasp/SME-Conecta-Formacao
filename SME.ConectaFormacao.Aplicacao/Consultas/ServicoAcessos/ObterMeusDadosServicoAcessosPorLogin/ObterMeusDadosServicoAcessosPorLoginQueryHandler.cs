using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandler : IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;
        private readonly IRepositorioUsuario _repositorioUsuario;

        public ObterMeusDadosServicoAcessosPorLoginQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos,IRepositorioUsuario repositorioUsuario)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
            this._repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<DadosUsuarioDTO> Handle(ObterMeusDadosServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            var acessoDadosUsuario = await _servicoAcessos.ObterMeusDados(request.Login);
            acessoDadosUsuario.EmailEducacional = await _repositorioUsuario.ObterEmailEducacionalPorLogin(request.Login);
            return _mapper.Map<DadosUsuarioDTO>(acessoDadosUsuario);
        }
    }
}
