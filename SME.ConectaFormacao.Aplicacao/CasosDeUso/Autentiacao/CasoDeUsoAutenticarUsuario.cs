using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

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
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, 401);

            var usuarioPerfisRetornoDto = await mediator.Send(new ObterPerfisUsuarioServicoAcessosPorLoginQuery(autenticacaoDTO.Login));
            if (usuarioPerfisRetornoDto == null)
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, 401);

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(usuarioAutenticadoRetornoDto.Login)) ??
                new Usuario(usuarioAutenticadoRetornoDto.Login, usuarioAutenticadoRetornoDto.Nome);

            usuario.AtualizarUltimoLogin(DateTimeExtension.HorarioBrasilia());
            await mediator.Send(new SalvarUsuarioCommand(usuario));

            return usuarioPerfisRetornoDto;
        }
    }
}
