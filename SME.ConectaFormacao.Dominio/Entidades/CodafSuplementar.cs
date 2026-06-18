using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafSuplementar : EntidadeBaseAuditavel
    {
        public long CodafListaPresencaId { get; private set; }
        public DateTime? DataPublicacao { get; private set; }
        public DateTime? DataPublicacaoDom { get; private set; }
        public short? NumeroComunicado { get; private set; }
        public short? PaginaComunicadoDom { get; private set; }
        public int? CodigoCursoEol { get; private set; }
        public int? CodigoNivel { get; private set; }
        public string? Observacao { get; private set; }
        public StatusCodafSuplementar Status { get; private set; }

        public CodafListaPresenca CodafListaPresenca { get; set; } = null!;
        public ICollection<CodafSuplementarInscricao> CodafInscricoes { get; set; } = [];
        public ICollection<CodafSuplementarRetificacao> CodafRetificacoes { get; set; } = [];
        public ICollection<CodafSuplementarAnexo>? CodafAnexos { get; set; }

        protected CodafSuplementar() { }
        public CodafSuplementar(long codafListaPresencaId)
        {
            CodafListaPresencaId = codafListaPresencaId;
            Status = StatusCodafSuplementar.Iniciado;
        }

        public CodafSuplementar(long codafListaPresencaId, DadosPublicacaoLista dadosPublicacao)
        {
            CodafListaPresencaId = codafListaPresencaId;
            Status = StatusCodafSuplementar.Iniciado;
            AtribuirDadosPublicacao(dadosPublicacao);
        }

        public void AtualizarInformacoes(DadosPublicacaoLista dadosPublicacao)
        {
            AtribuirDadosPublicacao(dadosPublicacao);
        }
        private void AtribuirDadosPublicacao(DadosPublicacaoLista dados)
        {
            DataPublicacao = dados.DataPublicacao;
            DataPublicacaoDom = dados.DataPublicacaoDom;
            NumeroComunicado = dados.NumeroComunicado;
            PaginaComunicadoDom = dados.PaginaComunicadoDom;
            Observacao = dados.Observacao;

            CodigoNivel = dados.CodigoNivel;
            CodigoCursoEol = dados.CodigoCursoEol;
        }

        public void Iniciar()
        {
            Status = StatusCodafSuplementar.Iniciado;
        }
    }
}