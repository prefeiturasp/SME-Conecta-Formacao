using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterDadosPaginadosComFiltros : CasoDeUsoAbstratoPaginado,ICasoDeUsoObterDadosPaginadosComFiltros
    {
        public CasoDeUsoObterDadosPaginadosComFiltros(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<IEnumerable<DadosListagemFormacaoComTurma>> Executar(FiltroListagemInscricaoComTurmaDTO filtro)
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());
            return await mediator.Send(new ObterDadosPaginadosComFiltrosQuery(usuarioLogado.Id,NumeroPagina, NumeroRegistros,filtro.CodigoFormacao,filtro.NomeFormacao));
        }
    }
}