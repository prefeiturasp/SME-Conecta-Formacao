using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterDadosPaginadosComFiltros : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterDadosPaginadosComFiltros
    {
        public CasoDeUsoObterDadosPaginadosComFiltros(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>> Executar(FiltroListagemInscricaoComTurmaDTO filtro)
        {
            return await mediator.Send(new ObterDadosPaginadosComFiltrosQuery(NumeroPagina, NumeroRegistros, filtro.CodigoFormacao, filtro.NomeFormacao));
        }
    }
}