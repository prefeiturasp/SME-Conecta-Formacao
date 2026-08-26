using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Relatorio.ObterRelatorioSincrono;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    [ExcludeFromCodeCoverage]
    public class ObterRelatorioProspostaLaudaPublicacaoHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ObterRelatorioProspostaLaudaPublicacaoHandler _sut;

        public ObterRelatorioProspostaLaudaPublicacaoHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ObterRelatorioProspostaLaudaPublicacaoHandler>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoExecutar_EntaoRetornaBase64DoRelatorio()
        {
            // Arrange
            var query = new ObterRelatorioProspostaLaudaPublicacaoQuery(1);

            _mocker.GetMock<IServicoRelatorio>()
                .Setup(m => m.ObterRelatorioPropostaLaudaDePublicacao(1))
                .ReturnsAsync("relatorioBase64");

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().Be("relatorioBase64");
            _mocker.GetMock<IServicoRelatorio>().Verify(m => m.ObterRelatorioPropostaLaudaDePublicacao(1), Times.Once);
        }
    }
}
