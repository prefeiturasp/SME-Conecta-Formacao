using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriaPorId;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterCoordenadoriaPorIdQueryHandlerTestes
    {
        private readonly Mock<IRepositorioCoordenadoria> _repositorioCoordenadoriaMock;
        private readonly ObterCoordenadoriaPorIdQueryHandler _sut;
        private readonly Faker _faker;

        public ObterCoordenadoriaPorIdQueryHandlerTestes()
        {
            var mocker = new AutoMocker();

            _repositorioCoordenadoriaMock = mocker.GetMock<IRepositorioCoordenadoria>();
            _sut = mocker.CreateInstance<ObterCoordenadoriaPorIdQueryHandler>();
            _faker = new("pt_BR");
        }

        [Fact]
        public async Task DadoIdValido_QuandoTratarRequisicao_EntaoRetornaResultadoSucessoComDados()
        {
            // Arrange
            var query = new ObterCoordenadoriaPorIdQuery(_faker.Random.Long(1, 100));

            var areaPromotora = new AreaPromotora
            {
                Id = _faker.Random.Long(1, 100),
                Nome = _faker.Company.CompanyName()
            };

            var coordenadoria = new Coordenadoria
            {
                Id = query.Id,
                Nome = _faker.Company.CompanyName(),
                Sigla = _faker.Company.CompanySuffix(),
                CriadoEm = _faker.Date.Past(),
                CriadoPor = _faker.Person.FullName,
                CriadoLogin = _faker.Internet.UserName(),
                AlteradoEm = _faker.Date.Recent(),
                AlteradoPor = _faker.Person.FullName,
                AlteradoLogin = _faker.Internet.UserName(),
                AreasPromotoras = [areaPromotora]
            };

            _repositorioCoordenadoriaMock
                .Setup(r => r.ObterComAreaPromotoraAsync(query.Id))
                .ReturnsAsync(coordenadoria);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
            resultado.Dados.Should().NotBeNull();

            resultado.Dados!.Id.Should().Be(coordenadoria.Id);
            resultado.Dados.Nome.Should().Be(coordenadoria.Nome);
            resultado.Dados.Sigla.Should().Be(coordenadoria.Sigla);
            resultado.Dados.CriadoEm.Should().Be(coordenadoria.CriadoEm);
            resultado.Dados.CriadoPor.Should().Be(coordenadoria.CriadoPor);
            resultado.Dados.CriadoLogin.Should().Be(coordenadoria.CriadoLogin);
            resultado.Dados.AlteradoEm.Should().Be(coordenadoria.AlteradoEm);
            resultado.Dados.AlteradoPor.Should().Be(coordenadoria.AlteradoPor);
            resultado.Dados.AlteradoLogin.Should().Be(coordenadoria.AlteradoLogin);

            resultado.Dados.AreasPromotoras.Should().HaveCount(1);
            resultado.Dados.AreasPromotoras.First().Id.Should().Be(areaPromotora.Id);
            resultado.Dados.AreasPromotoras.First().Nome.Should().Be(areaPromotora.Nome);

            _repositorioCoordenadoriaMock.Verify(r => r.ObterComAreaPromotoraAsync(query.Id), Times.Once);
        }

        [Fact]
        public async Task DadoIdInvalido_QuandoTratarRequisicao_EntaoRetornaErroNaoEncontrado()
        {
            // Arrange
            var query = new ObterCoordenadoriaPorIdQuery(_faker.Random.Long(1, 100));

            _repositorioCoordenadoriaMock
                .Setup(r => r.ObterComAreaPromotoraAsync(query.Id))
                .ReturnsAsync((Coordenadoria?)null);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().ContainSingle().Which.Should().Be("Coordenadoria não encontrada.");
            resultado.Dados.Should().BeNull();

            _repositorioCoordenadoriaMock.Verify(r => r.ObterComAreaPromotoraAsync(query.Id), Times.Once);
        }
    }
}