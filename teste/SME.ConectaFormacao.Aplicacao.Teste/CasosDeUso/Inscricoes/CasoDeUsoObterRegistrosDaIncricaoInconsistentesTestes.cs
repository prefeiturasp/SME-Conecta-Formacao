using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Consultas.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterRegistrosDaIncricaoInconsistentesTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterRegistrosDaIncricaoInconsistentes _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterRegistrosDaIncricaoInconsistentesTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroPagina")).Returns("1");
            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroRegistros")).Returns("10");

            _sut = mocker.CreateInstance<CasoDeUsoObterRegistrosDaIncricaoInconsistentes>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoArquivoIdValido_QuandoExecutar_EntaoDeveEnviarQueryERetornarPaginacaoComSucesso()
        {
            // Arrange
            var arquivoId = _faker.Random.Long(1, 1000);
            var itens = new List<RegistroDaInscricaoInsconsistenteDto>
            {
                new()
                {
                    Linha = 1,
                    Turma = "Turma 1",
                    Nome = _faker.Person.FullName,
                    CPF = "12345678900",
                    RegistroFuncional = "1234567",
                    ColaboradorRede = "Sim",
                    Erro = "Inconsistência cadastral",
                    Vinculo = "Efetivo"
                }
            };
            var resultadoEsperado = new PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>(
                items: itens,
                totalRegistros: 1,
                numeroRegistros: 10,
                sucesso: true);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRegistrosDaIncricaoInconsistentesQuery>(q =>
                    q.QuantidadeRegistrosIgnorados == _sut.QuantidadeRegistrosIgnorados &&
                    q.NumeroRegistros == _sut.NumeroRegistros &&
                    q.ArquivoId == arquivoId
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(arquivoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(resultadoEsperado);
            resultado.Sucesso.Should().BeTrue();
            resultado.Items.Should().HaveCount(1);

            _mediatorMock.Verify(m => m.Send(It.Is<ObterRegistrosDaIncricaoInconsistentesQuery>(q =>
                q.QuantidadeRegistrosIgnorados == 0 &&
                q.NumeroRegistros == 10 &&
                q.ArquivoId == arquivoId
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPaginacaoCustomizadaNoContexto_QuandoCriarSutEExecutar_EntaoDeveEnviarQueryComValoresCalculados()
        {
            // Arrange
            var localMocker = new AutoMocker();
            var contextoMock = localMocker.GetMock<IContextoAplicacao>();
            var mediatorMock = localMocker.GetMock<IMediator>();

            contextoMock.Setup(c => c.ObterVariavel<string>("NumeroPagina")).Returns("3");
            contextoMock.Setup(c => c.ObterVariavel<string>("NumeroRegistros")).Returns("20");

            var sutLocal = localMocker.CreateInstance<CasoDeUsoObterRegistrosDaIncricaoInconsistentes>();
            var arquivoId = _faker.Random.Long(1, 1000);
            var resultadoEsperado = new PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>(
                items: new List<RegistroDaInscricaoInsconsistenteDto>(),
                totalRegistros: 0,
                numeroRegistros: 20,
                sucesso: true);

            mediatorMock
                .Setup(m => m.Send(It.Is<ObterRegistrosDaIncricaoInconsistentesQuery>(q =>
                    q.QuantidadeRegistrosIgnorados == 40 &&
                    q.NumeroRegistros == 20 &&
                    q.ArquivoId == arquivoId
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await sutLocal.Executar(arquivoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(resultadoEsperado);
            sutLocal.NumeroPagina.Should().Be(3);
            sutLocal.NumeroRegistros.Should().Be(20);
            sutLocal.QuantidadeRegistrosIgnorados.Should().Be(40);

            mediatorMock.Verify(m => m.Send(It.Is<ObterRegistrosDaIncricaoInconsistentesQuery>(q =>
                q.QuantidadeRegistrosIgnorados == 40 &&
                q.NumeroRegistros == 20 &&
                q.ArquivoId == arquivoId
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPaginacaoCustomizadaNasPropriedades_QuandoExecutar_EntaoDeveEnviarQueryComValoresDefinidos()
        {
            // Arrange
            var arquivoId = _faker.Random.Long(1, 1000);
            _sut.NumeroRegistros = 15;
            _sut.QuantidadeRegistrosIgnorados = 30;

            var resultadoEsperado = new PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>(
                items: new List<RegistroDaInscricaoInsconsistenteDto>(),
                totalRegistros: 0,
                numeroRegistros: 15,
                sucesso: true);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRegistrosDaIncricaoInconsistentesQuery>(q =>
                    q.QuantidadeRegistrosIgnorados == 30 &&
                    q.NumeroRegistros == 15 &&
                    q.ArquivoId == arquivoId
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(arquivoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(resultadoEsperado);

            _mediatorMock.Verify(m => m.Send(It.Is<ObterRegistrosDaIncricaoInconsistentesQuery>(q =>
                q.QuantidadeRegistrosIgnorados == 30 &&
                q.NumeroRegistros == 15 &&
                q.ArquivoId == arquivoId
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var arquivoId = _faker.Random.Long(1, 1000);
            var mensagemErro = "Erro ao buscar registros inconsistentes";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterRegistrosDaIncricaoInconsistentesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(arquivoId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterRegistrosDaIncricaoInconsistentesQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
