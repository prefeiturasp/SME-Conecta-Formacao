namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaFuncaoEspecifica : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public long CargoFuncaoId { get; set; }
    }
}
