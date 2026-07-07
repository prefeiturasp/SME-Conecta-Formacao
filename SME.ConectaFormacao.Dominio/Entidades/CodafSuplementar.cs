using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafSuplementar : EntidadeBaseAuditavel
    {
        public long CodafId { get; private set; }
        public DateTime? DataPublicacao { get; private set; }
        public DateTime? DataPublicacaoDom { get; private set; }
        public short? NumeroComunicado { get; private set; }
        public short? PaginaComunicadoDom { get; private set; }
        public int? CodigoCursoEol { get; private set; }
        public int? CodigoNivel { get; private set; }
        public string? Observacao { get; private set; }
        public StatusCodafSuplementar Status { get; private set; }

        public Proposta Proposta { get; set; } = null!;
        public PropostaTurma PropostaTurma { get; set; } = null!;
        public CodafListaPresenca CodafListaPresenca { get; set; } = null!;
        public ICollection<CodafSuplementarInscricao> CodafInscricoes { get; set; } = [];
        public ICollection<CodafSuplementarRetificacao> CodafRetificacoes { get; set; } = [];
        public ICollection<CodafSuplementarAnexo>? CodafAnexos { get; set; }
        public ICollection<CodafSuplementarLogRemessaConclusao>? CodafSuplementarLogRemessasConclusao { get; set; }

        protected CodafSuplementar() { }
        public CodafSuplementar(long codafListaPresencaId)
        {
            CodafId = codafListaPresencaId;
            Status = StatusCodafSuplementar.Iniciado;
        }

        public CodafSuplementar(long codafListaPresencaId, DadosPublicacaoLista dadosPublicacao)
        {
            CodafId = codafListaPresencaId;
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

        public void DefinirStatus()
        {
            if (Status == StatusCodafSuplementar.Finalizado)
                return;

            if (CodafInscricoes is not null && CodafInscricoes.Count != 0 &&
                CodafAnexos is not null && CodafAnexos.Count != 0 &&
                DataPublicacao is not null && DataPublicacaoDom is not null &&
                NumeroComunicado is not null && PaginaComunicadoDom is not null &&
                CodigoNivel is not null && CodigoCursoEol is not null)
            {
                Status = StatusCodafSuplementar.Aguardando;
            }
        }
    }
}