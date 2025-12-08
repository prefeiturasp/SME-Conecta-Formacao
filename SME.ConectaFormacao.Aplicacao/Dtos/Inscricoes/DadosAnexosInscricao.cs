namespace SME.ConectaFormacao.Aplicacao
{
    public class DadosAnexosInscricao
    {
        public DadosAnexosInscricao(string nome, Guid? codigo)
        {
            Nome = nome;
            Codigo = codigo;
        }

        public string Nome { get; set; }
        public Guid? Codigo { get; set; }
    }
}