﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTokenAcessoQueryHandler : IRequestHandler<ObterTokenAcessoQuery, UsuarioPerfisRetornoDTO>
    {
        private readonly IMediator mediator;

        public ObterTokenAcessoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<UsuarioPerfisRetornoDTO> Handle(ObterTokenAcessoQuery request, CancellationToken cancellationToken)
        {
            var usuarioPerfisRetornoDto = await mediator.Send(new ObterPerfisUsuarioServicoAcessosPorLoginQuery(request.Login));
            if (usuarioPerfisRetornoDto == null)
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, HttpStatusCode.Unauthorized);

            await ValidarPerfisAutomaticos(usuarioPerfisRetornoDto);

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(usuarioPerfisRetornoDto.UsuarioLogin)) ??
                new Dominio.Usuario(usuarioPerfisRetornoDto.UsuarioLogin, usuarioPerfisRetornoDto.UsuarioNome);

            usuario.AtualizarUltimoLogin(DateTimeExtension.HorarioBrasilia());
            await mediator.Send(new SalvarUsuarioCommand(usuario));

            return usuarioPerfisRetornoDto;
        }

        private async Task ValidarPerfisAutomaticos(UsuarioPerfisRetornoDTO usuarioPerfisRetornoDto)
        {
            var perfilCursista = new Guid(PerfilAutomatico.PERFIL_CURSISTA_GUID);
            if (usuarioPerfisRetornoDto.PerfilUsuario == null || !usuarioPerfisRetornoDto.PerfilUsuario.Any(t => t.Perfil == perfilCursista))
            {
                await mediator.Send(new VincularPerfilExternoCoreSSOServicoAcessosCommand(usuarioPerfisRetornoDto.UsuarioLogin, perfilCursista));

                usuarioPerfisRetornoDto = await mediator.Send(new ObterPerfisUsuarioServicoAcessosPorLoginQuery(usuarioPerfisRetornoDto.UsuarioLogin));
            }
        }
    }
}
