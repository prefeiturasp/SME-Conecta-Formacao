
namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaParecerista : EntidadeBaseAuditavel
    {

        public long PropostaId { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? NomeParecerista { get; set; }
    }
}
