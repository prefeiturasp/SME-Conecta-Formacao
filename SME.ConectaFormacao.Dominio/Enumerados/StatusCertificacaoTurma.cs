using System.ComponentModel;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum StatusCertificacaoTurma
    {
        [Description("Não emitidos")]
        NaoEmitido = 1,

        [Description("Emitir certificados")]
        DisponivelParaEmissao = 2,

        [Description("Sem certificados")]
        SemCertificacao = 3,

        [Description("Certificados emitidos")]
        Emitido = 4
    }
}