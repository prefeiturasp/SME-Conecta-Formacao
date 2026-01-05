using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterDadosPaginadosComFiltros : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterDadosPaginadosComFiltros
    {
        public CasoDeUsoObterDadosPaginadosComFiltros(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDto<DadosListagemFormacaoComTurmaDTO>> Executar(FiltroListagemInscricaoComTurmaDTO filtro)
        {
            var areaPromotoraUsuarioLogado = await mediator.Send(ObterAreaPromotoraUsuarioLogadoQuery.Instancia());
            return await mediator.Send(new ObterDadosPaginadosComFiltrosQuery(NumeroPagina, NumeroRegistros, filtro.CodigoFormacao, filtro.NomeFormacao, areaPromotoraUsuarioLogado?.Id, filtro.NumeroHomologacao));
        }
    }
}