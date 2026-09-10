using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterPropostaPaginada;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EntidadeAreaPromotora = SME.ConectaFormacao.Dominio.Entidades.AreaPromotora;
using EntidadeProposta = SME.ConectaFormacao.Dominio.Entidades.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaPaginacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterPropostaPaginacao _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterPropostaPaginacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroPagina")).Returns("1");
            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroRegistros")).Returns("10");

            _sut = mocker.CreateInstance<CasoDeUsoObterPropostaPaginacao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUsuarioLogadoComAreaPromotora_QuandoExecutar_EntaoDeveEnviarQueryComAreaPromotoraERetornarResultadoPaginado()
        {
            // Arrange
            var filtros = new PropostaFiltrosDTO
            {
                NomeFormacao = _faker.Random.Word(),
                NumeroHomologacao = _faker.Random.Long(1, 1000)
            };

            var areaPromotora = new EntidadeAreaPromotora
            {
                Id = _faker.Random.Long(1, 100),
                Nome = _faker.Company.CompanyName()
            };

            var listaPropostas = new List<PropostaPaginadaDTO>
            {
                new()
                {
                    Id = _faker.Random.Long(1, 1000),
                    NomeFormacao = _faker.Random.Word(),
                    AreaPromotora = areaPromotora.Nome,
                    Formato = _faker.Random.Word(),
                    TipoFormacao = _faker.Random.Word()
                }
            };

            var resultadoEsperado = new PaginacaoResultadoDto<PropostaPaginadaDTO>(
                items: listaPropostas,
                totalRegistros: 1,
                numeroRegistros: 10
            );

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterPropostaPaginadaQuery>(q =>
                        q.PropostaFiltrosDTO == filtros &&
                        q.NumeroPagina == _sut.NumeroPagina &&
                        q.NumeroRegistros == _sut.NumeroRegistros &&
                        q.AreaPromotoraIdUsuarioLogado == areaPromotora.Id),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(filtros);

            // Assert
            resultado.Should().BeEquivalentTo(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaPaginadaQuery>(q =>
                    q.PropostaFiltrosDTO == filtros &&
                    q.NumeroPagina == _sut.NumeroPagina &&
                    q.NumeroRegistros == _sut.NumeroRegistros &&
                    q.AreaPromotoraIdUsuarioLogado == areaPromotora.Id),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioLogadoSemAreaPromotora_QuandoExecutar_EntaoDeveEnviarQueryComAreaPromotoraNulaERetornarResultadoPaginado()
        {
            // Arrange
            var filtros = new PropostaFiltrosDTO
            {
                NomeFormacao = _faker.Random.Word()
            };

            var resultadoEsperado = new PaginacaoResultadoDto<PropostaPaginadaDTO>(
                items: new List<PropostaPaginadaDTO>(),
                totalRegistros: 0,
                numeroRegistros: 10
            );

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntidadeAreaPromotora?)null);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterPropostaPaginadaQuery>(q =>
                        q.PropostaFiltrosDTO == filtros &&
                        q.NumeroPagina == _sut.NumeroPagina &&
                        q.NumeroRegistros == _sut.NumeroRegistros &&
                        q.AreaPromotoraIdUsuarioLogado == null),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(filtros);

            // Assert
            resultado.Should().BeEquivalentTo(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaPaginadaQuery>(q =>
                    q.PropostaFiltrosDTO == filtros &&
                    q.NumeroPagina == _sut.NumeroPagina &&
                    q.NumeroRegistros == _sut.NumeroRegistros &&
                    q.AreaPromotoraIdUsuarioLogado == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoObterAreaPromotoraUsuarioLogado_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var filtros = new PropostaFiltrosDTO();
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            Func<Task> act = async () => await _sut.Executar(filtros);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterPropostaPaginadaQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoErroAoObterPropostasPaginadas_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var filtros = new PropostaFiltrosDTO();
            var areaPromotora = new EntidadeAreaPromotora
            {
                Id = _faker.Random.Long(1, 100),
                Nome = _faker.Company.CompanyName()
            };
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPaginadaQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            Func<Task> act = async () => await _sut.Executar(filtros);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaPaginadaQuery>(q =>
                    q.PropostaFiltrosDTO == filtros &&
                    q.NumeroPagina == _sut.NumeroPagina &&
                    q.NumeroRegistros == _sut.NumeroRegistros &&
                    q.AreaPromotoraIdUsuarioLogado == areaPromotora.Id),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
