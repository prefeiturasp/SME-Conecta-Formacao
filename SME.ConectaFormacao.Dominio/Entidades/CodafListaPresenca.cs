using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafListaPresenca : EntidadeBaseAuditavel
    {
        public long PropostaId { get; private set; }
        public long PropostaTurmaId { get; private set; }
        public DateTime? DataPublicacao { get; private set; }
        public DateTime? DataPublicacaoDom { get; private set; }
        public short? NumeroComunicado { get; private set; }
        public short? PaginaComunicadoDom { get; private set; }
        public int? CodigoCursoEol { get; private set; }
        public int? CodigoNivel { get; private set; }
        public string? Observacao { get; private set; }
        public StatusCodafListaPresenca Status { get; private set; }

        public Proposta Proposta { get; set; } = null!;
        public PropostaTurma PropostaTurma { get; set; } = null!;
        public ICollection<CodafComentario> CodafComentarios { get; set; } = [];
        public ICollection<CodafInscricaoListaPresenca> CodafInscricoes { get; set; } = [];
        public ICollection<CodafRetificacaoListaPresenca> CodafRetificacoes { get; set; } = [];
        public ICollection<CodafAnexo>? CodafAnexos { get; set; }

        protected CodafListaPresenca() { }

        public CodafListaPresenca(long propostaId, long propostaTurmaId, DateTime? dataPublicacao, DateTime? dataPublicacaoDom, short? numeroComunicado, short? paginaComunicadoDom, int? codigoCursoEol, int? codigoNivel, string? observacao, Guid? idPerfilUsuario)
        {
            PropostaId = propostaId;
            PropostaTurmaId = propostaTurmaId;
            DataPublicacao = dataPublicacao;
            DataPublicacaoDom = dataPublicacaoDom;
            NumeroComunicado = numeroComunicado;
            PaginaComunicadoDom = paginaComunicadoDom;
            Observacao = observacao;
            if (idPerfilUsuario == Perfis.ADMIN_DF)
            {
                CodigoNivel = codigoNivel;
                CodigoCursoEol = codigoCursoEol;
            }
        }

        public void AtualizarInformacoes(DateTime? dataPublicacao, DateTime? dataPublicacaoDom, short? numeroComunicado, short? paginaComunicadoDom, int? codigoCursoEol, int? codigoNivel, string? observacao, Guid? idPerfilUsuario)
        {
            DataPublicacao = dataPublicacao;
            DataPublicacaoDom = dataPublicacaoDom;
            NumeroComunicado = numeroComunicado;
            PaginaComunicadoDom = paginaComunicadoDom;
            Observacao = observacao;
            if (idPerfilUsuario == Perfis.ADMIN_DF)
            {
                CodigoNivel = codigoNivel;
                CodigoCursoEol = codigoCursoEol;
            }
        }

        public void Iniciar()
        {
            Status = StatusCodafListaPresenca.Iniciado;
        }
    }
}