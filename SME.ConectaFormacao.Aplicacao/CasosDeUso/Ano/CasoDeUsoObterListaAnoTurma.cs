using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Ano;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ano;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Ano
{
    public class CasoDeUsoObterListaAnoTurma : CasoDeUsoAbstrato, ICasoDeUsoObterListaAnoTurma
    {
        public CasoDeUsoObterListaAnoTurma(IMediator mediator) : base(mediator)
        { }

        public async Task<IEnumerable<RetornoListagemTodosDTO>> Executar(FiltroAnoTurmaDTO filtroAnoTurmaDTO)
        {
            return await mediator.Send(new ObterAnosPorModalidadeAnoLetivoQuery(filtroAnoTurmaDTO.Modalidade, filtroAnoTurmaDTO.AnoLetivo, filtroAnoTurmaDTO.ExibirOpcaoTodos));
        }
    }
}
