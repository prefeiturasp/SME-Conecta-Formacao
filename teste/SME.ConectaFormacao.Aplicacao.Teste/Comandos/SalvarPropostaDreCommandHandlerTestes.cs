using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaDre;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos
{
    public class SalvarPropostaDreCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly SalvarPropostaDreCommandHandler _sut;

        public SalvarPropostaDreCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<SalvarPropostaDreCommandHandler>();
        }

        [Fact]
        public async Task DadoDresInexistentesAnteriormente_QuandoExecutar_EntaoDeveInserir()
        {
            // Arrange
            var dresDto = new List<PropostaDre>
            {
                new() { DreId = 1 },
                new() { DreId = 2 }
            };
            var comando = new SalvarPropostaDreCommand(1, dresDto);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterDrePorId(1))
                .ReturnsAsync([]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.InserirDres(1, It.Is<IEnumerable<PropostaDre>>(x => x.Count() == 2)), Times.Once);
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.RemoverDres(It.IsAny<IEnumerable<PropostaDre>>()), Times.Never);
        }

        [Fact]
        public async Task DadoDresRemovidasNoDto_QuandoExecutar_EntaoDeveExcluir()
        {
            // Arrange
            var dresDto = new List<PropostaDre>(); // Empty DTOs => delete all
            var comando = new SalvarPropostaDreCommand(1, dresDto);

            var dresBanco = new List<PropostaDre>
            {
                new() { Id = 10, DreId = 1 },
                new() { Id = 20, DreId = 2 }
            };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterDrePorId(1))
                .ReturnsAsync(dresBanco);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.InserirDres(It.IsAny<long>(), It.IsAny<IEnumerable<PropostaDre>>()), Times.Never);
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.RemoverDres(It.Is<IEnumerable<PropostaDre>>(x => x.Count() == 2)), Times.Once);
        }

        [Fact]
        public async Task DadoDresMisturadas_QuandoExecutar_EntaoDeveInserirEExcluirCorretamente()
        {
            // Arrange
            var dresDto = new List<PropostaDre>
            {
                new() { DreId = 2 }, // Existe
                new() { DreId = 3 }  // Novo
            };
            var comando = new SalvarPropostaDreCommand(1, dresDto);

            var dresBanco = new List<PropostaDre>
            {
                new() { Id = 10, DreId = 1 }, // Excluido
                new() { Id = 20, DreId = 2 }  // Mantido
            };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterDrePorId(1))
                .ReturnsAsync(dresBanco);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.InserirDres(1, It.Is<IEnumerable<PropostaDre>>(x => x.Single().DreId == 3)), Times.Once);
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.RemoverDres(It.Is<IEnumerable<PropostaDre>>(x => x.Single().DreId == 1)), Times.Once);
        }
    }
}
