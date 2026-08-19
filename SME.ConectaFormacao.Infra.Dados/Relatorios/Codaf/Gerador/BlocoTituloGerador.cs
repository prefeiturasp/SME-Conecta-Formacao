using ClosedXML.Excel;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador.Intefaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador
{
    [ExcludeFromCodeCoverage]
    public class BlocoTituloGerador : IBlocoTituloGerador
    {
        public int Processar(IXLWorksheet sheet, int linhaInicial, TituloRelatorioCodafDto dados)
        {
            // Títulos Fixos
            CriarLinhaTitulo(sheet, 3, dados.Titulo1);
            CriarLinhaTitulo(sheet, 4, dados.Titulo2);
            CriarLinhaTitulo(sheet, 5, dados.Titulo3);

            return 6; // Próxima linha é a 6
        }
        private static void CriarLinhaTitulo(IXLWorksheet sheet, int linha, string texto)
        {
            var range = sheet.Range($"C{linha}:R{linha}");
            range.Merge();
            range.Value = texto;
            range.Style.Font.Bold = true;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
        }
    }
}
