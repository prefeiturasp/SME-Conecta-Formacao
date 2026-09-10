using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EntidadeUsuario = SME.ConectaFormacao.Dominio.Entidades.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterInscricaoFinalizadaPaginadaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterInscricaoFinalizadaPaginada _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterInscricaoFinalizadaPaginadaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            _sut = mocker.CreateInstance<CasoDeUsoObterInscricaoFinalizadaPaginada>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoFiltroPreenchido_QuandoExecutar_EntaoDeveMapearPropriedadesEnviarQueryERetornarPaginacao()
        {
            // Arrange
            var filtroDto = new InscricaoFinalizadaFiltroDTO
            {
                NomeFormacao = _faker.Lorem.Sentence(),
                SituacaoAprovacao = _faker.Random.Int(1, 5),
                SituacaoInscricao = _faker.Random.Int(1, 5),
                DataInicial = _faker.Date.Past(),
                DataFinal = _faker.Date.Future()
            };

            var usuarioLogado = new EntidadeUsuario { Id = _faker.Random.Long(1, 1000) };
            var resultadoEsperado = new PaginacaoResultadoDto<InscricaoPaginadaDTO>(new List<InscricaoPaginadaDTO>(), 0, _sut.NumeroRegistros);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterInscricaoFinalizadaPaginadaQuery>(q =>
                    q.UsuarioId == usuarioLogado.Id &&
                    q.NumeroPagina == _sut.NumeroPagina &&
                    q.NumeroRegistros == _sut.NumeroRegistros &&
                    q.Filtro != null &&
                    q.Filtro.NomeFormacao == filtroDto.NomeFormacao &&
                    q.Filtro.SituacaoAprovacao == filtroDto.SituacaoAprovacao &&
                    q.Filtro.SituacaoInscricao == filtroDto.SituacaoInscricao &&
                    q.Filtro.DataInicial == filtroDto.DataInicial &&
                    q.Filtro.DataFinal == filtroDto.DataFinal
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(filtroDto);

            // Assert
            resultado.Should().BeSameAs(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterInscricaoFinalizadaPaginadaQuery>(q =>
                q.UsuarioId == usuarioLogado.Id &&
                q.NumeroPagina == _sut.NumeroPagina &&
                q.NumeroRegistros == _sut.NumeroRegistros &&
                q.Filtro.NomeFormacao == filtroDto.NomeFormacao &&
                q.Filtro.SituacaoAprovacao == filtroDto.SituacaoAprovacao &&
                q.Filtro.SituacaoInscricao == filtroDto.SituacaoInscricao &&
                q.Filtro.DataInicial == filtroDto.DataInicial &&
                q.Filtro.DataFinal == filtroDto.DataFinal
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroNulo_QuandoExecutar_EntaoDeveEnviarQueryComFiltroVazioSemLancarExcecaoERetornarPaginacao()
        {
            // Arrange
            InscricaoFinalizadaFiltroDTO? filtroDto = null;
            var usuarioLogado = new EntidadeUsuario { Id = _faker.Random.Long(1, 1000) };
            var resultadoEsperado = new PaginacaoResultadoDto<InscricaoPaginadaDTO>(new List<InscricaoPaginadaDTO>(), 0, _sut.NumeroRegistros);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterInscricaoFinalizadaPaginadaQuery>(q =>
                    q.UsuarioId == usuarioLogado.Id &&
                    q.NumeroPagina == _sut.NumeroPagina &&
                    q.NumeroRegistros == _sut.NumeroRegistros &&
                    q.Filtro != null &&
                    q.Filtro.NomeFormacao == null &&
                    q.Filtro.SituacaoAprovacao == null &&
                    q.Filtro.SituacaoInscricao == null &&
                    q.Filtro.DataInicial == null &&
                    q.Filtro.DataFinal == null
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(filtroDto!);

            // Assert
            resultado.Should().BeSameAs(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterInscricaoFinalizadaPaginadaQuery>(q =>
                q.UsuarioId == usuarioLogado.Id &&
                q.NumeroPagina == _sut.NumeroPagina &&
                q.NumeroRegistros == _sut.NumeroRegistros &&
                q.Filtro != null &&
                q.Filtro.NomeFormacao == null &&
                q.Filtro.SituacaoAprovacao == null &&
                q.Filtro.SituacaoInscricao == null &&
                q.Filtro.DataInicial == null &&
                q.Filtro.DataFinal == null
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPaginacaoCustomizada_QuandoExecutar_EntaoDeveEnviarQueryComNumeroPaginaERegistrosConfigurados()
        {
            // Arrange
            var paginaCustomizada = _faker.Random.Int(2, 10);
            var registrosCustomizados = _faker.Random.Int(15, 50);
            _sut.NumeroPagina = paginaCustomizada;
            _sut.NumeroRegistros = registrosCustomizados;

            var filtroDto = new InscricaoFinalizadaFiltroDTO();
            var usuarioLogado = new EntidadeUsuario { Id = _faker.Random.Long(1, 1000) };
            var resultadoEsperado = new PaginacaoResultadoDto<InscricaoPaginadaDTO>(new List<InscricaoPaginadaDTO>(), 0, registrosCustomizados);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterInscricaoFinalizadaPaginadaQuery>(q =>
                    q.UsuarioId == usuarioLogado.Id &&
                    q.NumeroPagina == paginaCustomizada &&
                    q.NumeroRegistros == registrosCustomizados
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(filtroDto);

            // Assert
            resultado.Should().BeSameAs(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterInscricaoFinalizadaPaginadaQuery>(q =>
                q.UsuarioId == usuarioLogado.Id &&
                q.NumeroPagina == paginaCustomizada &&
                q.NumeroRegistros == registrosCustomizados
            ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
