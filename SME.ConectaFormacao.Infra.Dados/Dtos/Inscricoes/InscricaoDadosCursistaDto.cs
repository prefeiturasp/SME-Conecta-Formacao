namespace SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes
{
    public class InscricaoDadosCursistaDto
    {
        public long Id { get; set; }
        public string Login { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Nome { get; set; } = null!;
    }
}
