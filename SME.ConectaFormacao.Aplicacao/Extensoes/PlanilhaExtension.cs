using ClosedXML.Excel;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Extensoes;

[ExcludeFromCodeCoverage]
public static class PlanilhaExtension
{
    public static string ObterValorDaCelula(this IXLWorksheet planilha, int numeroLinha, int coluna)
    {
        return planilha.Cell(numeroLinha, coluna).Value.ToString().Trim();
    }
}