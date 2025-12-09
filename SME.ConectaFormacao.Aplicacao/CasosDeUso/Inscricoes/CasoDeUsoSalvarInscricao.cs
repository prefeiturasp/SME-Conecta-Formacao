using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
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
