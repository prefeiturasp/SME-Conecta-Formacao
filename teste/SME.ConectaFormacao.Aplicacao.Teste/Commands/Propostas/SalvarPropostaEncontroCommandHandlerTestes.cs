using AutoMapper;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaEncontro;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class SalvarPropostaEncontroCommandHandlerTestes
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IRepositorioProposta> _repositorioProposta;
        private readonly Mock<ITransacao> _transacao;
        private readonly Mock<ICacheDistribuido> _cacheDistribuido;
        private readonly SalvarPropostaEncontroCommandHandler _sut;

        public SalvarPropostaEncontroCommandHandlerTestes()
        {
            var mocker = new AutoMocker();

            _mapper = mocker.GetMock<IMapper>();
            _repositorioProposta = mocker.GetMock<IRepositorioProposta>();
            _transacao = mocker.GetMock<ITransacao>();
            _cacheDistribuido = mocker.GetMock<ICacheDistribuido>();

            _sut = mocker.CreateInstance<SalvarPropostaEncontroCommandHandler>();
        }

        [Fact]
        public void DadoMapperNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IMapper mapperNulo = null!;

            // Act
            var act = () => new SalvarPropostaEncontroCommandHandler(
                mapperNulo,
                _repositorioProposta.Object,
                _transacao.Object,
                _cacheDistribuido.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("mapper");
        }

        [Fact]
        public void DadoRepositorioPropostaNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IRepositorioProposta repositorioNulo = null!;

            // Act
            var act = () => new SalvarPropostaEncontroCommandHandler(
                _mapper.Object,
                repositorioNulo,
                _transacao.Object,
                _cacheDistribuido.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("repositorioProposta");
        }

        [Fact]
        public void DadoTransacaoNula_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            ITransacao transacaoNula = null!;

            // Act
            var act = () => new SalvarPropostaEncontroCommandHandler(
                _mapper.Object,
                _repositorioProposta.Object,
                transacaoNula,
                _cacheDistribuido.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("transacao");
        }

        [Fact]
        public void DadoCacheDistribuidoNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            ICacheDistribuido cacheNulo = null!;

            // Act
            var act = () => new SalvarPropostaEncontroCommandHandler(
                _mapper.Object,
                _repositorioProposta.Object,
                _transacao.Object,
                cacheNulo);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("cacheDistribuido");
        }

        [Fact]
        public async Task DadoNovoEncontro_QuandoProcessarComando_EntaoDeveInserirEncontroEListasRealizandoCommit()
        {
            // Arrange
            var comando = CriarComandoValido();
            var encontroDepois = CriarPropostaEncontroMap(comando.EncontroDto.Id);
            encontroDepois.Turmas = [new() { Id = 1 }];
            encontroDepois.Datas = [new() { Id = 1 }];

            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            _repositorioProposta.Setup(r => r.ObterEncontroPorId(It.IsAny<long>())).ReturnsAsync((PropostaEncontro)null!);
            _mapper.Setup(m => m.Map<PropostaEncontro>(comando.EncontroDto)).Returns(encontroDepois);

            ConfigurarRetornoListasVazias(encontroDepois.Id);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().Be(encontroDepois.Id);

            _repositorioProposta.Verify(r => r.InserirEncontro(comando.PropostaId, encontroDepois), Times.Once);
            _repositorioProposta.Verify(r => r.AtualizarEncontro(It.IsAny<PropostaEncontro>()), Times.Never);
            _repositorioProposta.Verify(r => r.InserirEncontroTurmas(encontroDepois.Id, It.Is<IEnumerable<PropostaEncontroTurma>>(t => t.Count() == 1)), Times.Once);
            _repositorioProposta.Verify(r => r.InserirEncontroDatas(encontroDepois.Id, It.Is<IEnumerable<PropostaEncontroData>>(d => d.Count() == 1)), Times.Once);

            _cacheDistribuido.Verify(c => c.RemoverAsync(CacheDistribuidoNomes.FormacaoDetalhada.Parametros(comando.PropostaId)), Times.Once);

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DadoEncontroExistenteComMudancasNoCabecalho_QuandoProcessarComando_EntaoDeveAtualizarEncontro()
        {
            // Arrange
            var comando = CriarComandoValido();
            var encontroAntes = CriarPropostaEncontroMap(comando.EncontroDto.Id);
            encontroAntes.HoraInicio = "10:00";

            var encontroDepois = CriarPropostaEncontroMap(comando.EncontroDto.Id);
            encontroDepois.HoraInicio = "11:00";

            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            _repositorioProposta.Setup(r => r.ObterEncontroPorId(comando.EncontroDto.Id)).ReturnsAsync(encontroAntes);
            _mapper.Setup(m => m.Map<PropostaEncontro>(comando.EncontroDto)).Returns(encontroDepois);

            ConfigurarRetornoListasVazias(encontroDepois.Id);

            // Act
            await _sut.Handle(comando, CancellationToken.None);

            // Assert
            _repositorioProposta.Verify(r => r.AtualizarEncontro(encontroDepois), Times.Once);
            _repositorioProposta.Verify(r => r.InserirEncontro(It.IsAny<long>(), It.IsAny<PropostaEncontro>()), Times.Never);
            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoEncontroExistenteSemMudancasNoCabecalho_QuandoProcessarComando_EntaoNaoDeveAtualizarEncontro()
        {
            // Arrange
            var comando = CriarComandoValido();
            var encontroAntes = CriarPropostaEncontroMap(comando.EncontroDto.Id);
            var encontroDepois = CriarPropostaEncontroMap(comando.EncontroDto.Id);

            encontroAntes.HoraInicio = "10:00"; encontroDepois.HoraInicio = "10:00";
            encontroAntes.HoraFim = "12:00"; encontroDepois.HoraFim = "12:00";
            encontroAntes.Local = "Sala 1"; encontroDepois.Local = "Sala 1";

            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            _repositorioProposta.Setup(r => r.ObterEncontroPorId(comando.EncontroDto.Id)).ReturnsAsync(encontroAntes);
            _mapper.Setup(m => m.Map<PropostaEncontro>(comando.EncontroDto)).Returns(encontroDepois);

            ConfigurarRetornoListasVazias(encontroDepois.Id);

            // Act
            await _sut.Handle(comando, CancellationToken.None);

            // Assert
            _repositorioProposta.Verify(r => r.AtualizarEncontro(It.IsAny<PropostaEncontro>()), Times.Never);
            _repositorioProposta.Verify(r => r.InserirEncontro(It.IsAny<long>(), It.IsAny<PropostaEncontro>()), Times.Never);
            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoMudancasNasListasDeTurmasEDatas_QuandoProcessarComando_EntaoDeveSincronizarListasCorretamente()
        {
            // Arrange
            var comando = CriarComandoValido();
            var encontroDepois = CriarPropostaEncontroMap(comando.EncontroDto.Id);

            var turmasAntes = new List<PropostaEncontroTurma> { new() { Id = 1, TurmaId = 100 } };
            encontroDepois.Turmas = [new() { Id = 2, TurmaId = 200 }];

            var datasAntes = new List<PropostaEncontroData>
            {
                new() { Id = 10, DataInicio = new(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new() { Id = 20, DataInicio = new(2023, 2, 2, 0, 0, 0, DateTimeKind.Utc) }
            };

            encontroDepois.Datas =
            [
                new() { Id = 20, DataInicio = new(2023, 2, 3, 0, 0, 0, DateTimeKind.Utc) },
                new() { Id = 30, DataInicio = new(2023, 3, 3, 0, 0, 0, DateTimeKind.Utc) }
            ];

            ConfigurarTransacaoComSucesso();

            _repositorioProposta.Setup(r => r.ObterEncontroPorId(comando.EncontroDto.Id)).ReturnsAsync(encontroDepois);
            _mapper.Setup(m => m.Map<PropostaEncontro>(comando.EncontroDto)).Returns(encontroDepois);
            _repositorioProposta.Setup(r => r.ObterEncontroTurmasPorEncontroId(encontroDepois.Id)).ReturnsAsync(turmasAntes);
            _repositorioProposta.Setup(r => r.ObterEncontroDatasPorEncontroId(encontroDepois.Id)).ReturnsAsync(datasAntes);

            // Act
            await _sut.Handle(comando, CancellationToken.None);

            // Assert
            _repositorioProposta.Verify(r => r.InserirEncontroTurmas(encontroDepois.Id, It.Is<IEnumerable<PropostaEncontroTurma>>(t => t.First().Id == 2)), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverEncontroTurmas(It.Is<IEnumerable<PropostaEncontroTurma>>(t => t.First().Id == 1)), Times.Once);

            _repositorioProposta.Verify(r => r.InserirEncontroDatas(encontroDepois.Id, It.Is<IEnumerable<PropostaEncontroData>>(d => d.First().Id == 30)), Times.Once);
            _repositorioProposta.Verify(r => r.AtualizarEncontroData(It.Is<PropostaEncontroData>(d => d.Id == 20)), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverEncontroDatas(It.Is<IEnumerable<PropostaEncontroData>>(d => d.First().Id == 10)), Times.Once);

            _cacheDistribuido.Verify(c => c.RemoverAsync(CacheDistribuidoNomes.PropostaTurmaEncontro.Parametros(100)), Times.Once);
        }

        [Fact]
        public async Task DadoErroDePersistencia_QuandoProcessarComando_EntaoDeveRealizarRollbackEPropagarExcecao()
        {
            // Arrange
            var comando = CriarComandoValido();
            var encontroDepois = CriarPropostaEncontroMap(comando.EncontroDto.Id);
            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            _repositorioProposta.Setup(r => r.ObterEncontroPorId(It.IsAny<long>())).ReturnsAsync((PropostaEncontro)null!);
            _mapper.Setup(m => m.Map<PropostaEncontro>(comando.EncontroDto)).Returns(encontroDepois);

            _repositorioProposta
                .Setup(r => r.InserirEncontro(It.IsAny<long>(), It.IsAny<PropostaEncontro>()))
                .ThrowsAsync(new Exception("Falha de banco de dados"));

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Falha de banco de dados");

            transacaoDbMock.Verify(t => t.Rollback(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
            transacaoDbMock.Verify(t => t.Commit(), Times.Never);
        }

        #region Factory Methods

        private static SalvarPropostaEncontroCommand CriarComandoValido()
        {
            return new SalvarPropostaEncontroCommand(999, new PropostaEncontroDto { Id = 1 });
        }

        private static PropostaEncontro CriarPropostaEncontroMap(long id)
        {
            return new PropostaEncontro
            {
                Id = id,
                HoraInicio = "10:00",
                HoraFim = "12:00",
                Local = "SME - Sala de Reunião",
                Turmas = [],
                Datas = []
            };
        }

        private Mock<IDbTransaction> ConfigurarTransacaoComSucesso()
        {
            var transacaoDbMock = new Mock<IDbTransaction>();
            _transacao.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);
            return transacaoDbMock;
        }

        private void ConfigurarRetornoListasVazias(long encontroId)
        {
            _repositorioProposta.Setup(r => r.ObterEncontroTurmasPorEncontroId(encontroId)).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterEncontroDatasPorEncontroId(encontroId)).ReturnsAsync([]);
        }

        #endregion
    }
}
