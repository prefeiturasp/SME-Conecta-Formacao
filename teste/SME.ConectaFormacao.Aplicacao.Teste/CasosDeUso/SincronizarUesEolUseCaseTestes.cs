using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class SincronizarUesEolUseCaseTestes
    {
        private readonly Mock<IServicoEol> _servicoEolMock;
        private readonly Mock<IServicoLogs> _servicoLogsMock;
        private readonly Mock<IRepositorioSincronizador> _repositorioSincronizadorMock;
        private readonly Mock<IRepositorioDre> _repositorioDreMock;
        private readonly SincronizarUesEolUseCase _sut;
        private readonly Faker _faker;

        public SincronizarUesEolUseCaseTestes()
        {
            _faker = new("pt_BR");
            var mocker = new AutoMocker();

            _servicoEolMock = mocker.GetMock<IServicoEol>();
            _servicoLogsMock = mocker.GetMock<IServicoLogs>();
            _repositorioSincronizadorMock = mocker.GetMock<IRepositorioSincronizador>();
            _repositorioDreMock = mocker.GetMock<IRepositorioDre>();
            _sut = mocker.CreateInstance<SincronizarUesEolUseCase>();
        }

        [Fact]
        public async Task Dado_ServicoEolRetornaNulo_Quando_Executar_Entao_DeveLogarAlertaERetornarVerdadeiro()
        {
            // Arrange
            _servicoEolMock.Setup(s => s.ObterTodasAsUesAsync())
                           .ReturnsAsync((IEnumerable<UeEol>?)null);

            // Act
            var resultado = await _sut.Executar(new MensagemRabbit());

            // Assert
            resultado.Should().BeTrue();

            _servicoLogsMock.Verify(l => l.Enviar(
                It.Is<string>(msg => msg.Contains("Nenhuma UE encontrada")),
                LogContexto.SincronizacaoUesEol,
                LogNivel.Alerta,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);

            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteUeEolAsync(It.IsAny<List<Ue>>()), Times.Never);
        }

        [Fact]
        public async Task Dado_UesValidasEDresEncontradas_Quando_Executar_Entao_DeveSincronizarLoteERetornarVerdadeiro()
        {
            // Arrange
            var codigoDre = _faker.Random.Number(10000, 99999).ToString();
            var uesEolOrigem = new List<UeEol>
            {
                new() { CodigoDRE = codigoDre, CodigoEscola = "123", NomeEscola = "Escola A", CodigoTipoEscola = 1, SiglaTipoEscola = "EMEF" },
                new() { CodigoDRE = codigoDre, CodigoEscola = "456", NomeEscola = "Escola B", CodigoTipoEscola = 2, SiglaTipoEscola = "CEI" }
            };

            var dre = new Dre { Id = _faker.Random.Long(1, 100), Codigo = codigoDre };

            _servicoEolMock.Setup(s => s.ObterTodasAsUesAsync())
                           .ReturnsAsync(uesEolOrigem);

            _repositorioDreMock.Setup(r => r.ObterDrePorCodigo(codigoDre))
                               .ReturnsAsync(dre);

            // Act
            var resultado = await _sut.Executar(new MensagemRabbit());

            // Assert
            resultado.Should().BeTrue();

            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteUeEolAsync(
                It.Is<List<Ue>>(lote => lote.Count == 2 && lote.All(u => u.DreId == dre.Id))), Times.Once);
        }

        [Fact]
        public async Task Dado_UeSemDreCorrespondente_Quando_Executar_Entao_DeveLogarAlertaIgnorarUeESincronizarRestante()
        {
            // Arrange
            var codigoDreValida = "DRE_VALIDA";
            var codigoDreInvalida = "DRE_INVALIDA";

            var uesEolOrigem = new List<UeEol>
            {
                new() { CodigoDRE = codigoDreValida, CodigoEscola = "111", NomeEscola = "Escola Valida", SiglaTipoEscola = "EMEF" },
                new() { CodigoDRE = codigoDreInvalida, CodigoEscola = "222", NomeEscola = "Escola Sem Dre", SiglaTipoEscola = "CEI" }
            };

            var dreValida = new Dre { Id = _faker.Random.Long(1, 100), Codigo = codigoDreValida };

            _servicoEolMock.Setup(s => s.ObterTodasAsUesAsync())
                           .ReturnsAsync(uesEolOrigem);

            _repositorioDreMock.Setup(r => r.ObterDrePorCodigo(codigoDreValida))
                               .ReturnsAsync(dreValida);

            _repositorioDreMock.Setup(r => r.ObterDrePorCodigo(codigoDreInvalida))
                               .ReturnsAsync((Dre)null!);

            // Act
            var resultado = await _sut.Executar(new MensagemRabbit());

            // Assert
            resultado.Should().BeTrue();

            _servicoLogsMock.Verify(l => l.Enviar(
                It.Is<string>(msg => msg.Contains(codigoDreInvalida) && msg.Contains("não encontrada")),
                LogContexto.SincronizacaoUesEol,
                LogNivel.Alerta,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);

            _repositorioSincronizadorMock.Verify(r => r.SincronizarLoteUeEolAsync(
                It.Is<List<Ue>>(lote => lote.Count == 1 && lote.First().CodigoUe == "111")), Times.Once);
        }

        [Fact]
        public async Task Dado_ErroInesperado_Quando_Executar_Entao_DeveLogarCriticoERetornarFalso()
        {
            // Arrange
            var mensagemErro = "Erro de conexão com banco de dados";

            _servicoEolMock.Setup(s => s.ObterTodasAsUesAsync())
                           .ThrowsAsync(new Exception(mensagemErro));

            // Act
            var resultado = await _sut.Executar(new MensagemRabbit());

            // Assert
            resultado.Should().BeFalse();

            _servicoLogsMock.Verify(l => l.Enviar(
                It.Is<string>(msg => msg.Contains(mensagemErro)),
                LogContexto.SincronizacaoUesEol,
                LogNivel.Critico,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }
    }
}
