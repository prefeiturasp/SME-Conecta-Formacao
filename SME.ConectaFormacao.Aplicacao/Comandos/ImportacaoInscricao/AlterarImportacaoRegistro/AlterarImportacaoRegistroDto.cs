using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarImportacaoRegistro
{
    public readonly record struct AlterarImportacaoRegistroDto(long Id, string Conteudo, SituacaoImportacaoArquivoRegistro Situacao, string? Erro)
    {
    }
}
