using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEmitirCertificadoCodafTests
    {
        // Mocks das dependências
        private readonly Mock<IRepositorioCodafCertificado> _repositorioMock;
        private readonly Mock<IKeyedServiceProvider> _keyedServiceProviderMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICertificadoCodafGeradorConteudo> _geradorConteudoMock;

        // System Under Test
        private readonly CasoDeUsoEmitirCertificadoCodaf _sut;

        public CasoDeUsoEmitirCertificadoCodafTests()
        {
            _repositorioMock = new Mock<IRepositorioCodafCertificado>();
            _keyedServiceProviderMock = new Mock<IKeyedServiceProvider>();
            _mediatorMock = new Mock<IMediator>();
            _geradorConteudoMock = new Mock<ICertificadoCodafGeradorConteudo>();

            _sut = new CasoDeUsoEmitirCertificadoCodaf(
                _repositorioMock.Object,
                _keyedServiceProviderMock.Object,
                _mediatorMock.Object
            );
        }

        [Fact]
        public async Task Dado_ListaVazia_Quando_Executar_Entao_DeveRetornarErroNaoEncontrado()
        {
            // Arrange
            long idListaPresenca = 123;
            _repositorioMock.Setup(x => x.ObterDadosParaEmissaoCertificadosCodafAsync(idListaPresenca))
                .ReturnsAsync(new List<DadosEmissaoCertificadoCodafDto>());

            // Act
            var resultado = await _sut.ExecutarAsync(idListaPresenca);

            // Assert
            resultado.Should().NotBeNull();
            // Assumindo que sua classe Resultado tem uma propriedade Sucesso ou validação de tipo de erro
            resultado.Sucesso.Should().BeFalse();

            // Verifica que NADA foi salvo ou publicado
            _repositorioMock.Verify(x => x.InserirLoteAsync(It.IsAny<IEnumerable<CodafCertificado>>()), Times.Never);
            _mediatorMock.Verify(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
