using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using System;
using System.Threading.Tasks;
using Xunit;
using EntidadeProposta = SME.ConectaFormacao.Dominio.Entidades.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoSalvarPropostaRegenteTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoSalvarPropostaRegente _sut;
        private readonly Faker _faker;

        public CasoDeUsoSalvarPropostaRegenteTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoSalvarPropostaRegente>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoParametrosValidos_QuandoExecutar_EntaoDeveEnviarCommandERetornarId()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);
            var dto = new PropostaRegenteDTO { NomeRegente = _faker.Person.FullName };
            var idEsperado = _faker.Random.Long(1, 100);

            _mediatorMock
                .Setup(m => m.Send(It.Is<SalvarPropostaRegenteCommand>(c => c.PropostaId == id && c.PropostaRegenteDTO == dto), default))
                .ReturnsAsync(idEsperado);

            // Act
            var resultado = await _sut.Executar(id, dto);

            // Assert
            resultado.Should().Be(idEsperado);
            _mediatorMock.Verify(m => m.Send(
                It.Is<SalvarPropostaRegenteCommand>(c => c.PropostaId == id && c.PropostaRegenteDTO == dto),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);
            var dto = new PropostaRegenteDTO();
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarPropostaRegenteCommand>(), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(id, dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
        }
    }
}
