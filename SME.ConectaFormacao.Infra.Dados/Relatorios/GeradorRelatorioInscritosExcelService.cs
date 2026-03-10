using MiniExcelLibs;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.InscritosPorFormacao;
using SME.ConectaFormacao.Infra.Dados.Templates;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios
{
    [ExcludeFromCodeCoverage]
    public class GeradorRelatorioInscritosExcelService(
        IServicoArmazenamento servicoArmazenamento, 
        ITemplateService templateService) : IGeradorRelatorioInscritosExcelService
    {
        private const string NOME_ARQUIVO_TEMPLATE = "Template_Relatorio_Inscritos_Por_Formacao.xlsx";
        private const string CONTENT_TYPE_EXCEL = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public async Task<string> GerarEArmazenarRelatorioAsync(RelatorioInscritosFormacaoDto dados)
        {
            var caminhoArquivoTemporario = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var nomeArquivo = $"Relatorio_Inscritos_Formacao_{dados.NomeUsuario.Replace(" ", "").RemoverCaracteresEspeciais()}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            try
            {
                var template = templateService.ObterTemplateBytes(NOME_ARQUIVO_TEMPLATE);
                var parametrosTemplate = new
                {
                    Usuario = dados.NomeUsuario,
                    RF = dados.Rf,
                    Data = dados.DataGeracao.ToString("dd/MM/yyyy"),
                    inscritos = dados.Inscritos
                };

                await MiniExcel.SaveAsByTemplateAsync(caminhoArquivoTemporario, template, parametrosTemplate);

                await using var filestream = new FileStream(
                    caminhoArquivoTemporario, FileMode.Open, 
                    FileAccess.Read, FileShare.Read, bufferSize: 81920, useAsync: true);

                var nomeArquiboBucket = $"relatorios/inscritos_formacao/{DateTime.Now:yyyy/MM}/{nomeArquivo}";

                var chaveAcesso = await servicoArmazenamento.Armazenar(nomeArquiboBucket, filestream, CONTENT_TYPE_EXCEL);

                return chaveAcesso;
            }
            finally
            {
                if (File.Exists(caminhoArquivoTemporario))
                    File.Delete(caminhoArquivoTemporario);
            }
        }
    }
}
