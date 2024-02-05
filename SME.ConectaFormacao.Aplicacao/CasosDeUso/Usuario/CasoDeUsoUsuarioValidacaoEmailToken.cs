using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoUsuarioValidacaoEmailToken : CasoDeUsoAbstrato, ICasoDeUsoUsuarioValidacaoEmailToken
    {
        public CasoDeUsoUsuarioValidacaoEmailToken(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(Guid token)
        {
            var login = await mediator.Send(new ObterLoginUsuarioTokenServicoAcessosQuery(token, TipoAcao.ValidacaoEmail));
            
            if (login.NaoEstaPreenchido())
                throw new NegocioException(MensagemNegocio.TOKEN_INVALIDO);
            
            return await mediator.Send(new AtivarUsuarioExternoCommand(login));
        }
    }
}
