using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoObterSituacoesPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterSituacoesProposta _casoDeUso;

        public CasoDeUsoObterSituacoesPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterSituacoesProposta>();
        }

        [Fact]
        public async Task DadoSituacoesExistentes_QuandoExecutar_EntaoDeveEnviarQueryERetornarLista()
        {
            // Arrange
            var situacoesEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Criada" },
                new() { Id = 2, Descricao = "Em Análise" },
                new() { Id = 3, Descricao = "Aprovada" }
            };

            _mediatorMock
                .Setup(m => m.Send(ObterSituacaoPropostaQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(situacoesEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(situacoesEsperadas);

            _mediatorMock.Verify(m => m.Send(ObterSituacaoPropostaQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoSituacoesInexistentes_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var situacoesEsperadas = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(ObterSituacaoPropostaQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(situacoesEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(ObterSituacaoPropostaQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoEnviarQuery_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            const string mensagemErro = "Erro ao obter situações da proposta";

            _mediatorMock
                .Setup(m => m.Send(ObterSituacaoPropostaQuery.Instancia, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _casoDeUso.Executar();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(ObterSituacaoPropostaQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
