using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EntidadeUsuario = SME.ConectaFormacao.Dominio.Entidades.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterInscricaoPaginadaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterInscricaoPaginada _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterInscricaoPaginadaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            _sut = mocker.CreateInstance<CasoDeUsoObterInscricaoPaginada>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUsuarioLogado_QuandoExecutar_EntaoDeveEnviarQueryComUsuarioIdEPaginacaoERetornarResultado()
        {
            // Arrange
            var usuarioLogado = new EntidadeUsuario { Id = _faker.Random.Long(1, 1000) };
            var resultadoEsperado = new PaginacaoResultadoDto<InscricaoPaginadaDTO>(new List<InscricaoPaginadaDTO>(), 0, _sut.NumeroRegistros);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterInscricaoPaginadaPorUsuarioIdQuery>(q =>
                    q.UsuarioId == usuarioLogado.Id &&
                    q.NumeroPagina == _sut.NumeroPagina &&
                    q.NumeroRegistros == _sut.NumeroRegistros
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().BeSameAs(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterInscricaoPaginadaPorUsuarioIdQuery>(q =>
                q.UsuarioId == usuarioLogado.Id &&
                q.NumeroPagina == _sut.NumeroPagina &&
                q.NumeroRegistros == _sut.NumeroRegistros
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPaginacaoCustomizada_QuandoExecutar_EntaoDeveEnviarQueryComValoresPaginacaoConfigurados()
        {
            // Arrange
            var paginaCustomizada = _faker.Random.Int(2, 10);
            var registrosCustomizados = _faker.Random.Int(15, 50);
            _sut.NumeroPagina = paginaCustomizada;
            _sut.NumeroRegistros = registrosCustomizados;

            var usuarioLogado = new EntidadeUsuario { Id = _faker.Random.Long(1, 1000) };
            var resultadoEsperado = new PaginacaoResultadoDto<InscricaoPaginadaDTO>(new List<InscricaoPaginadaDTO>(), 0, registrosCustomizados);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterInscricaoPaginadaPorUsuarioIdQuery>(q =>
                    q.UsuarioId == usuarioLogado.Id &&
                    q.NumeroPagina == paginaCustomizada &&
                    q.NumeroRegistros == registrosCustomizados
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().BeSameAs(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterInscricaoPaginadaPorUsuarioIdQuery>(q =>
                q.UsuarioId == usuarioLogado.Id &&
                q.NumeroPagina == paginaCustomizada &&
                q.NumeroRegistros == registrosCustomizados
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPaginacaoConfiguradaNoContexto_QuandoCriarSutEExecutar_EntaoDeveUtilizarValoresDoContexto()
        {
            // Arrange
            var localMocker = new AutoMocker();
            var contextoMock = localMocker.GetMock<IContextoAplicacao>();
            var mediatorMock = localMocker.GetMock<IMediator>();

            contextoMock.Setup(c => c.ObterVariavel<string>("NumeroPagina")).Returns("5");
            contextoMock.Setup(c => c.ObterVariavel<string>("NumeroRegistros")).Returns("50");

            var sutLocal = localMocker.CreateInstance<CasoDeUsoObterInscricaoPaginada>();

            var usuarioLogado = new EntidadeUsuario { Id = _faker.Random.Long(1, 1000) };
            var resultadoEsperado = new PaginacaoResultadoDto<InscricaoPaginadaDTO>(new List<InscricaoPaginadaDTO>(), 0, 50);

            mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            mediatorMock
                .Setup(m => m.Send(It.Is<ObterInscricaoPaginadaPorUsuarioIdQuery>(q =>
                    q.UsuarioId == usuarioLogado.Id &&
                    q.NumeroPagina == 5 &&
                    q.NumeroRegistros == 50
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await sutLocal.Executar();

            // Assert
            resultado.Should().BeSameAs(resultadoEsperado);
            sutLocal.NumeroPagina.Should().Be(5);
            sutLocal.NumeroRegistros.Should().Be(50);
            mediatorMock.Verify(m => m.Send(It.Is<ObterInscricaoPaginadaPorUsuarioIdQuery>(q =>
                q.UsuarioId == usuarioLogado.Id &&
                q.NumeroPagina == 5 &&
                q.NumeroRegistros == 50
            ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
