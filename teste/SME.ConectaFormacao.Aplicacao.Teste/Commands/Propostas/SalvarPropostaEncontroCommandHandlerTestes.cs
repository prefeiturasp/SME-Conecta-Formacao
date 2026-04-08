using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaEncontro;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class SalvarPropostaEncontroCommandHandlerTestes
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IRepositorioPropostaEncontro> _repositorioPropostaEncontro;
        private readonly Mock<ITransacao> _transacao;
        private readonly Mock<ICacheDistribuido> _cacheDistribuido;
        private readonly Mock<IDbTransaction> _dbTransactionMock;

        private readonly SalvarPropostaEncontroCommandHandler _sut;
        private readonly Faker _faker;

        public SalvarPropostaEncontroCommandHandlerTestes()
        {
            var mocker = new AutoMocker();

            _mapper = mocker.GetMock<IMapper>();
            _repositorioPropostaEncontro = mocker.GetMock<IRepositorioPropostaEncontro>();
            _transacao = mocker.GetMock<ITransacao>();
            _cacheDistribuido = mocker.GetMock<ICacheDistribuido>();
            _dbTransactionMock = new Mock<IDbTransaction>();

            _sut = mocker.CreateInstance<SalvarPropostaEncontroCommandHandler>();
            _faker = new Faker("pt_BR");

            _transacao.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);
        }

        [Fact]
        public async Task DadoEncontroNovo_QuandoHandle_EntaoDeveInserir()
        {
            // Arrange
            var comando = GerarComandoValido();
            var encontroDepois = GerarPropostaEncontroMock(comando.EncontroDto.Id, comando.PropostaId);

            _mapper
                .Setup(m => m.Map<PropostaEncontro>(comando.EncontroDto))
                .Returns(encontroDepois);

            _repositorioPropostaEncontro
                .Setup(r => r.ObterEncontroTurmasPorEncontroIdAsync(It.IsAny<long>()))
                .ReturnsAsync([]);

            _repositorioPropostaEncontro
                .Setup(r => r.ObterEncontroDatasPorEncontroIdAsync(It.IsAny<long>()))
                .ReturnsAsync([]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().Be(encontroDepois.Id);

            _repositorioPropostaEncontro.Verify(r => r.InserirEncontroAsync(comando.PropostaId, encontroDepois), Times.Once);
            _repositorioPropostaEncontro.Verify(r => r.AtualizarEncontroAsync(It.IsAny<PropostaEncontro>()), Times.Never);

            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);

            _cacheDistribuido.Verify(c => c.RemoverAsync(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task DadoEncontroExistenteComAlteracoes_QuandoHandle_EntaoDeveAtualizar()
        {
            // Arrange
            var comando = GerarComandoValido();
            var encontroAntes = GerarPropostaEncontroMock(comando.EncontroDto.Id, comando.PropostaId);
            encontroAntes.HoraInicio = "08:00";

            var encontroDepois = GerarPropostaEncontroMock(comando.EncontroDto.Id, comando.PropostaId);
            encontroDepois.HoraInicio = "09:00";

            var turmaAntes = new PropostaEncontroTurma { TurmaId = 1 };
            var dataAntes = new PropostaEncontroData { Id = 1, DataInicio = DateTime.Now.AddDays(-1) };

            encontroDepois.Turmas = [new PropostaEncontroTurma { TurmaId = 2 }];
            encontroDepois.Datas = [new PropostaEncontroData { Id = 1, DataInicio = DateTime.Now }];

            _repositorioPropostaEncontro
                .Setup(r => r.ObterEncontroPorIdAsync(comando.EncontroDto.Id))
                .ReturnsAsync(encontroAntes);

            _mapper
                .Setup(m => m.Map<PropostaEncontro>(comando.EncontroDto))
                .Returns(encontroDepois);

            _repositorioPropostaEncontro
                .Setup(r => r.ObterEncontroTurmasPorEncontroIdAsync(It.IsAny<long>()))
                .ReturnsAsync([turmaAntes]);

            _repositorioPropostaEncontro
                .Setup(r => r.ObterEncontroDatasPorEncontroIdAsync(It.IsAny<long>()))
                .ReturnsAsync([dataAntes]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().Be(encontroDepois.Id);

            _repositorioPropostaEncontro.Verify(r => r.AtualizarEncontroAsync(encontroDepois), Times.Once);
            _repositorioPropostaEncontro.Verify(r => r.InserirEncontroTurmasAsync(encontroDepois.Id, It.Is<IEnumerable<PropostaEncontroTurma>>(t => t.Any(x => x.TurmaId == 2))), Times.Once);
            _repositorioPropostaEncontro.Verify(r => r.RemoverEncontroTurmasAsync(It.Is<IEnumerable<PropostaEncontroTurma>>(t => t.Any(x => x.TurmaId == 1))), Times.Once);
            _repositorioPropostaEncontro.Verify(r => r.AtualizarEncontroDataAsync(It.Is<PropostaEncontroData>(d => d.Id == 1)), Times.Once);

            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoProcessamento_QuandoHandle_EntaoDeveFazerRollback()
        {
            // Arrange
            var comando = GerarComandoValido();
            var encontroDepois = GerarPropostaEncontroMock(comando.EncontroDto.Id, comando.PropostaId);

            _mapper
                .Setup(m => m.Map<PropostaEncontro>(comando.EncontroDto))
                .Returns(encontroDepois);

            _repositorioPropostaEncontro
                .Setup(r => r.ObterEncontroTurmasPorEncontroIdAsync(It.IsAny<long>()))
                .ThrowsAsync(new Exception("Erro forçado na transação"));

            // Act
            await Assert.ThrowsAsync<Exception>(async () => await _sut.Handle(comando, CancellationToken.None));

            // Assert
            _dbTransactionMock.Verify(t => t.Commit(), Times.Never);
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Once);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);
        }

        #region Métodos Privados Auxiliares

        private SalvarPropostaEncontroCommand GerarComandoValido()
        {
            var encontroDto = new PropostaEncontroDto
            {
                Id = _faker.Random.Long(1, 100),
                HoraInicio = "10:00",
                HoraFim = "12:00",
                Local = _faker.Address.StreetAddress()
            };

            return new SalvarPropostaEncontroCommand(_faker.Random.Long(1, 50), encontroDto);
        }

        private PropostaEncontro GerarPropostaEncontroMock(long id, long propostaId) => new()
            {
                Id = id,
                PropostaId = propostaId,
                HoraInicio = "10:00",
                HoraFim = "12:00",
                Local = _faker.Address.StreetAddress(),
                Turmas = [],
                Datas = []
            };

        #endregion
    }
}
