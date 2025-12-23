using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafListaPresenca : EntidadeBaseAuditavel
    {
        public long PropostaId { get; private set; }
        public long PropostaTurmaId { get; private set; }
        public DateOnly? DataPublicacao { get; private set; }
        public DateOnly? DataPublicacaoDom { get; private set; }
        public short? NumeroComunicado { get; private set; }
        public short? PaginaComunicadoDom { get; private set; }
        public int? CodigoCursoEol { get; private set; }
        public int? CodigoNivel { get; private set; }
        public string? Observacao { get; private set; }
        public StatusCodafListaPresenca Status { get; private set; }

        public Proposta Proposta { get; set; } = null!;
        public PropostaTurma PropostaTurma { get; set; } = null!;
        public IEnumerable<CodafComentario> CodafComentarios { get; set; } = [];
        public IEnumerable<CodafInscricaoListaPresenca> CodafInscricoes { get; set; } = [];
        public IEnumerable<CodafRetificacao> CodafRetificacoes { get; set; } = [];

        protected CodafListaPresenca() { }

        public CodafListaPresenca(long propostaId, long propostaTurmaId, DateOnly? dataPublicacao, DateOnly? dataPublicacaoDom, short? numeroComunicado, short? paginaComunicadoDom, int? codigoCursoEol, int? codigoNivel, string? observacao, Guid? idPerfilUsuario)
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

        public void AtualizarInformacoes(DateOnly? dataPublicacao, DateOnly? dataPublicacaoDom, short? numeroComunicado, short? paginaComunicadoDom, int? codigoCursoEol, int? codigoNivel, string? observacao, Guid? idPerfilUsuario)
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