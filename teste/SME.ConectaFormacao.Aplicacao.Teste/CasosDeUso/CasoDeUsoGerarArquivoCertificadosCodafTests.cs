using Bogus;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarArquivoCertificadosCodafTests
    {
        private readonly Mock<IServicoRelatorio> _mockServicoRelatorio;
        private readonly Mock<IRepositorioCodafCertificado> _mockRepositorioCertificado;
        private readonly Mock<IServicoArmazenamento> _mockServicoArmazenamento;
        private readonly Mock<IKeyedServiceProvider> _mockServiceProvider;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMediator> _mockMediator;
        private readonly CasoDeUsoGerarArquivoCertificadosCodaf _sut;
        private readonly Faker _faker;

        public CasoDeUsoGerarArquivoCertificadosCodafTests()
        {
            var mocker = new AutoMocker();
            _mockServicoRelatorio = mocker.GetMock<IServicoRelatorio>();
            _mockRepositorioCertificado = mocker.GetMock<IRepositorioCodafCertificado>();
            _mockServicoArmazenamento = mocker.GetMock<IServicoArmazenamento>();
            _mockServiceProvider = mocker.GetMock<IKeyedServiceProvider>();
            _mockConfiguration = mocker.GetMock<IConfiguration>();
            _mockMediator = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoGerarArquivoCertificadosCodaf>();
            _faker = new();
        }

        [Fact]
        public async Task DadoQueNaoTemCertificadosParaProcessar_QuandoExecutar_EntaoNaoDeveExecutarProcesso()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["UrlFrontEnd"]).Returns(_faker.Internet.Url());

            // Act
            await _sut.Executar(new());

            // Assert
            _mockRepositorioCertificado.Verify(x => x.ObterCertificadosParaProcessamentoAsync(), Times.Once);
            _mockServicoRelatorio.Verify(x => x.ConveterHtmlCertificadoCodafParaPdfAsync(It.IsAny<HtmlCertificadoCodafDto>()), Times.Never);
        }
    }
}