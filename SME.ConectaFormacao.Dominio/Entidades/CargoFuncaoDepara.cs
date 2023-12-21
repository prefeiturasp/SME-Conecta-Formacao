namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CargoFuncaoDeparaEol : EntidadeBase
    {
        public long CargoFuncaoId { get; set; }
        public long? CodigoCargoEol { get; set; }
        public long? CodigoFuncaoEol { get; set; }
    }
}
