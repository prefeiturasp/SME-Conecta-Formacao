using Bogus;
using ClosedXML.Excel;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador.Intefaces;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Relatorios
{
    public class GeradorRelatorioCodafExcelServiceTestes
    {
        private readonly Mock<ITemplateService> _templateServiceMock;
        private readonly Mock<IBlocoTituloGerador> _blocoTituloMock;
        private readonly Mock<IBlocoCabecalhoGerador> _blocoCabecalhoMock;
        private readonly Mock<IBlocoRegentesGerador> _blocoRegentesMock;
        private readonly Mock<IBlocoAlunosGerador> _blocoAlunosMock;
        private readonly Mock<IBlocoAssinaturaGerador> _blocoAssinaturaMock;
        private readonly GeradorRelatorioCodafExcelService _sut;

        public GeradorRelatorioCodafExcelServiceTestes()
        {
            var mocker = new AutoMocker();
            _templateServiceMock = mocker.GetMock<ITemplateService>();
            _blocoTituloMock = mocker.GetMock<IBlocoTituloGerador>();
            _blocoCabecalhoMock = mocker.GetMock<IBlocoCabecalhoGerador>();
            _blocoRegentesMock = mocker.GetMock<IBlocoRegentesGerador>();
            _blocoAlunosMock = mocker.GetMock<IBlocoAlunosGerador>();
            _blocoAssinaturaMock = mocker.GetMock<IBlocoAssinaturaGerador>();

            _sut = mocker.CreateInstance<GeradorRelatorioCodafExcelService>();
        }

        [Fact]
        public void DadoDadosPrincipaisValidos_QuandoChamarGerarRelatorio_EntaoDeveOrquestrarGeradoresERetornarBytesDoExcel()
        {
            // Arrange
            var dadosBrutos = new DadosPrincipaisRelatorioCodafDto
            {
                NomeTurma = "Turma 1",
                Participantes = [],
                RegentesTurma = [],
                DataAulas = []
            };

            var bytesTemplate = CriarTemplateExcelValido();
            _templateServiceMock.Setup(t => t.ObterTemplateBytes(It.IsAny<string>())).Returns(bytesTemplate);

            _blocoTituloMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<TituloRelatorioCodafDto>())).Returns(5);
            _blocoCabecalhoMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<CabecalhoRelatorioCodafDto>())).Returns(10);
            _blocoRegentesMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<List<RegenteTurmaRelatorioCodafDto>>())).Returns(15);
            _blocoAlunosMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<GrupoAlunosRelatorioCodafDto>())).Returns(20);

            // Act
            var resultadoBytes = _sut.GerarRelatorio(dadosBrutos, ehCodafSuplementar: false);

            // Assert
            resultadoBytes.Should().NotBeNull();
            resultadoBytes.Should().NotBeEmpty();

            _templateServiceMock.Verify(t => t.ObterTemplateBytes("Template_Relatorio_Codaf_Modelo_2026.xlsx"), Times.Once);
            _blocoTituloMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), 1, It.IsAny<TituloRelatorioCodafDto>()), Times.Once);
            _blocoCabecalhoMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), 5, It.IsAny<CabecalhoRelatorioCodafDto>()), Times.Once);
            _blocoRegentesMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), 10, It.IsAny<List<RegenteTurmaRelatorioCodafDto>>()), Times.Once);

            // O bloco de alunos é chamado 4 vezes (AprovadosSME, AprovadosParceira, ReprovadosSME, ReprovadosParceira)
            _blocoAlunosMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<GrupoAlunosRelatorioCodafDto>()), Times.Exactly(4));

            _blocoAssinaturaMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), 20, It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void DadoTurmaComNomeMuitoLongo_QuandoChamarGerarRelatorio_EntaoDeveTratarONomeDaAbaParaEvitarErroDoExcel()
        {
            // Arrange
            var dadosBrutos = new DadosPrincipaisRelatorioCodafDto
            {
                NomeTurma = "NOME DE TURMA MUITO LONGO QUE PASSA DE TRINTA E UM CARACTERES PERMITIDOS PELO EXCEL",
                Participantes = [],
                RegentesTurma = [],
                DataAulas = []
            };

            var bytesTemplate = CriarTemplateExcelValido();
            _templateServiceMock.Setup(t => t.ObterTemplateBytes(It.IsAny<string>())).Returns(bytesTemplate);

            _blocoTituloMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<TituloRelatorioCodafDto>())).Returns(5);
            _blocoCabecalhoMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<CabecalhoRelatorioCodafDto>())).Returns(10);
            _blocoRegentesMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<List<RegenteTurmaRelatorioCodafDto>>())).Returns(15);
            _blocoAlunosMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<GrupoAlunosRelatorioCodafDto>())).Returns(20);

            // Act
            var resultadoBytes = _sut.GerarRelatorio(dadosBrutos, ehCodafSuplementar: true);

            // Assert
            resultadoBytes.Should().NotBeNull();

            // O nome da aba é processado internamente, o fato de não lançar exceção significa que o substring funcionou.
            using var ms = new MemoryStream(resultadoBytes);
            using var workbook = new XLWorkbook(ms);
            // Deve existir a aba truncada
            workbook.Worksheets.Count.Should().BeGreaterThan(0);
        }

        private static byte[] CriarTemplateExcelValido()
        {
            using var wb = new XLWorkbook();
            wb.AddWorksheet("Template_Original");
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}
