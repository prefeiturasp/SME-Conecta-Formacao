using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioRedeParceriaPorIdQueryHandler : IRequestHandler<ObterUsuarioRedeParceriaPorIdQuery, UsuarioRedeParceriaDTO>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMapper _mapper;

        public ObterUsuarioRedeParceriaPorIdQueryHandler(IRepositorioUsuario repositorioUsuario, IMapper mapper)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UsuarioRedeParceriaDTO> Handle(ObterUsuarioRedeParceriaPorIdQuery request, CancellationToken cancellationToken)
        {
            var usuario = await _repositorioUsuario.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            if(!usuario.Tipo.EhRedeParceria())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            return _mapper.Map<UsuarioRedeParceriaDTO>(usuario);
        }
    }
}
