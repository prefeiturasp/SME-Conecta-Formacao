using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.CasosDeUso;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao
{
    public class CasoDeUsoAutenticar : CasoDeUsoAbstrato, ICasoDeUsoAutenticar
    {
        public CasoDeUsoAutenticar(IMediator mediator) : base(mediator)
        {
        }

        public async Task<UsuarioAutenticacaoRetornoDTO> Executar(string login, string senha)
        {
            var usuarioAutenticadoRetorno = await mediator.Send(new ObterUsuarioServicoAcessoPorLoginSenhaQuery(login, senha));

            if (string.IsNullOrEmpty(usuarioAutenticadoRetorno.Login))
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS);

            return usuarioAutenticadoRetorno;
        }
    }
}
