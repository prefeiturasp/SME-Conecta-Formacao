using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoAlterarVinculoInscricao : CasoDeUsoAbstrato, ICasoDeUsoAlterarVinculoInscricao
    {
        public CasoDeUsoAlterarVinculoInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id, AlterarCargoFuncaoVinculoIncricaoDTO alterarCargoFuncaoVinculoIncricao)
        {
            return await mediator.Send(new AlterarCargoFuncaoVinculoInscricaoCommand(id, alterarCargoFuncaoVinculoIncricao));
        }
    }
}