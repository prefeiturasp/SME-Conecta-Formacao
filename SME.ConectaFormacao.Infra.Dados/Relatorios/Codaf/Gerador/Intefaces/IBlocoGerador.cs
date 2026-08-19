using ClosedXML.Excel;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador.Intefaces
{
    public interface IBlocoGerador<in T>
    {
        int Processar(IXLWorksheet sheet, int linhaInicial, T dados);
    }
}
