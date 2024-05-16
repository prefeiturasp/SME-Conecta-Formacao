using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuarioRedeParceria
{
    public class CasoDeUsoRemoverUsuarioRedeParceria : CasoDeUsoAbstrato, ICasoDeUsoRemoverUsuarioRedeParceria
    {
        public CasoDeUsoRemoverUsuarioRedeParceria(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorIdQuery(id)) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            if(!usuario.Tipo.EhRedeParceria())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            var usuarioPossuiProposta = await mediator.Send(new UsuarioPossuiPropostaQuery(usuario.Login));

            if (usuarioPossuiProposta)
            {
                usuario.Situacao = SituacaoUsuario.Inativo;
                await mediator.Send(new SalvarUsuarioCommand(usuario));
            }
            else
            {
                await mediator.Send(new RemoverUsuarioCommand(id));
            }

            // TODO: Remover no coresso.

            return true;
        }
    }
}
