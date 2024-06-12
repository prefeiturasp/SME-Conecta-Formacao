using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoSortearInscricoes : CasoDeUsoAbstrato, ICasoDeUsoSortearInscricoes
    {
        public CasoDeUsoSortearInscricoes(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(long propostaTurmaId)
        {
            await mediator.Send(new SortearInscricaoCommand(propostaTurmaId));
            return RetornoDTO.RetornarSucesso(MensagemNegocio.SORTEIO_REALIZADO_COM_SUCESSO, propostaTurmaId);
        }
    }
}
