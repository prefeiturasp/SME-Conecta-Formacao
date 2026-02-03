namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class DadosArquivoCodafEolDto
    {
        public string RegistroFuncional { get; set; } = string.Empty;
        public string CodigoCursoEol { get; set; } = string.Empty;
        public string CodigoNivel { get; set; } = string.Empty;
        public string DataFimCurso { get; set; } = string.Empty;
        public string NumeroHomologacao { get; set; } = string.Empty;
        public string CargaHoraria { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{RegistroFuncional}|{CodigoCursoEol}|{DataFimCurso}|{CodigoNivel}|{NumeroHomologacao}|{CargaHoraria}";
        }
    }
}
