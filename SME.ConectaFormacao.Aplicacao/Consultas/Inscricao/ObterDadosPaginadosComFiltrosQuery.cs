using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDadosPaginadosComFiltrosQuery :IRequest<PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>>
    {
        public ObterDadosPaginadosComFiltrosQuery(int numeroPagina, int numeroRegistros, long? codigoFormacao, string? nomeFormacao)
        {
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
            CodigoFormacao = codigoFormacao;
            NomeFormacao = nomeFormacao;
        }

        public int NumeroPagina { get; set;}
        public int NumeroRegistros { get; set;}
        public long? CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
    }
}