using ClosedXML.Excel;

namespace SME.ConectaFormacao.Aplicacao.Extensoes;

public static class PlanilhaExtension
{
    public static string ObterValorDaCelula(this IXLWorksheet planilha, int numeroLinha, int coluna)
    {
        return planilha.Cell(numeroLinha, coluna).Value.ToString().Trim();
    }
}