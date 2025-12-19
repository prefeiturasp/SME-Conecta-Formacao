namespace SME.ConectaFormacao.Dominio.Comum
{
    public readonly struct Erro(TipoFalha tipo, string mensagem)
    {
        public TipoFalha Tipo { get; } = tipo;
        public string Mensagem { get; } = mensagem;

        // Fábricas estáticas para sintaxe fluida (Sugar Syntax)
        public static Erro NaoEncontrado(string mensagem = "Registro não encontrado.")
            => new(TipoFalha.NaoEncontrado, mensagem);

        public static Erro Validacao(string mensagem)
            => new(TipoFalha.Validacao, mensagem);

        public static Erro Negocio(string mensagem)
            => new(TipoFalha.RegraDeNegocio, mensagem);
    }
}