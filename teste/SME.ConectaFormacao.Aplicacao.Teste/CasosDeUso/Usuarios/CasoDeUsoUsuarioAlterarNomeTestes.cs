using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioAlterarNomeTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioAlterarNome _sut;
        private readonly Faker _faker;

        public CasoDeUsoUsuarioAlterarNomeTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoUsuarioAlterarNome>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoAlteracaoComSucesso_QuandoExecutar_EntaoEnviaCommandsERetornaTrue()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var nome = _faker.Person.FullName;

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarNomeServicoAcessosCommand>(c => c.Login == login && c.Nome == nome), default))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.Is<SalvarUsuarioParcialCommand>(c => c.Login == login && c.Nome == nome.ToUpper()), default))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarEmailEduAoAlterarNomeTipoEmailCommand>(c => c.Login == login), default))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(login, nome);

            // Assert
            resultado.Should().BeTrue();

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarNomeServicoAcessosCommand>(c => c.Login == login && c.Nome == nome),
                default), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<SalvarUsuarioParcialCommand>(c => c.Login == login && c.Nome == nome.ToUpper()),
                default), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarEmailEduAoAlterarNomeTipoEmailCommand>(c => c.Login == login),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoSalvarUsuarioParcialRetornaFalse_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var nome = _faker.Person.FullName;

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarNomeServicoAcessosCommand>(c => c.Login == login && c.Nome == nome), default))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.Is<SalvarUsuarioParcialCommand>(c => c.Login == login && c.Nome == nome.ToUpper()), default))
                .ReturnsAsync(false);

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarEmailEduAoAlterarNomeTipoEmailCommand>(c => c.Login == login), default))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(login, nome);

            // Assert
            resultado.Should().BeFalse();

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarNomeServicoAcessosCommand>(c => c.Login == login && c.Nome == nome),
                default), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<SalvarUsuarioParcialCommand>(c => c.Login == login && c.Nome == nome.ToUpper()),
                default), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarEmailEduAoAlterarNomeTipoEmailCommand>(c => c.Login == login),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoPropagaExcecao()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var nome = _faker.Person.FullName;
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AlterarNomeServicoAcessosCommand>(), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(login, nome);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
        }
    }
}
