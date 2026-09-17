using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterInformacoesInscricoesEstaoAbertasPorIdTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterInformacoesInscricoesEstaoAbertasPorIdTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaIdValido_QuandoExecutar_EntaoDeveEnviarQueryERetornarPodeInscreverMensagemDTO()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var esperado = new PodeInscreverMensagemDTO
            {
                PodeInscrever = true,
                Mensagem = "Inscrições abertas para esta formação"
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterInformacoesInscricoesEstaoAbertasPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(esperado);

            // Act
            var resultado = await _sut.Executar(propostaId);

            // Assert
            resultado.Should().BeEquivalentTo(esperado);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterInformacoesInscricoesEstaoAbertasPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterInformacoesInscricoesEstaoAbertasPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(propostaId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterInformacoesInscricoesEstaoAbertasPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
