using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoSalvarInscricao : CasoDeUsoAbstrato, ICasoDeUsoSalvarInscricao
    {
        public CasoDeUsoSalvarInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(InscricaoDTO inscricaoDTO)
        {
            return await mediator.Send(new SalvarInscricaoCommand(inscricaoDTO));
        }
    }
}
