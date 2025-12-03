using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao
{
    public class CasoDeUsoObterListagemFormacaoPaginada(
        IMediator mediator, IContextoAplicacao contextoAplicacao, IRepositorioProposta repositorioProposta) :
        CasoDeUsoAbstratoPaginado(mediator, contextoAplicacao), ICasoDeUsoObterListagemFormacaoPaginada
    {
        public async Task<PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>> Executar(FiltroListagemFormacaoDTO filtroListagemFormacaoDTO)
        {
            var propostasPaginadas = await repositorioProposta.ObterListagemFormacoesPorFiltro(new()
            {
                AreasPromotorasIds = filtroListagemFormacaoDTO.AreasPromotorasIds,
                DataFinal = filtroListagemFormacaoDTO.DataFinal,
                DataInicial = filtroListagemFormacaoDTO.DataInicial,
                EhPerfilCursista = EhPerfilCursista(),
                FormatosIds = filtroListagemFormacaoDTO.FormatosIds,
                Pagina = NumeroPagina,
                PalavrasChavesIds = filtroListagemFormacaoDTO.PalavrasChavesIds,
                TamanhoPagina = NumeroRegistros,
                PublicosAlvosIds = filtroListagemFormacaoDTO.PublicosAlvosIds,
                RfServidor = contextoAplicacao.UsuarioLogado,
                Titulo = filtroListagemFormacaoDTO.Titulo
            });

            var formacoes = Enumerable.Empty<RetornoListagemFormacaoDTO>();
            if (propostasPaginadas.Itens.Any())
                formacoes = await mediator.Send(new ObterPropostasPorIdsQuery(propostasPaginadas.Itens));

            return new PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>(formacoes, propostasPaginadas.TotalRegistros, NumeroRegistros);
        }

        private bool EhPerfilCursista()
        {
            var perfilCursista = PerfilAutomatico.PERIL_CURSISTA_CODIGO;
            return contextoAplicacao.IdPerfilUsuario == perfilCursista;
        }
    }
}
