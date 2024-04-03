namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class CursistaServicoEol
    {
        public string Rf { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }

        public string CargoCodigo { get; set; }
        public string Cargo { get; set; }
        public string CargoDreCodigo { get; set; }
        public string CargoUeCodigo { get; set; }

        public string FuncaoCodigo { get; set; }
        public string Funcao { get; set; }
        public string FuncaoDreCodigo { get; set; }
        public string FuncaoUeCodigo { get; set; }
        public bool Associado { get; set; }
        public int? TipoVinculo { get; set; }
    }
}
