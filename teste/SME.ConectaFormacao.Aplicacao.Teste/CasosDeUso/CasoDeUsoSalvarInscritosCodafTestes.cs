using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoSalvarInscritosCodafTestes
    {
        private readonly Mock<ICodafInscritosListaPresencaService> _inscritosServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoSalvarInscritosCodaf _sut;
        private readonly Faker _faker;
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;

        public CasoDeUsoSalvarInscritosCodafTestes()
        {
            var mocker = new AutoMocker();
            _inscritosServiceMock = mocker.GetMock<ICodafInscritosListaPresencaService>();
            _mapperMock = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<CasoDeUsoSalvarInscritosCodaf>();
            _faker = new();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
        }

        [Fact]
        public async Task DadoUmaListaDeInscritosVazia_QuandoSalvarInscritos_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long();

            // Act
            var resultado = await _sut.ExecutarAsync([], codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            _inscritosServiceMock.Verify(i => i.SalvarInscritosAsync(It.IsAny<List<CodafInscricaoListaPresenca>>(), It.IsAny<long>())
            , Times.Never);
        }

        [Fact]
        public async Task DadoUmaListaComInscritosDuplicados_QuandoSalvarInscritos_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long();
            var inscritos = new List<CodafInscritoListaPresencaSalvarDto>
            {
                new()
                {
                    InscricaoId = 123
                },
                new()
                {
                    InscricaoId = 123
                }
            };

            // Act
            var resultado = await _sut.ExecutarAsync(inscritos, codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            _inscritosServiceMock.Verify(i => i.SalvarInscritosAsync(It.IsAny<List<CodafInscricaoListaPresenca>>(), It.IsAny<long>())
            , Times.Never);
        }

        [Fact]
        public async Task DadoUmaListaDeInscritosValida_QuandoSalvarInscritos_EntaoDeveSalvarInscritos()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long();
            var inscritoDto = new CodafInscritoListaPresencaSalvarDto
            {
                InscricaoId = _faker.Random.Long(1),
                Aprovado = _faker.Random.Bool(),
                AtividadeObrigatorio = _faker.Random.Bool(),
                ConceitoFinal = _faker.Random.String(),
                PercentualFrequencia = _faker.Random.Decimal(1, 100)
            };


            var codaf = new CodafListaPresenca(
                propostaId: _faker.Random.Long(1),
                propostaTurmaId: _faker.Random.Long(1),
                StatusCodafListaPresenca.Iniciado
            );
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(codafListaPresencaId))
                .ReturnsAsync(codaf);   

            var inscritosDto = new List<CodafInscritoListaPresencaSalvarDto> { inscritoDto };
            var inscritos = new List<CodafInscricaoListaPresenca>
            {
                new()
                {
                    Aprovado = inscritoDto.Aprovado,
                    PercentualFrequencia = inscritoDto.PercentualFrequencia,
                    ConceitoFinal = inscritoDto.ConceitoFinal,
                    AtividadeObrigatorio = inscritoDto.AtividadeObrigatorio,
                    InscricaoId = inscritoDto.InscricaoId
                }
            };

            _mapperMock
                .Setup(m => m.Map<List<CodafInscricaoListaPresenca>>(It.IsAny<List<CodafInscritoListaPresencaSalvarDto>>()))
                .Returns(inscritos);

            // Act
            var resultado = await _sut.ExecutarAsync(inscritosDto, codafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _inscritosServiceMock.Verify(i => i.SalvarInscritosAsync(It.IsAny<List<CodafInscricaoListaPresenca>>(), codafListaPresencaId)
            , Times.Once);
        }
    }
}