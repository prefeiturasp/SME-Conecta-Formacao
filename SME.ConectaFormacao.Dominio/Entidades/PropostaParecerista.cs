
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaParecerista : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? NomeParecerista { get; set; }
        public SituacaoParecerista Situacao { get; set; }
        public long UsuarioPareceristaId { get; set; }
        public string Justificativa { get; set; }
    }
}
