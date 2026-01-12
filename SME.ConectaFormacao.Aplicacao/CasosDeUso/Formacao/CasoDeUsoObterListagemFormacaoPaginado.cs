using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao
{
    public class CasoDeUsoObterListagemFormacaoPaginada(
        IMediator mediator, IContextoAplicacao contextoAplicacao, IRepositorioProposta repositorioProposta) :
        CasoDeUsoAbstratoPaginado(mediator, contextoAplicacao), ICasoDeUsoObterListagemFormacaoPaginada
    {
        public async Task<PaginacaoResultadoDto<RetornoListagemFormacaoDTO>> Executar(FiltroListagemFormacaoDTO filtroListagemFormacaoDTO)
        {
            var propostasPaginadas = await repositorioProposta.ObterListagemFormacoesPorFiltro(new()
            {
                AreasPromotorasIds = filtroListagemFormacaoDTO.AreasPromotorasIds,
                DataFinal = filtroListagemFormacaoDTO.DataFinal,
                DataInicial = filtroListagemFormacaoDTO.DataInicial,
                FiltrarPorPerfil = FiltrarPorPerfil(),
                FormatosIds = filtroListagemFormacaoDTO.FormatosIds,
                Pagina = NumeroPagina,
                PalavrasChavesIds = filtroListagemFormacaoDTO.PalavrasChavesIds,
                TamanhoPagina = NumeroRegistros,
                PublicosAlvosIds = filtroListagemFormacaoDTO.PublicosAlvosIds,
                RfServidor = contextoAplicacao.LoginUsuario,
                Titulo = filtroListagemFormacaoDTO.Titulo
            });

            var formacoes = Enumerable.Empty<RetornoListagemFormacaoDTO>();
            if (propostasPaginadas.Itens.Any())
                formacoes = await mediator.Send(new ObterPropostasPorIdsQuery(propostasPaginadas.Itens));

            return new PaginacaoResultadoDto<RetornoListagemFormacaoDTO>(formacoes, propostasPaginadas.TotalRegistros, NumeroRegistros);
        }

        private bool FiltrarPorPerfil()
        {
            var perfilCursista = PerfilAutomatico.PERIL_CURSISTA_CODIGO;
            if (contextoAplicacao.IdPerfilUsuario != perfilCursista) return false;
            if (string.IsNullOrWhiteSpace(contextoAplicacao.LoginUsuario)) return false;
            if (UtilValidacoes.CpfEhValido(contextoAplicacao.LoginUsuario)) return false;
            return true;
        }
    }
}
