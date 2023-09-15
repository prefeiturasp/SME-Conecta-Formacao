using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPaginadaQuery : IRequest<PaginacaoResultadoDTO<PropostaPaginadaDTO>>
    {
        public ObterPropostaPaginadaQuery(PropostaFiltrosDTO propostaFiltrosDTO, int numeroPagina, int numeroRegistros)
        {
            PropostaFiltrosDTO = propostaFiltrosDTO;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public PropostaFiltrosDTO PropostaFiltrosDTO { get; }
        public int NumeroPagina { get; }
        public int NumeroRegistros { get; }
    }
}
