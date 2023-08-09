using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao
{
    public class CasoDeUsoAutenticarUsuario : CasoDeUsoAbstrato, ICasoDeUsoAutenticarUsuario
    {
        public CasoDeUsoAutenticarUsuario(IMediator mediator) : base(mediator)
        {
        }

        public async Task<UsuarioPerfisRetornoDTO> Executar(AutenticacaoDTO autenticacaoDTO)
        {
            var usuarioAutenticadoRetornoDto = await mediator.Send(new ObterUsuarioServicoAcessosPorLoginSenhaQuery(autenticacaoDTO.Login, autenticacaoDTO.Senha));

            if (string.IsNullOrEmpty(usuarioAutenticadoRetornoDto.Login))
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, HttpStatusCode.Unauthorized);

            var usuarioPerfisRetornoDto = await mediator.Send(new ObterPerfisUsuarioServicoAcessosPorLoginQuery(autenticacaoDTO.Login));
            if (usuarioPerfisRetornoDto == null)
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, HttpStatusCode.Unauthorized);

            await ValidarPerfisAutomaticos(usuarioPerfisRetornoDto);

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(usuarioAutenticadoRetornoDto.Login)) ??
                new Dominio.Usuario(usuarioAutenticadoRetornoDto.Login, usuarioAutenticadoRetornoDto.Nome);

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
