using SME.ConectaFormacao.Dominio.Constantes;
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
        public CodafSuplementar(long codafListaPresencaId, StatusCodafSuplementar status)
        {
            CodafListaPresencaId = codafListaPresencaId;
            Status = status;
        }

        public CodafSuplementar(long codafListaPresencaId, DadosPublicacaoLista dadosPublicacao, Guid? idPerfilUsuario)
        {
            CodafListaPresencaId = codafListaPresencaId;
            AtribuirDadosPublicacao(dadosPublicacao, idPerfilUsuario);
        }

        public void AtualizarInformacoes(DadosPublicacaoLista dadosPublicacao, Guid? idPerfilUsuario)
        {
            AtribuirDadosPublicacao(dadosPublicacao, idPerfilUsuario);
        }
        private void AtribuirDadosPublicacao(DadosPublicacaoLista dados, Guid? idPerfilUsuario)
        {
            DataPublicacao = dados.DataPublicacao;
            DataPublicacaoDom = dados.DataPublicacaoDom;
            NumeroComunicado = dados.NumeroComunicado;
            PaginaComunicadoDom = dados.PaginaComunicadoDom;
            Observacao = dados.Observacao;

            if (idPerfilUsuario == Perfis.ADMIN_DF)
            {
                CodigoNivel = dados.CodigoNivel;
                CodigoCursoEol = dados.CodigoCursoEol;
            }
        }

        public void Iniciar()
        {
            Status = StatusCodafSuplementar.Iniciado;
        }
    }
}