using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterInscricaoFinalizadaPaginada : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterInscricaoFinalizadaPaginada
    {
        public CasoDeUsoObterInscricaoFinalizadaPaginada(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDto<InscricaoPaginadaDTO>> Executar()
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());

            return await mediator.Send(new ObterInscricaoPaginadaPorUsuarioIdQuery(usuarioLogado.Id, NumeroPagina, NumeroRegistros));
        }
    }
}
