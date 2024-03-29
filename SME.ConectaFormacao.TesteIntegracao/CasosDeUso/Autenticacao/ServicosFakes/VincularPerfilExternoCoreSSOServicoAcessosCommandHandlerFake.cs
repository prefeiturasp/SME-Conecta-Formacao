﻿using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.ServicosFakes
{
    public class VincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFake : IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>
    {
        public Task<bool> Handle(VincularPerfilExternoCoreSSOServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            var perfilUsuario = AutenticacaoMock.UsuarioPerfisRetornoDTOValido.PerfilUsuario ?? new List<PerfilUsuarioDTO>();
            perfilUsuario.Add(new PerfilUsuarioDTO(request.PerfilId, PerfilAutomatico.PERFIL_CURSISTA_DESCRICAO));

            AutenticacaoMock.UsuarioPerfisRetornoDTOValido.PerfilUsuario = perfilUsuario;
            return Task.FromResult(true);
        }
    }
}
