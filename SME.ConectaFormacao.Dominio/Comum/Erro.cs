namespace SME.ConectaFormacao.Dominio.Comum
{
    public readonly struct Erro(TipoFalha tipo, IEnumerable<string> mensagens)
    {
        public TipoFalha Tipo { get; init; } = tipo;
        public string[] Mensagens { get; } = [.. mensagens];

        public Erro(TipoFalha tipo, string mensagem) : this(tipo, [mensagem]) { }

        // Fábricas estáticas para sintaxe fluida (Sugar Syntax)
        public static Erro NaoEncontrado(string mensagem = "Registro não encontrado.")
            => new(TipoFalha.NaoEncontrado, mensagem);
        public static Erro Validacao(IEnumerable<string> mensagens)
            => new(TipoFalha.Validacao, mensagens);

        public static Erro Validacao(string mensagem)
            => new(TipoFalha.Validacao, mensagem);

        public static Erro Negocio(string mensagem)
            => new(TipoFalha.RegraDeNegocio, mensagem);
    }
}