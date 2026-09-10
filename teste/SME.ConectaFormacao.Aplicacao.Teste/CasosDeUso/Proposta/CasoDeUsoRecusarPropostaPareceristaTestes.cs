using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoRecusarPropostaPareceristaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRecusarPropostaParecerista _sut;
        private readonly Faker _faker;

        public CasoDeUsoRecusarPropostaPareceristaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRecusarPropostaParecerista>();
            _faker = new Faker();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task DadoJustificativaNaoInformada_QuandoExecutar_EntaoDeveLancarNegocioException(string? justificativa)
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var dto = new PropostaJustificativaDTO { Justificativa = justificativa! };

            // Act
            Func<Task> act = async () => await _sut.Executar(propostaId, dto);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.JUSTIFICATIVA_NAO_INFORMADA);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarParecerPareceristaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoJustificativaInformada_QuandoExecutar_EntaoDeveEnviarParecerRecusadaERetornarTrue()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var justificativa = _faker.Lorem.Sentence();
            var dto = new PropostaJustificativaDTO { Justificativa = justificativa };
            var usuario = new Usuario
            {
                Login = _faker.Random.Number(1000000, 9999999).ToString(),
                Nome = _faker.Person.FullName
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<EnviarParecerPareceristaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(propostaId, dto);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarParecerPareceristaCommand>(c =>
                c.PropostaId == propostaId &&
                c.RegistroFuncional == usuario.Login &&
                c.Situacao == SituacaoParecerista.Recusada &&
                c.Justificativa == justificativa), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
