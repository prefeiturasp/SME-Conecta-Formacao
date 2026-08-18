using System;
using System.IO;
using ClosedXML.Excel;

class Program
{
    static void Main()
    {
        var path = @"C:\dev\Back\SME-Conecta-Formacao\SME.ConectaFormacao.Infra.Dados\Templates\Template_Relatorio_Codaf_Suplementar_Modelo_2026.xlsx";
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(1);
        foreach (var cell in ws.CellsUsed())
        {
            if (cell.Value.Type == XLDataType.Text && !string.IsNullOrWhiteSpace(cell.Value.GetText()))
            {
                Console.WriteLine(cell.Address + ": " + cell.Value.GetText());
            }
        }
    }
}
