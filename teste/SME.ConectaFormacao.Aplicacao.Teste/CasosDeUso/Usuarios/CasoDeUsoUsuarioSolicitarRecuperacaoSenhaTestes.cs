using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.EnviarEmailAdminSolicitacaoResetSenha;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioSolicitarRecuperacaoSenhaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioSolicitarRecuperacaoSenha _sut;
        private readonly Faker _faker;

        public CasoDeUsoUsuarioSolicitarRecuperacaoSenhaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoUsuarioSolicitarRecuperacaoSenha>();
            _faker = new Faker();
        }

        [Theory]
        [InlineData("")]
        [InlineData("emailinvalido")]
        [InlineData("usuario.sem.arroba")]
        public async Task DadoEmailRetornadoSemArroba_QuandoExecutar_EntaoDeveLancarNegocioExceptionLoginNaoEncontrado(string emailInvalido)
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();

            _mediatorMock
                .Setup(m => m.Send(It.Is<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand>(c => c.Login == login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emailInvalido);

            // Act
            Func<Task> act = async () => await _sut.Executar(login);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.LOGIN_NAO_ENCONTRADO);
            _mediatorMock.Verify(m => m.Send(It.Is<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand>(c => c.Login == login), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarEmailAdminSolicitacaoResetSenhaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoEmailValido_QuandoExecutar_EntaoDeveEnviarEmailAdminERetornarMensagemOrientacoes()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var email = "usuario@sme.prefeitura.sp.gov.br";
            var mensagemEsperada = string.Format(MensagemNegocio.ORIENTACOES_RECUPERACAO_SENHA, email.TratarEmail());

            _mediatorMock
                .Setup(m => m.Send(It.Is<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand>(c => c.Login == login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(email);

            _mediatorMock
                .Setup(m => m.Send(It.Is<EnviarEmailAdminSolicitacaoResetSenhaCommand>(c => c.Login == login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(login);

            // Assert
            resultado.Should().Be(mensagemEsperada);
            _mediatorMock.Verify(m => m.Send(It.Is<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand>(c => c.Login == login), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarEmailAdminSolicitacaoResetSenhaCommand>(c => c.Login == login), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Erro de conexão ao serviço de acessos."));

            // Act
            Func<Task> act = async () => await _sut.Executar(login);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro de conexão ao serviço de acessos.");
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarEmailAdminSolicitacaoResetSenhaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
