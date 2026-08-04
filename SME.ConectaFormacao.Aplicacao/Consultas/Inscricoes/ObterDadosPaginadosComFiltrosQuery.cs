using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDadosPaginadosComFiltrosQuery : IRequest<PaginacaoResultadoDto<DadosListagemFormacaoComTurmaDTO>>
    {
        public ObterDadosPaginadosComFiltrosQuery(int numeroPagina, int numeroRegistros, long? codigoFormacao, string? nomeFormacao, long? areaPromotoraIdUsuarioLogado, long? numeroHomologacao, bool? apenasSemCodaf)
        {
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
            CodigoFormacao = codigoFormacao;
            NomeFormacao = nomeFormacao;
            AreaPromotoraIdUsuarioLogado = areaPromotoraIdUsuarioLogado;
            NumeroHomologacao = numeroHomologacao;
            ApenasSemCodaf = apenasSemCodaf;
        }

        public int NumeroPagina { get; set; }
        public int NumeroRegistros { get; set; }
        public long? CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long? AreaPromotoraIdUsuarioLogado { get; set; }
        public long? NumeroHomologacao { get; set; }
        public bool? ApenasSemCodaf { get; set; }
    }
}