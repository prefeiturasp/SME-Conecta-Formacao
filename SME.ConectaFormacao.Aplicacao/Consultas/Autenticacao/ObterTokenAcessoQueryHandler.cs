﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTokenAcessoQueryHandler : IRequestHandler<ObterTokenAcessoQuery, UsuarioPerfisRetornoDTO>
    {
        private readonly IMediator mediator;
        private const int TAMANHO_RF = 7;

        public ObterTokenAcessoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<UsuarioPerfisRetornoDTO> Handle(ObterTokenAcessoQuery request, CancellationToken cancellationToken)
        {
            var usuarioPerfisRetornoDto = await mediator.Send(new ObterPerfisUsuarioServicoAcessosPorLoginQuery(request.Login, request.PerfilUsuarioId), cancellationToken)
                                          ?? throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, HttpStatusCode.Unauthorized);

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(request.Login), cancellationToken);

            if (usuario == null && request.Login.Trim().Length > TAMANHO_RF)
                throw new NegocioException(MensagemNegocio.REALIZE_SEU_CADASTRO_NO_SISTEMA, HttpStatusCode.Unauthorized);

            if (usuario == null)
            {
                usuario = new Usuario(usuarioPerfisRetornoDto.UsuarioLogin, usuarioPerfisRetornoDto.UsuarioNome, usuarioPerfisRetornoDto.Email);
            }

            usuarioPerfisRetornoDto = await ValidarPerfisAutomaticos(request, usuarioPerfisRetornoDto, cancellationToken);

            if (usuario.Tipo == Dominio.Enumerados.TipoUsuario.Externo && usuario.EstaAguardandoValidacaoEmail())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_VALIDOU_EMAIL, HttpStatusCode.Unauthorized);

            usuario.Atualizar(usuarioPerfisRetornoDto.Email, DateTimeExtension.HorarioBrasilia(), usuarioPerfisRetornoDto.Cpf);
            await mediator.Send(new SalvarUsuarioCommand(usuario), cancellationToken);

            return usuarioPerfisRetornoDto;
        }

        private async Task<UsuarioPerfisRetornoDTO> ValidarPerfisAutomaticos(ObterTokenAcessoQuery request, UsuarioPerfisRetornoDTO usuarioPerfisRetornoDto, CancellationToken cancellationToken)
        {
            var perfilCursista = new Guid(PerfilAutomatico.PERFIL_CURSISTA_GUID);
            if (usuarioPerfisRetornoDto.PerfilUsuario == null || !usuarioPerfisRetornoDto.PerfilUsuario.Any(t => t.Perfil == perfilCursista))
            {
                await mediator.Send(new VincularPerfilExternoCoreSSOServicoAcessosCommand(request.Login, perfilCursista), cancellationToken);

                usuarioPerfisRetornoDto = await mediator.Send(new ObterPerfisUsuarioServicoAcessosPorLoginQuery(request.Login, request.PerfilUsuarioId), cancellationToken);
            }

            return usuarioPerfisRetornoDto;
        }
    }
}
