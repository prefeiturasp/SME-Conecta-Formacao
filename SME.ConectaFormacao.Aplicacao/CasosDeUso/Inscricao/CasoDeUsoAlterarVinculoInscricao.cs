using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoAlterarVinculoInscricao : CasoDeUsoAbstrato, ICasoDeUsoAlterarVinculoInscricao
    {
        public CasoDeUsoAlterarVinculoInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id, int tipoVinculo)
        {
            return await mediator.Send(new AlterarVinculoInscricaoCommand(id, tipoVinculo));
        }
    }
}