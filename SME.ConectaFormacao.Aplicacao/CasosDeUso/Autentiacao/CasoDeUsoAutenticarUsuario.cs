using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
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

            return await mediator.Send(new ObterTokenAcessoQuery(usuarioAutenticadoRetornoDto.Login));
        }
    }
}
