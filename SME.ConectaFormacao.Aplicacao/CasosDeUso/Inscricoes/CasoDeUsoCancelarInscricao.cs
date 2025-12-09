using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoCancelarInscricao : CasoDeUsoAbstrato, ICasoDeUsoCancelarInscricao
    {
        public CasoDeUsoCancelarInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id)
        {
            return await mediator.Send(new CancelarInscricaoCommand(id, null));
        }
    }
}
