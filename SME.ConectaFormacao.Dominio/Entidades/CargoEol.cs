namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CargoEol
    {
        public Guid Id { get; private set; }
        public int CdCargoBaseServidor { get; set; }
        public int CodigoCargo { get; private set; }
        public string CodigoRegistroFuncional { get; private set; }
        public string CodigoUe { get; private set; }
        public bool Sobreposto { get; private set; }
        public string CodigoDre { get; private set; }
        public DateTime DataAtualizacao { get; private set; }
        public DateOnly? DataPosse { get; set; }
        public string? NomeCargo { get; set; }
        public int? TipoVinculo { get; set; }

        public CargoEol(int cdCargoBaseServidor, int cdCargo, string cdRegistroFuncional, string codigoUe, bool sobreposto, string codigoDre)
        {
            Id = Guid.NewGuid();
            CdCargoBaseServidor = cdCargoBaseServidor;
            CodigoCargo = cdCargo;
            CodigoRegistroFuncional = cdRegistroFuncional;
            CodigoUe = codigoUe;
            Sobreposto = sobreposto;
            CodigoDre = codigoDre;
            DataAtualizacao = DateTime.UtcNow;
        }

        protected CargoEol()  // EF Core
        {
            CodigoRegistroFuncional = null!;
            CodigoUe = null!;
            CodigoDre = null!;
        }

        public string ObterChaveNegocio()
        {
            return $"{CodigoRegistroFuncional}-{CodigoCargo}-{CodigoUe}-{Sobreposto}";
        }
    }
}