using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios : CasoDeUsoAbstrato,
        ICasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios
    {
        public CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var usuariosInscricao = param.ObterObjetoMensagem<IEnumerable<InscricaoUsuarioInternoDto>>();

            var usuariosAtivos =
                await mediator.Send(
                    new VerificarSeUsuarioPossuiCargoAtivoNoEolQuery(usuariosInscricao.Select(x => x.Login).ToArray()));

            var inscricoesParaCancelar = usuariosInscricao.Where(u => !usuariosAtivos.Contains(u.Login));

            foreach (var inscricao in inscricoesParaCancelar)
            {
                await mediator.Send(new CancelarInscricaoCommand(inscricao.InscricaoId));
            }

            return true;
        }
    }
}