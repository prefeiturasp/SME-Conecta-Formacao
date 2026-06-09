using AutoMapper;
using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterInscricaoProximaPaginadaQueryHandlerTestes
    {
        private readonly Mock<IRepositorioInscricao> _repositorioMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObterInscricaoProximaPaginadaQueryHandler _handler;

        public ObterInscricaoProximaPaginadaQueryHandlerTestes()
        {
            _repositorioMock = new Mock<IRepositorioInscricao>();
            _mapperMock = new Mock<IMapper>();

            _handler = new ObterInscricaoProximaPaginadaQueryHandler(
                _repositorioMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DeveRetornarVazio_QuandoNaoExistiremRegistros()
        {
            // Arrange
            var filtro = new InscricaoProximaFiltro
            {
                NomeFormacao = "Formação Teste"
            };

            var query = new ObterInscricaoProximaPaginadaQuery(
                usuarioId: 1,
                numeroPagina: 1,
                numeroRegistros: 10,
                filtro: filtro);

            _repositorioMock
                .Setup(r => r.ObterTotalRegistrosPorInscricoesProximas(1, filtro))
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
                r.ObterDadosPaginadosPorInscricoesProximas(
                    It.IsAny<long>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<InscricaoProximaFiltro>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_DeveRetornarRegistrosComCargoFuncao_QuandoExistiremRegistros()
        {
            // Arrange
            var usuarioId = 10L;

            var filtro = new InscricaoProximaFiltro
            {
                CodigoFormacao = 200
            };

            var query = new ObterInscricaoProximaPaginadaQuery(
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
            new InscricaoPaginadaDTO { Id = 99 }
        };

            var cargoFuncao = new CargoFuncaoDTO
            {
                CargoFuncaoCodigo = "CF999",
                CargoFuncaoNome = "Coordenador",
                TipoVinculo = 1
            };

            _repositorioMock
                .Setup(r => r.ObterTotalRegistrosPorInscricoesProximas(usuarioId, filtro))
                .ReturnsAsync(1);

            _repositorioMock
                .Setup(r => r.ObterDadosPaginadosPorInscricoesProximas(
                    usuarioId,
                    query.NumeroPagina,
                    query.NumeroRegistros,
                    filtro))
                .ReturnsAsync(inscricoes);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<InscricaoPaginadaDTO>>(inscricoes))
                .Returns(dtoList);

            _repositorioMock
                .Setup(r => r.ObterCargoFuncaoPorId(99))
                .ReturnsAsync(cargoFuncao);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalRegistros.Should().Be(1);
            resultado.Items.Should().HaveCount(1);

            var item = resultado.Items.First();
            item.CargoFuncaoCodigo.Should().Be("CF999");
            item.CargoFuncao.Should().Be("Coordenador");
            item.TipoVinculo.Should().Be(1);

            _repositorioMock.Verify(r => r.ObterCargoFuncaoPorId(99), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveChamarCargoFuncaoParaCadaItem()
        {
            // Arrange
            var filtro = new InscricaoProximaFiltro();

            var query = new ObterInscricaoProximaPaginadaQuery(
                usuarioId: 5,
                numeroPagina: 1,
                numeroRegistros: 10,
                filtro);

            var inscricoes = new List<Inscricao>
        {
            new Inscricao(),
            new Inscricao(),
            new Inscricao()
        };

            var dtoList = new List<InscricaoPaginadaDTO>
        {
            new InscricaoPaginadaDTO { Id = 1 },
            new InscricaoPaginadaDTO { Id = 2 },
            new InscricaoPaginadaDTO { Id = 3 }
        };

            _repositorioMock
                .Setup(r => r.ObterTotalRegistrosPorInscricoesProximas(5, filtro))
                .ReturnsAsync(3);

            _repositorioMock
                .Setup(r => r.ObterDadosPaginadosPorInscricoesProximas(5, 1, 10, filtro))
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
                    TipoVinculo = 2
                });

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositorioMock.Verify(
                r => r.ObterCargoFuncaoPorId(It.IsAny<long>()),
                Times.Exactly(3));
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoRepositorioForNulo()
        {
            Action act = () => new ObterInscricaoProximaPaginadaQueryHandler(null, _mapperMock.Object);

            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("repositorioInscricao");
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoMapperForNulo()
        {
            Action act = () => new ObterInscricaoProximaPaginadaQueryHandler(_repositorioMock.Object, null);

            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("mapper");
        }
    }
}