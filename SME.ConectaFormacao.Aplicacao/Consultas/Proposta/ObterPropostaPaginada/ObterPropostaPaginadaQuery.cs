using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterPropostaPaginada
{
    public class ObterPropostaPaginadaQuery(PropostaFiltrosDTO propostaFiltrosDTO, int numeroPagina, int numeroRegistros, long? areaPromotoraIdUsuarioLogado) : 
        IRequest<PaginacaoResultadoDto<PropostaPaginadaDTO>>
    {
        public PropostaFiltrosDTO PropostaFiltrosDTO { get; } = propostaFiltrosDTO;
        public int NumeroPagina { get; } = numeroPagina;
        public int NumeroRegistros { get; } = numeroRegistros;
        public long? AreaPromotoraIdUsuarioLogado { get; } = areaPromotoraIdUsuarioLogado;
    }
}
