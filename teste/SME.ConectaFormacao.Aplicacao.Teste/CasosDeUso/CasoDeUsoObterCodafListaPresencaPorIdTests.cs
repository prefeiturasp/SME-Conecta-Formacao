using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterCodafListaPresencaPorIdTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoObterCodafListaPresencaPorId _casoDeUsoObterCodafListaPresencaPorId;
        private readonly Faker _faker;

        public CasoDeUsoObterCodafListaPresencaPorIdTests()
        {
            var mocker = new Moq.AutoMock.AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _mapperMock = mocker.GetMock<IMapper>();
            _casoDeUsoObterCodafListaPresencaPorId = mocker.CreateInstance<CasoDeUsoObterCodafListaPresencaPorId>();
            _faker = new();
        }

        [Fact]
        public async Task DadoIdValido_QuandoChamarExecutar_DeveRetornarResultadoEsperado()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);
            var listaPresencaEntidade = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: 1,
                dataPublicacao: DateTime.Now,
                dataPublicacaoDom: DateTime.Now,
                numeroComunicado: 123,
                paginaComunicadoDom: 12,
                codigoCursoEol: 1,
                codigoNivel: 2,
                observacao: "Observação teste",
                Perfis.ADMIN_DF);
            var listaPresencaDto = new CodafListaPresencaDto
            {
                Id = listaPresencaId,
                PropostaId = 1,
                PropostaTurmaId = 1
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(listaPresencaEntidade))
                .Returns(listaPresencaDto);

            // Act
            var resultado = await _casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(listaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Id.Should().Be(listaPresencaId);
        }

        [Fact]
        public async Task DadoIdInvalido_QuandoChamarExecutar_DeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync((CodafListaPresenca?)null);
            // Act
            var resultado = await _casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(listaPresencaId);
            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().NotBeNull();
            resultado.MensagensErro.Should().Contain("Lista de presença não encontrada.");
        }
    }
}
