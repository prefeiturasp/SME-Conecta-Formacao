using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using EntidadeProposta = SME.ConectaFormacao.Dominio.Entidades.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.PropostaTestes
{
    public class CasoDeUsoObterComunicadoComunicadoAcaoFormativaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterComunicadoComunicadoAcaoFormativa _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterComunicadoComunicadoAcaoFormativaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterComunicadoComunicadoAcaoFormativa>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaIdZero_QuandoExecutar_EntaoDeveRetornarComunicadoParametroSistema()
        {
            // Arrange
            long propostaId = 0;
            var anoAtual = DateTimeExtension.HorarioBrasilia().Year;
            var parametroDescricao = new ParametroSistema { Valor = _faker.Lorem.Sentence() };
            var parametroUrl = new ParametroSistema { Valor = _faker.Internet.Url() };

            _mediatorMock.Setup(m => m.Send(
                It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q => q.TipoParametroSistema == TipoParametroSistema.ComunicadoAcaoFormativaDescricao && q.Ano == anoAtual), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(parametroDescricao);

            _mediatorMock.Setup(m => m.Send(
                It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q => q.TipoParametroSistema == TipoParametroSistema.ComunicadoAcaoFormativaUrl && q.Ano == anoAtual), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(parametroUrl);

            // Act
            var resultado = await _casoDeUso.Executar(propostaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Descricao.Should().Be(parametroDescricao.Valor);
            resultado.Url.Should().Be(parametroUrl.Valor);
            
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaAcaoInformativaVerdadeiro_QuandoExecutar_EntaoDeveRetornarComunicadoDaProposta()
        {
            // Arrange
            long propostaId = _faker.Random.Long(1);
            var proposta = new EntidadeProposta
            {
                AcaoInformativa = true,
                AcaoFormativaTexto = _faker.Lorem.Sentence(),
                AcaoFormativaLink = _faker.Internet.Url()
            };

            _mediatorMock.Setup(m => m.Send(
                It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            var resultado = await _casoDeUso.Executar(propostaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Descricao.Should().Be(proposta.AcaoFormativaTexto);
            resultado.Url.Should().Be(proposta.AcaoFormativaLink);
            
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaAcaoInformativaFalso_QuandoExecutar_EntaoDeveRetornarComunicadoParametroSistema()
        {
            // Arrange
            long propostaId = _faker.Random.Long(1);
            var proposta = new EntidadeProposta
            {
                AcaoInformativa = false
            };
            var anoAtual = DateTimeExtension.HorarioBrasilia().Year;
            var parametroDescricao = new ParametroSistema { Valor = _faker.Lorem.Sentence() };
            var parametroUrl = new ParametroSistema { Valor = _faker.Internet.Url() };

            _mediatorMock.Setup(m => m.Send(
                It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            _mediatorMock.Setup(m => m.Send(
                It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q => q.TipoParametroSistema == TipoParametroSistema.ComunicadoAcaoFormativaDescricao && q.Ano == anoAtual), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(parametroDescricao);

            _mediatorMock.Setup(m => m.Send(
                It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q => q.TipoParametroSistema == TipoParametroSistema.ComunicadoAcaoFormativaUrl && q.Ano == anoAtual), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(parametroUrl);

            // Act
            var resultado = await _casoDeUso.Executar(propostaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Descricao.Should().Be(parametroDescricao.Valor);
            resultado.Url.Should().Be(parametroUrl.Valor);
        }
    }
}
