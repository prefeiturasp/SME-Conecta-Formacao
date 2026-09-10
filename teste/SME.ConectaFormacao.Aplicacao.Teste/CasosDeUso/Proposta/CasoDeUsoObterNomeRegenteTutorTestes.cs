using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System;
using System.Threading.Tasks;
using Xunit;
using EntidadeProposta = SME.ConectaFormacao.Dominio.Entidades.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterNomeRegenteTutorTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterNomeRegenteTutor _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterNomeRegenteTutorTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterNomeRegenteTutor>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoRegistroFuncionalValido_QuandoExecutar_EntaoDeveEnviarQueryERetornarNomeProfissional()
        {
            // Arrange
            var registroFuncional = _faker.Random.Number(1000000, 9999999).ToString();
            var nomeEsperado = _faker.Person.FullName;
            var retornoUsuario = new RetornoUsuarioCpfNomeDTO
            {
                Nome = nomeEsperado,
                Cpf = _faker.Random.Replace("###.###.###-##")
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterNomeCpfProfissionalPorRegistroFuncionalQuery>(q => q.RegistroFuncional == registroFuncional), default))
                .ReturnsAsync(retornoUsuario);

            // Act
            var resultado = await _sut.Executar(registroFuncional);

            // Assert
            resultado.Should().Be(nomeEsperado);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterNomeCpfProfissionalPorRegistroFuncionalQuery>(q => q.RegistroFuncional == registroFuncional),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoEnviarQuery_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var registroFuncional = _faker.Random.Number(1000000, 9999999).ToString();
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterNomeCpfProfissionalPorRegistroFuncionalQuery>(q => q.RegistroFuncional == registroFuncional), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(registroFuncional);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterNomeCpfProfissionalPorRegistroFuncionalQuery>(q => q.RegistroFuncional == registroFuncional),
                default), Times.Once);
        }
    }
}
