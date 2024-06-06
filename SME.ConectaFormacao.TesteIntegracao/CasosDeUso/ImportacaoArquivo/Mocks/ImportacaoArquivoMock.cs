using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.ImportacaoArquivo.Mocks
{
    public class ImportacaoArquivoMock
    {
        public static IFormFile GerarArquivoValido()
        {
            var stream = ObterMockArquivoXLS();

            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", "importacao_arquivo.xlsx")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };

            return file;
        }

        private static MemoryStream ObterMockArquivoXLS()
        {
            const string COLUNA_TURMA_TEXTO = "TURMA";
            const string COLUNA_COLABORADOR_DA_REDE_TEXTO = "COLABORADOR DA REDE";
            const string COLUNA_REGISTRO_FUNCIONAL_TEXTO = "REGISTRO FUNCIONAL";
            const string COLUNA_CPF_TEXTO = "CPF";
            const string COLUNA_NOME_TEXTO = "NOME"; 
            const string COLUNA_VINCULO_TEXTO = "VINCULO";

            var stream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Planilha1");
                worksheet.Cell("A1").Value = COLUNA_TURMA_TEXTO;
                worksheet.Cell("B1").Value = COLUNA_COLABORADOR_DA_REDE_TEXTO;
                worksheet.Cell("C1").Value = COLUNA_REGISTRO_FUNCIONAL_TEXTO;
                worksheet.Cell("D1").Value = COLUNA_CPF_TEXTO;
                worksheet.Cell("E1").Value = COLUNA_NOME_TEXTO;
                worksheet.Cell("F1").Value = COLUNA_VINCULO_TEXTO;
                worksheet.Cell("A2").Value = COLUNA_TURMA_TEXTO;
                worksheet.Cell("B2").Value = COLUNA_COLABORADOR_DA_REDE_TEXTO;
                worksheet.Cell("C2").Value = COLUNA_REGISTRO_FUNCIONAL_TEXTO;
                worksheet.Cell("D2").Value = COLUNA_CPF_TEXTO;
                worksheet.Cell("E2").Value = COLUNA_NOME_TEXTO;
                worksheet.Cell("F2").Value = COLUNA_VINCULO_TEXTO;
                workbook.SaveAs(stream);
            }

            return stream;
        }
    }
}
