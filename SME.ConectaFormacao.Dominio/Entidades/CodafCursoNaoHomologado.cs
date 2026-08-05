using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafCursoNaoHomologado : EntidadeBaseAuditavel
    {
        public long PropostaId { get; private set; }
        public long PropostaTurmaId { get; private set; }
        public string? Observacao { get; private set; }
        public StatusCodafCursoNaoHomologado Status { get; private set; }

        public Proposta Proposta { get; set; } = null!;
        public PropostaTurma PropostaTurma { get; set; } = null!;

        public CodafCursoNaoHomologado(long propostaId, long propostaTurmaId, string? observacao)
        {
            PropostaId = propostaId;
            PropostaTurmaId = propostaTurmaId;
            Status = StatusCodafCursoNaoHomologado.Iniciado;
            Observacao = observacao;
        }
        public ICollection<CodafCursoNaoHomologadoInscricao> CodafInscricoes { get; set; } = [];
        public ICollection<CodafCursoNaoHomologadoAnexo>? CodafAnexos { get; set; }
    }
}