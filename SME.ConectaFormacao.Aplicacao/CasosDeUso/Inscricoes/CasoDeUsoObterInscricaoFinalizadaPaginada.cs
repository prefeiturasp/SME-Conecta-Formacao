using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterInscricaoFinalizadaPaginada : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterInscricaoFinalizadaPaginada
    {
        public CasoDeUsoObterInscricaoFinalizadaPaginada(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDto<InscricaoPaginadaDTO>> Executar(InscricaoFinalizadaFiltroDTO inscricaoDTO)
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());

            var filtro = MapearParaFiltroDominio(inscricaoDTO);

            return await mediator.Send(new ObterInscricaoFinalizadaPaginadaQuery(usuarioLogado.Id, NumeroPagina, NumeroRegistros, filtro));
        }

        private InscricaoFinalizadaFiltro MapearParaFiltroDominio(InscricaoFinalizadaFiltroDTO dto)
        {
            if (dto == null)
                return new InscricaoFinalizadaFiltro();

            return new InscricaoFinalizadaFiltro
            {
                NomeFormacao = dto.NomeFormacao,
                SituacaoAprovacao = dto.SituacaoAprovacao,
                SituacaoInscricao = dto.SituacaoInscricao,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal
            };
        }
    }
}
