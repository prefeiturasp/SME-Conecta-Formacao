namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CargoEol(int cdCargo, string cdRegistroFuncional, string codigoUe, bool sobreposto, string codigoDre)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public int CdCargo { get; private set; } = cdCargo;
        public string CdRegistroFuncional { get; private set; } = cdRegistroFuncional;
        public string CodigoUe { get; private set; } = codigoUe;
        public bool Sobreposto { get; private set; } = sobreposto;
        public string CodigoDre { get; private set; } = codigoDre;
        public DateTime DataAtualizacao { get; private set; } = DateTime.UtcNow;

        protected CargoEol() : this(0, string.Empty, string.Empty, false, string.Empty) { } // EF Core

        public string ObterChaveNegocio()
        {
            return $"{CdRegistroFuncional}-{CdCargo}-{CodigoUe}-{Sobreposto}";
        }
    }
}