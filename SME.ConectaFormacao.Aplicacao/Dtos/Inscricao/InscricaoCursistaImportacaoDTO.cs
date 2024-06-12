namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoCursistaImportacaoDTO
    {
        public string Turma { get; set; }
        public string ColaboradorRede { get; set; }
        public string RegistroFuncional { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Vinculo { get; set; }

        public Dominio.Entidades.Inscricao Inscricao { get; set; }
    }
}
