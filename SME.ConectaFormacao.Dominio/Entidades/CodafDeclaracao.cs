using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafDeclaracao : EntidadeBaseAuditavel
    {
        public long CodigoDeclaracao { get; init; }
        public long? CodafCursoNaoHomologadoInscricaoId { get; private set; }
        public CodafCursoNaoHomologadoInscricao? CodafCursoNaoHomologadoInscricao { get; set; }
        public long? PropostaRegenteTurmaId { get; private set; }
        public PropostaRegenteTurma? PropostaRegenteTurma { get; set; }
        public TipoParticipacaoCodaf TipoParticipacao { get; private set; }
        public DateTime DataEmissao { get; private set; }
        public string HtmlContentSnapshot { get; private set; }
        public string? MetadadosJson { get; private set; }
        public StatusProcessamentoDeclaracaoCodaf StatusProcessamento { get; set; }
        public string? ChaveObjetoArmazenamento { get; set; }
        public string? ErroProcessamento { get; set; }
        public long? CodafCursoNaoHomologadoId { get; set; }
        public CodafCursoNaoHomologado? CodafCursoNaoHomologado { get; set; }

        public CodafDeclaracao(long codafCursoNaoHomologadoInscricaoId, TipoParticipacaoCodaf tipoParticipacao, long? referenciaId, string htmlContentSnapshot, string? metadadosJson)
        {
            if (tipoParticipacao == TipoParticipacaoCodaf.Cursista)
                CodafCursoNaoHomologadoInscricaoId = referenciaId;
            else if (tipoParticipacao == TipoParticipacaoCodaf.Regente)
                PropostaRegenteTurmaId = referenciaId;

            TipoParticipacao = tipoParticipacao;
            HtmlContentSnapshot = htmlContentSnapshot;
            MetadadosJson = metadadosJson;
            DataEmissao = DateTime.UtcNow;
            StatusProcessamento = StatusProcessamentoDeclaracaoCodaf.Pendente;
        }

        protected CodafDeclaracao()
        {
            HtmlContentSnapshot = null!;
        }    
    }
}
