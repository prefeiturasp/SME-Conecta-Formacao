using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterPropostaEncontroPaginacaoTestes
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly Mock<IRepositorioPropostaEncontro> _repositorioPropostaEncontroMock;

        private readonly CasoDeUsoObterPropostaEncontroPaginacao _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterPropostaEncontroPaginacaoTestes()
        {
            _mocker = new AutoMocker();
            _contextoAplicacaoMock = _mocker.GetMock<IContextoAplicacao>();
            _repositorioPropostaEncontroMock = _mocker.GetMock<IRepositorioPropostaEncontro>();

            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroPagina")).Returns("1");
            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroRegistros")).Returns("10");

            _sut = _mocker.CreateInstance<CasoDeUsoObterPropostaEncontroPaginacao>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoIdIgualAZero_QuandoExecutarAsync_EntaoDeveRetornarPaginacaoVazia()
        {
            // Arrange
            long idProposta = 0;

            // Act
            var resultado = await _sut.ExecutarAsync(idProposta);

            // Assert
            resultado.Items.Should().BeEmpty();
            resultado.TotalRegistros.Should().Be(0);

            _repositorioPropostaEncontroMock.Verify(r =>
                r.ObterEncontrosPorPropostaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoPropostaSemEncontros_QuandoExecutarAsync_EntaoDeveRetornarPaginacaoVaziaComDadosDoRepositorio()
        {
            // Arrange
            long idProposta = _faker.Random.Long(1, 100);
            var resultadoPaginadoVazio = new ResultadoPaginado<PropostaEncontro>
            {
                Itens = [],
                TotalRegistros = 15,
                TamanhoPagina = 10
            };

            _repositorioPropostaEncontroMock
                .Setup(r => r.ObterEncontrosPorPropostaAsync(idProposta, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(resultadoPaginadoVazio);

            // Act
            var resultado = await _sut.ExecutarAsync(idProposta);

            // Assert
            resultado.Items.Should().BeEmpty();
            resultado.TotalRegistros.Should().Be(15);
        }

        [Fact]
        public async Task DadoEncontroComDataUnica_QuandoExecutarAsync_EntaoDeveMapearListaSemExpansao()
        {
            // Arrange
            long idProposta = _faker.Random.Long(1, 100);
            var dataEncontro = _faker.Date.Future().Date;

            var encontro = CriarPropostaEncontroMock(dataEncontro, dataEncontro);
            ConfigurarMockRepositorio(idProposta, [encontro]);
            var horaInicioEsperada = encontro.HoraInicio;
            var horaFimEsperada = encontro.HoraFim;

            // Act
            var resultado = await _sut.ExecutarAsync(idProposta);

            // Assert
            resultado.Items.Should().HaveCount(1);
            var cronogramaMapeado = resultado.Items.First();

            cronogramaMapeado.Id.Should().Be(encontro.Id);
            cronogramaMapeado.CronogramaDatas.Should().HaveCount(1);
            cronogramaMapeado.CronogramaDatas[0].Data.Should().Be(dataEncontro);
            cronogramaMapeado.CronogramaDatas[0].HoraInicio.Should().Be(horaInicioEsperada);
            cronogramaMapeado.CronogramaDatas[0].HoraFim.Should().Be(horaFimEsperada);
        }

        [Fact]
        public async Task DadoEncontroEmPeriodo_QuandoExecutarAsync_EntaoDeveExpandirRemovendoFinaisDeSemana()
        {
            // Arrange
            long idProposta = _faker.Random.Long(1, 100);

            var dataInicio = new DateTime(2023, 11, 1, 0, 0, 0, DateTimeKind.Utc);
            var dataFim = new DateTime(2023, 11, 6, 0, 0, 0, DateTimeKind.Utc);
            var diasUteisEsperados = 4; // Qua, Qui, Sex, Seg

            var encontro = CriarPropostaEncontroMock(dataInicio, dataFim);
            ConfigurarMockRepositorio(idProposta, [encontro]);

            // Act
            var resultado = await _sut.ExecutarAsync(idProposta);

            // Assert
            resultado.Items.Should().HaveCount(1);
            var cronogramaMapeado = resultado.Items.First();

            cronogramaMapeado.CronogramaDatas.Should().HaveCount(diasUteisEsperados);
            cronogramaMapeado.CronogramaDatas.Should().NotContain(c => c.Data.DayOfWeek == DayOfWeek.Saturday);
            cronogramaMapeado.CronogramaDatas.Should().NotContain(c => c.Data.DayOfWeek == DayOfWeek.Sunday);
        }

        // --- Métodos Privados Auxiliares ---

        private PropostaEncontro CriarPropostaEncontroMock(DateTime dataInicio, DateTime dataFim)
        {
            return new PropostaEncontro
            {
                Id = _faker.Random.Long(1, 1000),
                Tipo = _faker.PickRandom<TipoEncontro>(),
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Turmas =
                [
                    new() { Turma = new PropostaTurma { Nome = _faker.Commerce.Department() } }
                ],
                Datas =
                [
                    new() { DataInicio = dataInicio, DataFim = dataFim }
                ]
            };
        }

        private void ConfigurarMockRepositorio(long idProposta, IEnumerable<PropostaEncontro> itensRetorno)
        {
            var resultadoPaginado = new ResultadoPaginado<PropostaEncontro>
            {
                Itens = itensRetorno,
                TotalRegistros = itensRetorno.Count(),
                TamanhoPagina = 10
            };

            _repositorioPropostaEncontroMock
                .Setup(r => r.ObterEncontrosPorPropostaAsync(idProposta, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(resultadoPaginado);
        }
    }
}