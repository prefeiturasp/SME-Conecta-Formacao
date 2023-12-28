using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterInscricaoPaginada : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterInscricaoPaginada
    {
        public CasoDeUsoObterInscricaoPaginada(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<InscricaoPaginadaDTO>> Executar()
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());

            return await mediator.Send(new ObterInscricaoPaginadaPorUsuarioIdQuery(usuarioLogado.Id, NumeroPagina, NumeroRegistros));
        }
    }
}
