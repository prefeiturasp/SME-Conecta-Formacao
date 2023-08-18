using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoUsuarioRecuperarSenha : CasoDeUsoAbstrato, ICasoDeUsoUsuarioRecuperarSenha
    {
        public CasoDeUsoUsuarioRecuperarSenha(IMediator mediator) : base(mediator)
        {
        }

        public async Task<UsuarioPerfisRetornoDTO> Executar(RecuperacaoSenhaDto recuperacaoSenhaDto)
        {
            var login = await mediator.Send(new AlterarSenhaServicoAcessosPorTokenCommand(recuperacaoSenhaDto.Token, recuperacaoSenhaDto.NovaSenha));
            return await mediator.Send(new ObterTokenAcessoQuery(login));
        }
    }
}
