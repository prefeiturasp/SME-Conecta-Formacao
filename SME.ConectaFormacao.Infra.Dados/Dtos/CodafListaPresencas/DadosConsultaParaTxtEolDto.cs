namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas
{
    public class DadosConsultaParaTxtEolDto
    {
        public required string RegistroFuncional { get; set; }
        public int CodigoCursoEol { get; set; }
        public int CodigoNivel { get; set; }
        public DateTime? DataFimCurso { get; set; }
        public long NumeroHomologacao { get; set; }
        public int? HorasTotais { get; set; }
        public string? CargaHorariaTotalOutra { get; set; }
        public required string NomeTurma { get; set; }
    }
}
