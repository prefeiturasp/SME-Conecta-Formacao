namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoPaginadaDTO
    {
        public long Id { get; set; }
        public long CodigoFormacao { get; set; }
        public string NomeFormacao { get; set; }
        public string NomeTurma { get; set; }
        public string Datas { get; set; }
        public string CargoFuncao { get; set; }
        public string Situacao { get; set; }
        public bool PodeCancelar { get; set; }
    }
}
