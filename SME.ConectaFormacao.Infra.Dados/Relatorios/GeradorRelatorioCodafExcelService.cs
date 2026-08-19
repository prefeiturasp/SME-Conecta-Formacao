using ClosedXML.Excel;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador.Intefaces;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios
{
    public class GeradorRelatorioCodafExcelService(
        ITemplateService templateService,
        IBlocoTituloGerador blocoTitulo,
        IBlocoCabecalhoGerador blocoCabecalho,
        IBlocoRegentesGerador blocoRegentes,
        IBlocoAlunosGerador blocoAlunos,
        IBlocoAssinaturaGerador blocoAssinatura) : IGeradorRelatorioCodafExcelService
    {
        private const string NOME_ARQUIVO_TEMPLATE = "Template_Relatorio_Codaf_Modelo_2026.xlsx";

        public byte[] GerarRelatorio(DadosPrincipaisRelatorioCodafDto dadosBrutos, bool ehCodafSuplementar)
        {
            var dadosRelatorio = RelatorioCodafDto.MapearParaDtoEstruturado(dadosBrutos);
            var templateBytes = templateService.ObterTemplateBytes(NOME_ARQUIVO_TEMPLATE);

            using var stream = new MemoryStream();
            stream.Write(templateBytes, 0, templateBytes.Length);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);
            var templateSheet = workbook.Worksheet(1); // Assuming first sheet is the template

            foreach (var turma in dadosRelatorio.Turmas)
            {
                var nomeAba = turma.NomeTurma.Length > 31 ? turma.NomeTurma[..31] : turma.NomeTurma;
                // Avoid duplicate names if turmas have similar names
                var baseName = nomeAba;
                var suffix = 1;
                while (workbook.TryGetWorksheet(nomeAba, out _))
                {
                    nomeAba = $"{baseName[..Math.Min(baseName.Length, 28)]}({suffix})";
                    suffix++;
                }

                var sheet = templateSheet.CopyTo(nomeAba);

                var linhaAtual = 1;

                // 1º Bloco: Título (Brasão)
                linhaAtual = blocoTitulo.Processar(sheet, linhaAtual, new()
                {
                    Titulo1 = "SECRETARIA MUNICIPAL DE EDUCAÇÃO - SME",
                    Titulo2 = ehCodafSuplementar ? "CONTROLE DE DOCUMENTAÇÃO DAS AÇÕES FORMATIVAS - CODAF SUPLEMENTAR" : "CONTROLE DE DOCUMENTAÇÃO DAS AÇÕES FORMATIVAS - CODAF",
                    Titulo3 = "RELATÓRIO DE CONCLUSÃO DE TURMA - MODELO 2026 - REDE DIRETA"
                });

                // 2º Bloco: Cabeçalho
                linhaAtual = blocoCabecalho.Processar(sheet, linhaAtual, turma.Cabecalho);

                // 3º Bloco: Regentes
                linhaAtual = blocoRegentes.Processar(sheet, linhaAtual, turma.RegentesDaTurma);

                // Linha Vazia
                sheet.Range(linhaAtual, 1, linhaAtual, 20);
                linhaAtual++;

                // 4º Bloco: Alunos
                linhaAtual = blocoAlunos.Processar(sheet, linhaAtual, turma.AlunosAprovadosMunicipal);
                linhaAtual = blocoAlunos.Processar(sheet, linhaAtual, turma.AlunosAprovadosParceira);
                linhaAtual = blocoAlunos.Processar(sheet, linhaAtual, turma.AlunosReprovadosMunicipal);
                linhaAtual = blocoAlunos.Processar(sheet, linhaAtual, turma.AlunosReprovadosParceira);

                var rangeBordaInferior = sheet.Range(6, 1, linhaAtual - 1, 20);
                rangeBordaInferior.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                // 5º Bloco: Assinaturas
                blocoAssinatura.Processar(sheet, linhaAtual, null);
            }

            templateSheet.Delete();

            using var outStream = new MemoryStream();
            workbook.SaveAs(outStream);
            return outStream.ToArray();
        }
    }
}
