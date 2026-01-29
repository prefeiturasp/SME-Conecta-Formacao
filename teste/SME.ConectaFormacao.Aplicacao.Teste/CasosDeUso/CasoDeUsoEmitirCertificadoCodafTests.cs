using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEmitirCertificadoCodafTests
    {
        private readonly Mock<IRepositorioCodafCertificado> _repositorioMock;
        private readonly Mock<IKeyedServiceProvider> _serviceProviderMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEmitirCertificadoCodaf _sut;

        public CasoDeUsoEmitirCertificadoCodafTests()
        {
            _repositorioMock = new Mock<IRepositorioCodafCertificado>();
            _serviceProviderMock = new Mock<IKeyedServiceProvider>();
            _mediatorMock = new Mock<IMediator>();

            _sut = new CasoDeUsoEmitirCertificadoCodaf(
                _repositorioMock.Object,
                _serviceProviderMock.Object,
                _mediatorMock.Object
            );
        }

        [Fact]
        public async Task Dado_NenhumRegistroEncontrado_Quando_Executar_Entao_DeveRetornarFalsoENaoProcessar()
        {
            // Arrange
            long codafId = 123;
            _repositorioMock.Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(codafId))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto>());

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Should().BeFalse();

            // Verifica que NÃO tentou salvar nada
            _repositorioMock.Verify(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()), Times.Never);

            // Verifica que NÃO publicou na fila
            _mediatorMock.Verify(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
