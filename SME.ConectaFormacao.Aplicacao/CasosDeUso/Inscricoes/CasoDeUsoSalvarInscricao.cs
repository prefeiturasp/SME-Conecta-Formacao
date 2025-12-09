using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoSalvarInscricao(IMediator mediator) : CasoDeUsoAbstrato(mediator), ICasoDeUsoSalvarInscricao
    {
        public async Task<RetornoDTO> Executar(InscricaoDto inscricaoDto)
        {
            return await mediator.Send(new SalvarInscricaoCommand(inscricaoDto));
        }
    }
}
