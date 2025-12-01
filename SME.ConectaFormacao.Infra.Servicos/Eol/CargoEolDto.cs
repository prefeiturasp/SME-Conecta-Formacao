namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class CargoEolDto
    {
        public required int CdCargo { get; set; }
        public required string CdRegistroFuncional { get; set; }
        public required string CodigoUe { get; set; }
        public required bool Sobreposto { get; set; }
        public required string CodigoDre { get; set; }

        public string ObterChaveNegocio()
        {
            return $"{CdRegistroFuncional}-{CdCargo}-{CodigoUe}-{Sobreposto}";
        }
    }
}