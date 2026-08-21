using ClosedXML.Excel;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador.Extensions;
using SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador.Intefaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador
{
    [ExcludeFromCodeCoverage]
    public class BlocoRegentesGerador : IBlocoRegentesGerador
    {
        private readonly XLColor _corFundoSubTitulo = XLColor.FromHtml("#E6E6E6");

        public int Processar(IXLWorksheet sheet, int linhaInicial, List<RegenteTurmaRelatorioCodafDto> regentes)
        {
            var linha = linhaInicial;

            // Título do Bloco
            var rangeTitulo = sheet.Range(linha, 1, linha, 20); // A:T
            rangeTitulo.Merge();
            rangeTitulo.Value = "REGENTES DA TURMA COM RF";
            rangeTitulo.Style.Font.Bold = true;
            rangeTitulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeTitulo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeTitulo.Style.Fill.BackgroundColor = _corFundoSubTitulo;
            rangeTitulo.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            linha++;

            // Lista de Regentes
            foreach (var regente in regentes)
            {
                RenderizarLinhaRegente(sheet, linha, regente);
                linha++;
            }

            return linha;
        }

        private static void RenderizarLinhaRegente(IXLWorksheet sheet, int linha, RegenteTurmaRelatorioCodafDto regente)
        {
            // Label Nome
            var cellLabelNome = sheet.Cell(linha, 1);
            cellLabelNome.ConfigurarLabelComFundo("NOME:");

            // Valor Nome (B:K)
            var rangeNome = sheet.Range(linha, 2, linha, 11);
            rangeNome.Merge();
            rangeNome.Value = regente.NomeRegente;
            rangeNome.EstilizarValor(bordaDireita: XLBorderStyleValues.Thin);

            // Label RF (L)
            var cellLabelRf = sheet.Cell(linha, 12);
            cellLabelRf.ConfigurarLabelComFundo("R.F.:");

            // Valor RF (M:O)
            var rangeRf = sheet.Range(linha, 13, linha, 15);
            rangeRf.Merge();
            rangeRf.Value = FormatarDocumento(regente.RfRegente);
            rangeRf.EstilizarValor(bordaDireita: XLBorderStyleValues.Thin, centralizar: true);

            // Label Registro (P:R)
            var rangeLblReg = sheet.Range(linha, 16, linha, 18);
            rangeLblReg.Merge();
            rangeLblReg.ConfigurarLabelComFundo("Nº DE REGISTRO:");
            rangeLblReg.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeLblReg.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // Valor Registro (S:T)
            var rangeRegistro = sheet.Range(linha, 19, linha, 20);
            rangeRegistro.Merge();
            rangeRegistro.Value = regente.CodigoCertificado.FormatarValorOuMascarar();
            rangeRegistro.EstilizarValor(bordaDireita: XLBorderStyleValues.Thick, centralizar: true);

            // Borda inferior da linha toda
            sheet.Range(linha, 1, linha, 20).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        private static string FormatarDocumento(string documento)
        {
            var (valor, tipo) = ResolvedorDocumentoUsuario.Resolver(documento, documento);
            return ResolvedorDocumentoUsuario.FormatarValor(valor, tipo);
        }


    }
}
