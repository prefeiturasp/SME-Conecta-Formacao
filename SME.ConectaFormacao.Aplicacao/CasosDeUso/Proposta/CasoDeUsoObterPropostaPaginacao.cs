using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaPaginacao : CasoDeUsoAbstrato, ICasoDeUsoObterPropostaPaginacao
    {
        public CasoDeUsoObterPropostaPaginacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<PaginacaoResultadoDTO<PropostaPaginadaDTO>> Executar(PropostaFiltrosDTO propostaFiltrosDTO)
        {
            var (grupoUsuarioLogadoId,dresCodigoDoUsuarioLogado) = await mediator.Send(ObterGrupoUsuarioEDresUsuarioLogadoQuery.Instancia());
            propostaFiltrosDTO.GrupoId = grupoUsuarioLogadoId;
            propostaFiltrosDTO.DresCodigo = dresCodigoDoUsuarioLogado;
            
            int numeroPagina = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroPagina")), out numeroPagina) ? numeroPagina : 1;
            int numeroRegistros = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroRegistros")), out numeroRegistros) ? numeroRegistros : 10;

            return await mediator.Send(new ObterPropostaPaginadaQuery(propostaFiltrosDTO, numeroPagina, numeroRegistros));
        }
    }
}
