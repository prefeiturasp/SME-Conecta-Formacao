namespace SME.ConectaFormacao.Dominio.Comum
{
    public class Resultado<T>
    {
        public T? Dados { get; }
        public bool Sucesso { get; }
        public TipoFalha TipoFalha { get; }
        public List<string> MensagensErro { get; }

        protected Resultado(bool sucesso, T? dados, TipoFalha tipo, List<string> mensagensErro)
        {
            Sucesso = sucesso;
            Dados = dados;
            TipoFalha = tipo;
            MensagensErro = mensagensErro;
        }

        public static Resultado<T> DeSucesso(T dados)
        => new(true, dados, TipoFalha.Nenhuma, []);

        public static Resultado<T> DeFalha(TipoFalha tipo, string mensagem)
            => new(false, default, tipo, [mensagem]);

        public static Resultado<T> DeFalha(TipoFalha tipo, List<string> mensagens)
            => new(false, default, tipo, mensagens);

        // Permite fazer: return objeto; ao invés de return Resultado<Dto>.DeSucesso(objeto);
        public static implicit operator Resultado<T>(T dados) => DeSucesso(dados);

        // Conversão Implícita para ERRO (Erro -> Resultado<T>)
        public static implicit operator Resultado<T>(Erro erro)
            => new(false, default, erro.Tipo, [.. erro.Mensagens]);
    }

    public class Resultado : Resultado<bool>
    {
        private Resultado(bool sucesso, TipoFalha tipo, List<string> mensagensErro)
        : base(sucesso, true, tipo, mensagensErro)
        {
        }
        public static Resultado DeSucesso()
            => new(true, TipoFalha.Nenhuma, []);
        public static new Resultado DeFalha(TipoFalha tipo, string mensagem)
            => new(false, tipo, [mensagem]);

        public static implicit operator Resultado(Erro erro)
        => new(false, erro.Tipo, [.. erro.Mensagens]);
    }
}