using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.ObterDominioDeEmailPermitido;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    [ExcludeFromCodeCoverage]
    public class ObterDominioDeEmailPermitidoQueryHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ObterDominioDeEmailPermitidoQueryHandler _sut;

        public ObterDominioDeEmailPermitidoQueryHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ObterDominioDeEmailPermitidoQueryHandler>();
        }

        [Fact]
        public async Task DadoConsultaValida_QuandoExecutar_EntaoRetornaDominios()
        {
            // Arrange
            var query = new ObterDominioDeEmailPermitidoQuery();
            var dominiosEsperados = new List<string> { "sme.prefeitura.sp.gov.br", "gmail.com" };

            _mocker.GetMock<IRepositorioParametroSistema>()
                .Setup(m => m.ObterDominiosPermitidosParaUesParceirasAsync())
                .ReturnsAsync(dominiosEsperados);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(dominiosEsperados);
            _mocker.GetMock<IRepositorioParametroSistema>().Verify(m => m.ObterDominiosPermitidosParaUesParceirasAsync(), Times.Once);
        }
    }
}
