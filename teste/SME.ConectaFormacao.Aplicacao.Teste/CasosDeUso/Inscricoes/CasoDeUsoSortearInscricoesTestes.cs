using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Dominio.Constantes;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoSortearInscricoesTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoSortearInscricoes _sut;
        private readonly Faker _faker;

        public CasoDeUsoSortearInscricoesTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoSortearInscricoes>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaTurmaIdValido_QuandoExecutar_EntaoDeveEnviarSortearInscricaoCommandERetornarRetornoDTOSucesso()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<SortearInscricaoCommand>(c => c.PropostaTurmaId == propostaTurmaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(propostaTurmaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be(MensagemNegocio.SORTEIO_REALIZADO_COM_SUCESSO);
            resultado.EntidadeId.Should().Be(propostaTurmaId);
            _mediatorMock.Verify(m => m.Send(It.Is<SortearInscricaoCommand>(c => c.PropostaTurmaId == propostaTurmaId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1, 1000);
            var excecaoEsperada = new InvalidOperationException("Erro ao sortear inscrições.");

            _mediatorMock
                .Setup(m => m.Send(It.Is<SortearInscricaoCommand>(c => c.PropostaTurmaId == propostaTurmaId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var act = () => _sut.Executar(propostaTurmaId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro ao sortear inscrições.");
            _mediatorMock.Verify(m => m.Send(It.Is<SortearInscricaoCommand>(c => c.PropostaTurmaId == propostaTurmaId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
