using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class InscricaoCursistaImportacaoDto
    {
        public required string Turma { get; set; }
        public required string ColaboradorRede { get; set; }
        public required string RegistroFuncional { get; set; }
        public required string Cpf { get; set; }
        public required string Nome { get; set; }
        public string? Vinculo { get; set; }

        public Inscricao Inscricao { get; set; } = null!;
    }
}
