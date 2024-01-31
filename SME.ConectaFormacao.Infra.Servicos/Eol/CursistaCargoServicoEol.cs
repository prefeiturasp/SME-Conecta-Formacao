namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class CursistaCargoServicoEol
    {
        public long RF { get; set; }
        public string Cpf { get; set; }
        public long? CdCargoBase { get; set; }
        public string CargoBase { get; set; }
        public string CdDreCargoBase { get; set; }
        public string CdUeCargoBase { get; set; }
        public string UeCargoBase { get; set; }
        public long? CdCargoSobreposto { get; set; }
        public string CargoSobreposto { get; set; }
        public string CdDreCargoSobreposto { get; set; }
        public string CdUeCargoSobreposto { get; set; }
        public string UeCargoSobreposto { get; set; }
        public long? CdFuncaoAtividade { get; set; }
        public string FuncaoAtividade { get; set; }
        public string CdDreFuncaoAtividade { get; set; }
        public string CdUeFuncaoAtividade { get; set; }
        public string UeFuncaoAtividade { get; set; }
    }
}
