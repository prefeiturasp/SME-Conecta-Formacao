using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes
{
    public class VincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFake : IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>
    {
        public Task<bool> Handle(VincularPerfilExternoCoreSSOServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            var perfilUsuario = UsuarioRecuperarSenhaMock.UsuarioPerfisRetornoDTOValido.PerfilUsuario ?? new List<PerfilUsuarioDTO>();
            perfilUsuario.Add(new PerfilUsuarioDTO(request.PerfilId, PerfilAutomatico.PERFIL_CURSISTA_DESCRICAO));

            UsuarioRecuperarSenhaMock.UsuarioPerfisRetornoDTOValido.PerfilUsuario = perfilUsuario;
            return Task.FromResult(true);
        }
    }
}
