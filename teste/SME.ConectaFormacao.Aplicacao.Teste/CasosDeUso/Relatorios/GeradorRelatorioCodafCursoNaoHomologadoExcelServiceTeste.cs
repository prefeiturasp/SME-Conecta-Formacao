using Bogus;
using ClosedXML.Excel;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Relatorios.Codaf.Gerador.Intefaces;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Relatorios
{
    public class GeradorRelatorioCodafCursoNaoHomologadoExcelServiceTeste
    {
        private readonly Mock<ITemplateService> templateServiceMock;
        private readonly Mock<IBlocoTituloGerador> blocoTituloMock;
        private readonly Mock<IBlocoCabecalhoGerador> blocoCabecalhoMock;
        private readonly Mock<IBlocoRegentesGerador> blocoRegentesMock;
        private readonly Mock<IBlocoAlunosGerador> blocoAlunosMock;
        private readonly Mock<IBlocoAssinaturaGerador> blocoAssinaturaMock;
        private readonly GeradorRelatorioCodafCursoNaoHomologadoExcelService sut;
        private readonly Faker faker;

        public GeradorRelatorioCodafCursoNaoHomologadoExcelServiceTeste()
        {
            var mocker = new AutoMocker();
            templateServiceMock = mocker.GetMock<ITemplateService>();
            blocoTituloMock = mocker.GetMock<IBlocoTituloGerador>();
            blocoCabecalhoMock = mocker.GetMock<IBlocoCabecalhoGerador>();
            blocoRegentesMock = mocker.GetMock<IBlocoRegentesGerador>();
            blocoAlunosMock = mocker.GetMock<IBlocoAlunosGerador>();
            blocoAssinaturaMock = mocker.GetMock<IBlocoAssinaturaGerador>();

            sut = mocker.CreateInstance<GeradorRelatorioCodafCursoNaoHomologadoExcelService>();
            faker = new Faker("pt_BR");

            templateServiceMock.Setup(t => t.ObterTemplateBytes(It.IsAny<string>())).Returns(CriarTemplateExcelValido);

            blocoTituloMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<TituloRelatorioCodafDto>())).Returns(5);
            blocoCabecalhoMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<CabecalhoRelatorioCodafDto>())).Returns(10);
            blocoRegentesMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<List<RegenteTurmaRelatorioCodafDto>>())).Returns(15);
            blocoAlunosMock.Setup(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<GrupoAlunosRelatorioCodafDto>())).Returns(20);
        }

        [Fact(DisplayName = "GerarRelatorio - Deve orquestrar os blocos e retornar os bytes do Excel")]
        public void Deve_Orquestrar_Blocos_E_Retornar_Bytes_Do_Excel()
        {
            var dadosBrutos = CriarDadosValidos();

            var resultadoBytes = sut.GerarRelatorio(dadosBrutos);

            resultadoBytes.Should().NotBeNull();
            resultadoBytes.Should().NotBeEmpty();

            blocoTituloMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), 1, It.IsAny<TituloRelatorioCodafDto>()), Times.Once);
            blocoCabecalhoMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), 5, It.IsAny<CabecalhoRelatorioCodafDto>()), Times.Once);
            blocoRegentesMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), 10, It.IsAny<List<RegenteTurmaRelatorioCodafDto>>()), Times.Once);

            // Os 4 grupos: AprovadosMunicipal, AprovadosParceira, ReprovadosMunicipal, ReprovadosParceira
            blocoAlunosMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), It.IsAny<int>(), It.IsAny<GrupoAlunosRelatorioCodafDto>()), Times.Exactly(4));

            blocoAssinaturaMock.Verify(b => b.Processar(It.IsAny<IXLWorksheet>(), 20, null), Times.Once);
        }

        [Fact(DisplayName = "GerarRelatorio - Deve tratar nome de turma muito longo para não exceder o limite de aba do Excel")]
        public void Deve_Tratar_Nome_De_Turma_Muito_Longo()
        {
            var dadosBrutos = CriarDadosValidos();
            dadosBrutos.NomeTurma = "NOME DE TURMA MUITO LONGO QUE PASSA DE TRINTA E UM CARACTERES PERMITIDOS PELO EXCEL";

            var resultadoBytes = sut.GerarRelatorio(dadosBrutos);

            resultadoBytes.Should().NotBeNull();

            using var ms = new MemoryStream(resultadoBytes);
            using var workbook = new XLWorkbook(ms);
            workbook.Worksheets.Count.Should().Be(1);
            workbook.Worksheets.First().Name.Length.Should().BeLessThanOrEqualTo(31);
        }

        private DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto CriarDadosValidos()
        {
            return new DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto
            {
                CodafId = faker.Random.Long(1),
                TurmaId = faker.Random.Long(1),
                NomeTurma = "Turma Teste",
                QuantidadeVagasTurma = 30,
                NomeAreaPromotora = "Área Promotora Teste",
                TipoFormacao = TipoFormacao.Curso,
                NomeFormacao = "Formação Teste",
                QuantidadeTurmas = 1,
                PeriodoRealizacaoInicio = DateTime.Today,
                PeriodoRealizacaoFim = DateTime.Today.AddDays(5),
                CursoComCertificado = true,
                NumeroHomologacao = 123,
                CodigoEventoSigpec = 456,
                CargaHorariaTotal = 40,
                CargaHorariaDistancia = "10:00",
                CargaHorariaPresencial = "20:00",
                CargaHorariaSincrona = "10:00",
                TipoFormato = Formato.Presencial,
                NomeDre = "DRE Teste",
                Observacao = "Observação teste",
                DataCodaf = DateTime.Today,
                NumeroComunicado = 789,
                DataPublicacao = DateTime.Today,
                DataPublicacaoDom = DateTime.Today,
                PaginaComunicadoDom = 10,
                Retificacoes = null,
                DataAulas = [],
                RegentesTurma = [],
                Participantes = []
            };
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