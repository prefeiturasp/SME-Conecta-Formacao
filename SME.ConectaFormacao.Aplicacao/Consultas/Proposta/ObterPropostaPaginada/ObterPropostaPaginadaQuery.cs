using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPaginadaQuery : IRequest<PaginacaoResultadoDTO<PropostaPaginadaDTO>>
    {
        public ObterPropostaPaginadaQuery(PropostaFiltrosDTO propostaFiltrosDTO, int numeroPagina, int numeroRegistros, long? areaPromotoraIdUsuarioLogado)
        {
            PropostaFiltrosDTO = propostaFiltrosDTO;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
            AreaPromotoraIdUsuarioLogado = areaPromotoraIdUsuarioLogado;
        }

        public PropostaFiltrosDTO PropostaFiltrosDTO { get; }
        public int NumeroPagina { get; }
        public int NumeroRegistros { get; }
        public long? AreaPromotoraIdUsuarioLogado { get; }
    }
}
