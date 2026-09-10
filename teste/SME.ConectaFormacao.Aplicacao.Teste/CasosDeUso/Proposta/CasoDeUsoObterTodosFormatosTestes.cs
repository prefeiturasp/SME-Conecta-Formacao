using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterTodosFormatosTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterTodosFormatos _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterTodosFormatosTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterTodosFormatos>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoFormatosCadastrados_QuandoExecutar_EntaoDeveRetornarListaDeFormatos()
        {
            // Arrange
            var formatosEsperados = new List<RetornoListagemDTO>
            {
                new RetornoListagemDTO { Id = _faker.Random.Long(1, 100), Descricao = _faker.Commerce.Department() },
                new RetornoListagemDTO { Id = _faker.Random.Long(101, 200), Descricao = _faker.Commerce.Department() }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterFormatosQuery>(q => q.TipoFormacao == null), default))
                .ReturnsAsync(formatosEsperados);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(formatosEsperados);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterFormatosQuery>(q => q.TipoFormacao == null), default), Times.Once);
        }

        [Fact]
        public async Task DadoSemFormatosCadastrados_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var formatosEsperados = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterFormatosQuery>(q => q.TipoFormacao == null), default))
                .ReturnsAsync(formatosEsperados);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterFormatosQuery>(q => q.TipoFormacao == null), default), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var mensagemErro = _faker.Lorem.Sentence();
            var excecaoEsperada = new Exception(mensagemErro);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterFormatosQuery>(q => q.TipoFormacao == null), default))
                .ThrowsAsync(excecaoEsperada);

            // Act
            Func<Task> act = async () => await _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterFormatosQuery>(q => q.TipoFormacao == null), default), Times.Once);
        }
    }
}
