using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Relatorios
{
    public class CasoDeUsoObterRelatorioLaudaCompletaDocxTeste
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoObterRelatorioLaudaCompletaDocx _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterRelatorioLaudaCompletaDocxTeste()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CasoDeUsoObterRelatorioLaudaCompletaDocx>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaComDadosValidos_QuandoExecutarAsync_EntaoRetornaBytesDocx()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var dadosProposta = new PropostaLaudaCompletaDto();
            var bytesEsperados = new byte[] { 1, 2, 3 };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterDadosLaudaCompletaAsync(propostaId))
                .ReturnsAsync(dadosProposta);

            _mocker.GetMock<IGeradorLaudaDocxService>()
                .Setup(g => g.GerarArquivoLaudaCompletaAsync(dadosProposta))
                .ReturnsAsync(bytesEsperados);

            // Act
            var result = await _sut.ExecutarAsync(propostaId);

            // Assert
            result.Should().BeEquivalentTo(bytesEsperados);
            _mocker.GetMock<IRepositorioProposta>().Verify(r => r.ObterDadosLaudaCompletaAsync(propostaId), Times.Once);
            _mocker.GetMock<IGeradorLaudaDocxService>().Verify(g => g.GerarArquivoLaudaCompletaAsync(dadosProposta), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoExecutarAsync_EntaoLancaNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterDadosLaudaCompletaAsync(propostaId))
                .ReturnsAsync((PropostaLaudaCompletaDto?)null);

            // Act
            Func<Task> act = async () => await _sut.ExecutarAsync(propostaId);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage("Dados da proposta n*o encontrados para gera**o da lauda.");
            
            _mocker.GetMock<IGeradorLaudaDocxService>().Verify(g => g.GerarArquivoLaudaCompletaAsync(It.IsAny<PropostaLaudaCompletaDto>()), Times.Never);
        }
    }
}
