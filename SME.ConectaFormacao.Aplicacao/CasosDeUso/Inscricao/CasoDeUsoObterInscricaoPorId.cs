using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterInscricaoPorId : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterInscricaoPorId
    {
        public CasoDeUsoObterInscricaoPorId(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<DadosListagemInscricaoDTO>> Executar(long inscricaoId, FiltroListagemInscricaoDTO filtroListagemInscricaoDTO)
        {
            var dadosInscricao = await mediator.Send(new ObterInscricaoPorIdQuery(inscricaoId, filtroListagemInscricaoDTO, NumeroPagina, NumeroRegistros));

            if(!dadosInscricao.Items.Any())
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            return dadosInscricao;
        }
    }
}