using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterSugestoesPareceristas;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoObterSugestaoParecerPareceristasTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterSugestaoParecerPareceristas _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterSugestaoParecerPareceristasTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterSugestaoParecerPareceristas>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaIdValidoComSugestoes_QuandoExecutar_EntaoDeveEnviarQueryERetornarListaSugestoes()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 10000);
            var sugestoesEsperadas = new List<PropostaPareceristaSugestaoDTO>
            {
                new()
                {
                    Parecerista = _faker.Person.FullName,
                    Situacao = 1,
                    Justificativa = _faker.Lorem.Sentence()
                },
                new()
                {
                    Parecerista = _faker.Person.FullName,
                    Situacao = 2,
                    Justificativa = _faker.Lorem.Sentence()
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterSugestoesPareceristasQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sugestoesEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar(propostaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(sugestoesEsperadas);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterSugestoesPareceristasQuery>(q => q.PropostaId == propostaId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaSemSugestoes_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 10000);
            var sugestoesEsperadas = Enumerable.Empty<PropostaPareceristaSugestaoDTO>();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterSugestoesPareceristasQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sugestoesEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar(propostaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterSugestoesPareceristasQuery>(q => q.PropostaId == propostaId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoEnviarQuery_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 10000);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterSugestoesPareceristasQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _casoDeUso.Executar(propostaId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterSugestoesPareceristasQuery>(q => q.PropostaId == propostaId),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
