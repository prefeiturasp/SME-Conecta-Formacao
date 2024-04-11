namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class RegistrosPaginados<T> 
    {
        public IEnumerable<T> Registros { get; set; }
        public int TotalRegistros { get; set; }
    }
}
