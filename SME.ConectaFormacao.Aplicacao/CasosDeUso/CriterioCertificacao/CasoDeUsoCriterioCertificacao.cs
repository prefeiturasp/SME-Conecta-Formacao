using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.CriterioCertificacao.ObterCriterioCertificacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.CriterioCertificacao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CriterioCertificacao
{
    public class CasoDeUsoCriterioCertificacao: CasoDeUsoAbstrato, ICasoDeUsoCriterioCertificacao
    {
        public CasoDeUsoCriterioCertificacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterCriterioCertificacaoQuery.Instance);
        }
    }
}