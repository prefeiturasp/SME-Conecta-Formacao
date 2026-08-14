using System.ComponentModel;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum StatusDeclaracaoTurma
    {
        [Description("Sem declarações")]
        SemDeclaracao = 0,

        [Description("Não emitidas")]
        NaoEmitido = 1,

        [Description("Emitir declarações")]
        DisponivelParaEmissao = 2,

        [Description("Declarações em processamento")]
        EmProcessamento = 3,

        [Description("Declarações emitidas")]
        Emitido = 4
    }
}