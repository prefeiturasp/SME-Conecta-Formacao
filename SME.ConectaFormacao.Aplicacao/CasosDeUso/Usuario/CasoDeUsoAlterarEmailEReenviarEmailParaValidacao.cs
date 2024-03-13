using System.Net;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoAlterarEmailEReenviarEmailParaValidacao : CasoDeUsoAbstrato, ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao
    {
        public CasoDeUsoAlterarEmailEReenviarEmailParaValidacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(AlterarEmailUsuarioDto emailUsuarioDto)
        {
            var usuarioAutenticadoRetornoDto = await mediator.Send(new ObterUsuarioServicoAcessosPorLoginSenhaQuery(emailUsuarioDto.Login, emailUsuarioDto.Senha));
            if (string.IsNullOrEmpty(usuarioAutenticadoRetornoDto.Login))
                throw new NegocioException(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, HttpStatusCode.Unauthorized);
            else
            {
                await mediator.Send(new AlterarEmailServicoAcessosCommand(emailUsuarioDto.Login, emailUsuarioDto.Email));
                return await mediator.Send(new EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand(emailUsuarioDto.Login));
            }
        }
    }
}