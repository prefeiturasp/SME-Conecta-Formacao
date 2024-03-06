using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao
{
    public class CasoDeUsoAutenticarAlterarPerfil : CasoDeUsoAbstrato, ICasoDeUsoAutenticarAlterarPerfil
    {
        public CasoDeUsoAutenticarAlterarPerfil(IMediator mediator) : base(mediator)
        {
        }

        public async Task<UsuarioPerfisRetornoDTO> Executar(Guid PerfilUsuarioId)
        {
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia())
                ?? throw new NegocioException(MensagemNegocio.LOGIN_NAO_ENCONTRADO, System.Net.HttpStatusCode.Unauthorized);

            return await mediator.Send(new ObterTokenAcessoQuery(usuarioLogado.Login, PerfilUsuarioId));
        }
    }
}
