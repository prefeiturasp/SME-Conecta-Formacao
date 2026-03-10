using AutoMapper;
using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas;

public class ObterInscricaoFinalizadaPaginadaQueryHandlerTests
{
    private readonly Mock<IRepositorioInscricao> _repositorioMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ObterInscricaoFinalizadaPaginadaQueryHandler _handler;

    public ObterInscricaoFinalizadaPaginadaQueryHandlerTests()
    {
        _repositorioMock = new Mock<IRepositorioInscricao>();
        _mapperMock = new Mock<IMapper>();

        _handler = new ObterInscricaoFinalizadaPaginadaQueryHandler(
            _repositorioMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarVazio_QuandoNaoExistiremRegistros()
    {
        // Arrange
        var filtro = new InscricaoFinalizadaFiltro
        {
            NomeFormacao = "Teste"
        };

        var query = new ObterInscricaoFinalizadaPaginadaQuery(
            usuarioId: 10,
            numeroPagina: 1,
            numeroRegistros: 10,
            filtro: filtro);

        _repositorioMock
            .Setup(r => r.ObterTotalRegistrosPorInscricoesFinalizadas(10, filtro))
            .ReturnsAsync(0);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<InscricaoPaginadaDTO>>(
                It.IsAny<IEnumerable<Inscricao>>()))
            .Returns(Enumerable.Empty<InscricaoPaginadaDTO>());

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TotalRegistros.Should().Be(0);
        resultado.Items.Should().BeEmpty();

        _repositorioMock.Verify(r =>
            r.ObterDadosPaginadosPorInscricoesFinalizadas(
                It.IsAny<long>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<InscricaoFinalizadaFiltro>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DevePreencherCargoFuncao_QuandoExistiremRegistros()
    {
        // Arrange
        var usuarioId = 20L;

        var filtro = new InscricaoFinalizadaFiltro
        {
            SituacaoInscricao = 1
        };

        var query = new ObterInscricaoFinalizadaPaginadaQuery(
            usuarioId,
            numeroPagina: 1,
            numeroRegistros: 10,
            filtro);

        var inscricoes = new List<Inscricao>
        {
            new Inscricao()
        };

        var dtoList = new List<InscricaoPaginadaDTO>
        {
            new InscricaoPaginadaDTO { Id = 100 }
        };

        var cargoFuncao = new CargoFuncaoDTO
        {
            CargoFuncaoCodigo = "CF001",
            CargoFuncaoNome = "Analista de Sistemas",
            TipoVinculo = 2
        };

        _repositorioMock
            .Setup(r => r.ObterTotalRegistrosPorInscricoesFinalizadas(usuarioId, filtro))
            .ReturnsAsync(1);

        _repositorioMock
            .Setup(r => r.ObterDadosPaginadosPorInscricoesFinalizadas(
                usuarioId,
                query.NumeroPagina,
                query.NumeroRegistros,
                filtro))
            .ReturnsAsync(inscricoes);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<InscricaoPaginadaDTO>>(inscricoes))
            .Returns(dtoList);

        _repositorioMock
            .Setup(r => r.ObterCargoFuncaoPorId(100))
            .ReturnsAsync(cargoFuncao);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TotalRegistros.Should().Be(1);
        resultado.Items.Should().HaveCount(1);

        var item = resultado.Items.First();

        item.CargoFuncaoCodigo.Should().Be("CF001");
        item.CargoFuncao.Should().Be("Analista de Sistemas");
        item.TipoVinculo.Should().Be(2);

        _repositorioMock.Verify(r => r.ObterCargoFuncaoPorId(100), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveChamarCargoFuncaoParaCadaItem()
    {
        // Arrange
        var filtro = new InscricaoFinalizadaFiltro();

        var query = new ObterInscricaoFinalizadaPaginadaQuery(
            1,
            1,
            10,
            filtro);

        var inscricoes = new List<Inscricao>
        {
            new Inscricao(),
            new Inscricao()
        };

        var dtoList = new List<InscricaoPaginadaDTO>
        {
            new InscricaoPaginadaDTO { Id = 1 },
            new InscricaoPaginadaDTO { Id = 2 }
        };

        _repositorioMock
            .Setup(r => r.ObterTotalRegistrosPorInscricoesFinalizadas(1, filtro))
            .ReturnsAsync(2);

        _repositorioMock
            .Setup(r => r.ObterDadosPaginadosPorInscricoesFinalizadas(1, 1, 10, filtro))
            .ReturnsAsync(inscricoes);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<InscricaoPaginadaDTO>>(inscricoes))
            .Returns(dtoList);

        _repositorioMock
            .Setup(r => r.ObterCargoFuncaoPorId(It.IsAny<long>()))
            .ReturnsAsync(new CargoFuncaoDTO
            {
                CargoFuncaoCodigo = "X",
                CargoFuncaoNome = "Teste",
                TipoVinculo = 1
            });

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositorioMock.Verify(r => r.ObterCargoFuncaoPorId(It.IsAny<long>()), Times.Exactly(2));
    }
}