using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterTiposAreaPromotoraTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterTiposAreaPromotora _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterTiposAreaPromotoraTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterTiposAreaPromotora>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoTiposAreaPromotoraCadastrados_QuandoExecutar_EntaoDeveRetornarColecaoEsperada()
        {
            // Arrange
            var tiposEsperados = new List<AreaPromotoraTipoDTO>
            {
                new AreaPromotoraTipoDTO { Id = _faker.Random.Short(1, 50), Nome = _faker.Company.CompanyName() },
                new AreaPromotoraTipoDTO { Id = _faker.Random.Short(51, 100), Nome = _faker.Company.CompanyName() }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterTiposAreaPromotoraQuery>(), default))
                .ReturnsAsync(tiposEsperados);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(tiposEsperados);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterTiposAreaPromotoraQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoSemTiposAreaPromotoraCadastrados_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var tiposEsperados = Enumerable.Empty<AreaPromotoraTipoDTO>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterTiposAreaPromotoraQuery>(), default))
                .ReturnsAsync(tiposEsperados);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterTiposAreaPromotoraQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var mensagemErro = _faker.Lorem.Sentence();
            var excecaoEsperada = new Exception(mensagemErro);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterTiposAreaPromotoraQuery>(), default))
                .ThrowsAsync(excecaoEsperada);

            // Act
            Func<Task> act = async () => await _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterTiposAreaPromotoraQuery>(), default), Times.Once);
        }
    }
}
