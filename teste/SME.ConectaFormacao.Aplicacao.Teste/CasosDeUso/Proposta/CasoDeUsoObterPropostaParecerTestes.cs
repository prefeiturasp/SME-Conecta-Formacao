using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaParecerTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterPropostaParecer _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterPropostaParecerTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterPropostaParecer>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaParecerFiltroDtoValido_QuandoExecutar_EntaoEnviaQueryERetornaPropostaPareceristaConsideracaoCompletoDto()
        {
            // Arrange
            var propostaParecerFiltroDto = new PropostaParecerFiltroDTO
            {
                PropostaId = _faker.Random.Long(1, 1000),
                Campo = _faker.PickRandom<CampoConsideracao>()
            };

            var retornoEsperado = new PropostaPareceristaConsideracaoCompletoDTO
            {
                PropostaId = propostaParecerFiltroDto.PropostaId,
                PodeInserir = true,
                Itens = new List<PropostaPareceristaConsideracaoDTO>
                {
                    new()
                    {
                        Id = _faker.Random.Long(1, 1000),
                        Campo = propostaParecerFiltroDto.Campo,
                        Descricao = _faker.Lorem.Sentence(),
                        PodeAlterar = true
                    }
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterPropostaParecerPorPropostaIdECampoQuery>(q =>
                        q.PropostaId == propostaParecerFiltroDto.PropostaId &&
                        q.CampoConsideracao == propostaParecerFiltroDto.Campo),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _sut.Executar(propostaParecerFiltroDto);

            // Assert
            resultado.Should().BeEquivalentTo(retornoEsperado);
            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaParecerPorPropostaIdECampoQuery>(q =>
                    q.PropostaId == propostaParecerFiltroDto.PropostaId &&
                    q.CampoConsideracao == propostaParecerFiltroDto.Campo),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaSemParecerParaCampo_QuandoExecutar_EntaoRetornaNulo()
        {
            // Arrange
            var propostaParecerFiltroDto = new PropostaParecerFiltroDTO
            {
                PropostaId = _faker.Random.Long(1, 1000),
                Campo = _faker.PickRandom<CampoConsideracao>()
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterPropostaParecerPorPropostaIdECampoQuery>(q =>
                        q.PropostaId == propostaParecerFiltroDto.PropostaId &&
                        q.CampoConsideracao == propostaParecerFiltroDto.Campo),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((PropostaPareceristaConsideracaoCompletoDTO)null!);

            // Act
            var resultado = await _sut.Executar(propostaParecerFiltroDto);

            // Assert
            resultado.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaParecerPorPropostaIdECampoQuery>(q =>
                    q.PropostaId == propostaParecerFiltroDto.PropostaId &&
                    q.CampoConsideracao == propostaParecerFiltroDto.Campo),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoEnviarQuery_QuandoExecutar_EntaoPropagaExcecao()
        {
            // Arrange
            var propostaParecerFiltroDto = new PropostaParecerFiltroDTO
            {
                PropostaId = _faker.Random.Long(1, 1000),
                Campo = _faker.PickRandom<CampoConsideracao>()
            };

            var mensagemErro = _faker.Lorem.Sentence();
            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostaParecerPorPropostaIdECampoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            Func<Task> act = async () => await _sut.Executar(propostaParecerFiltroDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaParecerPorPropostaIdECampoQuery>(q =>
                    q.PropostaId == propostaParecerFiltroDto.PropostaId &&
                    q.CampoConsideracao == propostaParecerFiltroDto.Campo),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
