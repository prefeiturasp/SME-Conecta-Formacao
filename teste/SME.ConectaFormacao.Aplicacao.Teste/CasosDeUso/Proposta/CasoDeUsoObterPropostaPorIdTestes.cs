using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using System;
using System.Threading.Tasks;
using Xunit;
using EntidadeProposta = SME.ConectaFormacao.Dominio.Entidades.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaPorIdTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterPropostaPorId _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterPropostaPorIdTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterPropostaPorId>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoIdValidoComPropostaExistente_QuandoExecutar_EntaoDeveRetornarPropostaCompletoDTO()
        {
            // Arrange
            var id = _faker.Random.Long(1, 10000);
            var propostaRetorno = new PropostaCompletoDTO
            {
                NomeFormacao = _faker.Company.CatchPhrase(),
                Justificativa = _faker.Lorem.Paragraph(),
                Objetivos = _faker.Lorem.Paragraph()
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaCompletaPorIdQuery>(q => q.Id == id), default))
                .ReturnsAsync(propostaRetorno);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(propostaRetorno);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaCompletaPorIdQuery>(q => q.Id == id),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoIdValidoComPropostaNaoEncontrada_QuandoExecutar_EntaoDeveRetornarNulo()
        {
            // Arrange
            var id = _faker.Random.Long(1, 10000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaCompletaPorIdQuery>(q => q.Id == id), default))
                .ReturnsAsync((PropostaCompletoDTO?)null!);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().BeNull();

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaCompletaPorIdQuery>(q => q.Id == id),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoEnviarQuery_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 10000);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaCompletaPorIdQuery>(q => q.Id == id), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaCompletaPorIdQuery>(q => q.Id == id),
                default), Times.Once);
        }
    }
}
