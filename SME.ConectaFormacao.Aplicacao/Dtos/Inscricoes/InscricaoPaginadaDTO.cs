namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class InscricaoPaginadaDTO
    {
        public long Id { get; set; }
        public long CodigoFormacao { get; set; } //proxima
        public string NomeFormacao { get; set; } //proxima
        public string NomeTurma { get; set; } //proxima
        public string Datas { get; set; }
        public string CargoFuncaoCodigo { get; set; }
        public string CargoFuncao { get; set; }
        public int? TipoVinculo { get; set; }
        public string Situacao { get; set; } //proxima
        public bool PodeCancelar { get; set; } = true;
        public string Origem { get; set; }
        public bool IntegrarNoSga { get; set; }
        public bool Iniciado { get; set; }
        public string DataInscricao { get; set; } = string.Empty; //proxima
    }
}
