using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios : CasoDeUsoAbstrato ,ICasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios
    {
        public CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var inscricao = param.ObterObjetoMensagem<Inscricao>();
            var usuario = await mediator.Send(new ObterUsuarioPorIdQuery(inscricao.UsuarioId));
            if (usuario == null)
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
            
            
            var usuarioAtivo = await mediator.Send(new VerificarSeUsuarioPossuiCargoAtivoNoEolQuery(usuario.Login));
            if (!usuarioAtivo)
            {
                await mediator.Send(new CancelarInscricaoCommand(inscricao.Id));
            }
            return true;
        }
    }
}