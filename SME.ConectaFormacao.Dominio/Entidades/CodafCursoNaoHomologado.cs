using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafCursoNaoHomologado : EntidadeBaseAuditavel
    {
        public long PropostaId { get; private set; }
        public long PropostaTurmaId { get; private set; }
        public string? Observacao { get; private set; }
        public StatusCodafCursoNaoHomologado Status { get; private set; }
        public bool DeclaracaoEmitida => CodafDeclaracoes is not null && CodafDeclaracoes.Count > 0;

        public Proposta Proposta { get; set; } = null!;
        public PropostaTurma PropostaTurma { get; set; } = null!;

        public CodafCursoNaoHomologado() { }

        public CodafCursoNaoHomologado(long propostaId, long propostaTurmaId, string? observacao)
        {
            PropostaId = propostaId;
            PropostaTurmaId = propostaTurmaId;
            Status = StatusCodafCursoNaoHomologado.Iniciado;
            Observacao = observacao;
        }

        public void AtualizarInformacoes(string? observacao)
        {
            Observacao = observacao;
        }
        public void Finalizar()
        {
            if (Status == StatusCodafCursoNaoHomologado.Aguardando)
                Status = StatusCodafCursoNaoHomologado.Finalizado;
        }
        public bool EstaFinalizado()
           => Status == StatusCodafCursoNaoHomologado.Finalizado;

        public void DefinirStatus()
        {
            if (Status == StatusCodafCursoNaoHomologado.Finalizado)
                return;

            if (CodafInscricoes is not null && CodafInscricoes.Count != 0 &&
                CodafAnexos is not null && CodafAnexos.Count != 0)
            {
                Status = StatusCodafCursoNaoHomologado.Aguardando;
            }
        }
        public ICollection<CodafCursoNaoHomologadoInscricao> CodafInscricoes { get; set; } = [];
        public ICollection<CodafCursoNaoHomologadoAnexo>? CodafAnexos { get; set; }
        public ICollection<CodafDeclaracao>? CodafDeclaracoes { get; set; }
    }
}