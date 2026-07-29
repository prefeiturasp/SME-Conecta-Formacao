using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.AlterarNomeSocialServicoAcessos;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosServidorPorRfEol;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTokenAcessoQueryHandler
        (IMediator mediator) : IRequestHandler<ObterTokenAcessoQuery, UsuarioPerfisRetornoDTO>
    {
        public async Task<UsuarioPerfisRetornoDTO> Handle(ObterTokenAcessoQuery request, CancellationToken cancellationToken)
        {
            var usuarioPerfisRetornoDto = await ObterPerfisUsuarioAcessos(request, cancellationToken);

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(request.Login), cancellationToken);

            usuario ??= new(usuarioPerfisRetornoDto.UsuarioLogin, usuarioPerfisRetornoDto.UsuarioNome, usuarioPerfisRetornoDto.Email, usuarioPerfisRetornoDto.NomeSocial);

            usuarioPerfisRetornoDto = await ValidarPerfisAutomaticos(request, usuarioPerfisRetornoDto, cancellationToken);

            if (usuario.Tipo.EhExterno() && usuario.EstaAguardandoValidacaoEmail())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_VALIDOU_EMAIL, HttpStatusCode.Unauthorized);

            if (usuario.Tipo.EhInterno())
            {
                var dadosUsuarioEOL = await mediator.Send(new ObterDadosServidorPorRfEolQuery(request.Login), cancellationToken);
                usuarioPerfisRetornoDto.UsuarioNome = usuarioPerfisRetornoDto.UsuarioNome ?? dadosUsuarioEOL?.Nome ?? string.Empty;
                usuarioPerfisRetornoDto.NomeSocial = usuarioPerfisRetornoDto.NomeSocial ?? dadosUsuarioEOL?.NomeSocial;
            }

            usuario.Atualizar(usuarioPerfisRetornoDto.Email, DateTimeExtension.HorarioBrasilia(), usuarioPerfisRetornoDto.Cpf, usuarioPerfisRetornoDto.UsuarioNome, usuarioPerfisRetornoDto.NomeSocial);


            var alterouNomeUsuario = !usuarioPerfisRetornoDto.UsuarioNome.Equals(usuario.Nome);
            var alterouNomeSocialUsuario = !string.Equals(usuarioPerfisRetornoDto.NomeSocial, usuario.NomeSocial, StringComparison.Ordinal);

            await mediator.Send(new SalvarUsuarioCommand(usuario, alterouNomeUsuario), cancellationToken);
            if (alterouNomeSocialUsuario)
                await mediator.Send(new AlterarNomeSocialServicoAcessosCommand(usuarioPerfisRetornoDto.UsuarioLogin, usuarioPerfisRetornoDto.NomeSocial), cancellationToken);

            if (alterouNomeUsuario)
            {
                await mediator.Send(new AlterarNomeServicoAcessosCommand(usuarioPerfisRetornoDto.UsuarioLogin, usuarioPerfisRetornoDto.UsuarioNome), cancellationToken);
                return await ObterPerfisUsuarioAcessos(request, cancellationToken);
            }

            return usuarioPerfisRetornoDto;
        }

        private async Task<UsuarioPerfisRetornoDTO> ObterPerfisUsuarioAcessos(ObterTokenAcessoQuery request, CancellationToken cancellationToken)
        {
            return await mediator.Send(new ObterPerfisUsuarioServicoAcessosPorLoginQuery(request.Login, request.PerfilUsuarioId), cancellationToken)
                   ?? throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, HttpStatusCode.Unauthorized);
        }

        private async Task<UsuarioPerfisRetornoDTO> ValidarPerfisAutomaticos(ObterTokenAcessoQuery request, UsuarioPerfisRetornoDTO usuarioPerfisRetornoDto, CancellationToken cancellationToken)
        {
            var perfilCursista = PerfilAutomatico.PERIL_CURSISTA_CODIGO;
            if (usuarioPerfisRetornoDto.PerfilUsuario.EhNulo() || !usuarioPerfisRetornoDto.PerfilUsuario.Any(t => t.Perfil == perfilCursista))
            {
                await mediator.Send(new VincularPerfilExternoCoreSSOServicoAcessosCommand(request.Login, perfilCursista), cancellationToken);
                usuarioPerfisRetornoDto = await mediator.Send(new ObterPerfisUsuarioServicoAcessosPorLoginQuery(request.Login, request.PerfilUsuarioId), cancellationToken);
            }

            return usuarioPerfisRetornoDto;
        }
    }
}
