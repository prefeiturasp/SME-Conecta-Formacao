using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterDeclaracaoCodafParaDownloadTestes
    {
        private readonly Mock<IRepositorioCodafDeclaracao> _repositorioCodafDeclaracaoMock;
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly CasoDeUsoObterDeclaracaoCodafParaDownload _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterDeclaracaoCodafParaDownloadTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafDeclaracaoMock = mocker.GetMock<IRepositorioCodafDeclaracao>();
            _servicoArmazenamentoMock = mocker.GetMock<IServicoArmazenamento>();
            _sut = mocker.CreateInstance<CasoDeUsoObterDeclaracaoCodafParaDownload>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoDeclaracaoNaoEncontrada_QuandoChamarExecutar_EntaoRetornaErroNaoEncontrado()
        {
            // Arrange
            var declaracaoCodafId = _faker.Random.Long(1, 1000);

            _repositorioCodafDeclaracaoMock
                .Setup(r => r.ObterDeclaracaoDisponivelDoUsuarioAsync(declaracaoCodafId))
                .ReturnsAsync((DadosDeclaracaoUsuarioParaDownloadDto?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(declaracaoCodafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().Contain("Declaração CODAF não encontrada para o ID informado.");
        }

        [Fact]
        public async Task DadoDeclaracaoSemChaveObjeto_QuandoChamarExecutar_EntaoRetornaErroValidacao()
        {
            // Arrange
            var declaracaoCodafId = _faker.Random.Long(1, 1000);
            var declaracao = new DadosDeclaracaoUsuarioParaDownloadDto
            {
                Id = declaracaoCodafId,
                ChaveObjetoArmazenamento = string.Empty,
                NomeCompleto = "Usuário Teste",
                NomeFormacao = "Formação Teste"
            };

            _repositorioCodafDeclaracaoMock
                .Setup(r => r.ObterDeclaracaoDisponivelDoUsuarioAsync(declaracaoCodafId))
                .ReturnsAsync(declaracao);

            // Act
            var resultado = await _sut.ExecutarAsync(declaracaoCodafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("Declaração CODAF não possui arquivo associado para download.");
        }

        [Fact]
        public async Task DadoUrlArmazenamentoNaoEncontrada_QuandoChamarExecutar_EntaoRetornaErroNaoEncontrado()
        {
            // Arrange
            var declaracaoCodafId = _faker.Random.Long(1, 1000);
            var chaveObjeto = _faker.Random.AlphaNumeric(10);
            var declaracao = new DadosDeclaracaoUsuarioParaDownloadDto
            {
                Id = declaracaoCodafId,
                ChaveObjetoArmazenamento = chaveObjeto,
                NomeCompleto = "Usuário Teste",
                NomeFormacao = "Formação Teste"
            };

            _repositorioCodafDeclaracaoMock
                .Setup(r => r.ObterDeclaracaoDisponivelDoUsuarioAsync(declaracaoCodafId))
                .ReturnsAsync(declaracao);

            _servicoArmazenamentoMock
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(chaveObjeto))
                .ReturnsAsync((string?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(declaracaoCodafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().Contain("Não foi possível obter o arquivo da declaração CODAF.");
        }

        [Fact]
        public async Task DadoDeclaracaoEUrlValidas_QuandoChamarExecutar_EntaoRetornaDtoComDadosDownload()
        {
            // Arrange
            var declaracaoCodafId = _faker.Random.Long(1, 1000);
            var chaveObjeto = _faker.Random.AlphaNumeric(10);
            var urlArquivo = _faker.Internet.Url();
            var declaracao = new DadosDeclaracaoUsuarioParaDownloadDto
            {
                Id = declaracaoCodafId,
                ChaveObjetoArmazenamento = chaveObjeto,
                NomeCompleto = "Usuário Teste",
                NomeFormacao = "Formação Teste"
            };

            _repositorioCodafDeclaracaoMock
                .Setup(r => r.ObterDeclaracaoDisponivelDoUsuarioAsync(declaracaoCodafId))
                .ReturnsAsync(declaracao);

            _servicoArmazenamentoMock
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(chaveObjeto))
                .ReturnsAsync(urlArquivo);

            // Act
            var resultado = await _sut.ExecutarAsync(declaracaoCodafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Id.Should().Be(declaracaoCodafId);
            resultado.Dados.UrlDownload.Should().Be(urlArquivo);
            resultado.Dados.NomeCompleto.Should().Be(declaracao.NomeCompleto);
            resultado.Dados.NomeFormacao.Should().Be(declaracao.NomeFormacao);
        }
    }
}
