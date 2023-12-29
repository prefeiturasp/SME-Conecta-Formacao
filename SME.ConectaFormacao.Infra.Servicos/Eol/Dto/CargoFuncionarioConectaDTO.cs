namespace SME.ConectaFormacao.Infra.Servicos.Eol.Dto
{
    public class CargoFuncionarioConectaDTO
    {
        public long RF { get; set; }
        public string Cpf { get; set; }
        public long? CdCargoBase { get; set; }
        public string CargoBase { get; set; }
        public long? CdDreCargoBase { get; set; }
        public long? CdUeCargoBase { get; set; }
        public string UeCargoBase { get; set; }
        public long? CdCargoSobreposto { get; set; }
        public string CargoSobreposto { get; set; }
        public long? CdDreCargoSobreposto { get; set; }
        public long? CdUeCargoSobreposto { get; set; }
        public string UeCargoSobreposto { get; set; }
        public long? CdFuncaoAtividade { get; set; }
        public string FuncaoAtividade { get; set; }
        public long? CdDreFuncaoAtividade { get; set; }
        public long? CdUeFuncaoAtividade { get; set; }
        public string UeFuncaoAtividade { get; set; }
    }
}
