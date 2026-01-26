using System.ComponentModel;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum StatusCertificacaoTurma
    {
        [Description("Sem certificados")]
        SemCertificado = 0,

        [Description("Pendente emissão remessa")]
        PendenteEmissaoRemessa = 1,

        [Description("Emitir certificados")]
        DisponivelParaEmissao = 2,

        [Description("Certificados emitidos")]
        Emitido = 3
    }
}