using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Dre;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Dre
{
    public class CasoDeUsoObterDreListaUsuarioLogado : CasoDeUsoAbstrato, ICasoDeUsoObterDreListaUsuarioLogado
    {
        public CasoDeUsoObterDreListaUsuarioLogado(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            var dres = await mediator.Send(ObterDresPorGrupoUsuarioLogadoQuery.Instancia());
            return dres.Select(d => new RetornoListagemDTO { Id = d.Id, Descricao = d.Nome });
        }
    }
}
