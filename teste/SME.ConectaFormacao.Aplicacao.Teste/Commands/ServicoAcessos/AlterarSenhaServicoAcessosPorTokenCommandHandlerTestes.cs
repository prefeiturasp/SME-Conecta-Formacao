using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ServicoAcessos
{
    public class AlterarSenhaServicoAcessosPorTokenCommandHandlerTestes
    {
        private readonly Mock<IServicoAcessos> _servicoAcessosMock;
        private readonly Faker _faker;
        private readonly AlterarSenhaServicoAcessosPorTokenCommandHandler _handler;

        public AlterarSenhaServicoAcessosPorTokenCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _servicoAcessosMock = mocker.GetMock<IServicoAcessos>();
            _handler = mocker.CreateInstance<AlterarSenhaServicoAcessosPorTokenCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public void DadoServicoAcessoNulo_QuandoInstanciar_EntaoDeveLancarArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new AlterarSenhaServicoAcessosPorTokenCommandHandler(null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("servicoAcessos");
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_EntaoDeveRetornarStringSucesso()
        {
            // Arrange
            var token = Guid.NewGuid();
            var novaSenha = _faker.Internet.Password();
            var comando = new AlterarSenhaServicoAcessosPorTokenCommand(token, novaSenha);
            var respostaEsperada = "Senha alterada com sucesso";

            _servicoAcessosMock.Setup(s => s.AlterarSenhaComTokenRecuperacao(token, novaSenha))
                .ReturnsAsync(respostaEsperada);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNullOrWhiteSpace();
            resultado.Should().Be(respostaEsperada);
            _servicoAcessosMock.Verify(s => s.AlterarSenhaComTokenRecuperacao(token, novaSenha), Times.Once);
        }

        [Fact]
        public async Task DadoServicoAcessoLancaExcecao_QuandoExecutarHandle_EntaoDeveLancarExcecao()
        {
            // Arrange
            var token = Guid.NewGuid();
            var novaSenha = _faker.Internet.Password();
            var comando = new AlterarSenhaServicoAcessosPorTokenCommand(token, novaSenha);
            var mensagemErro = "Erro ao alterar senha";
            
            _servicoAcessosMock.Setup(s => s.AlterarSenhaComTokenRecuperacao(token, novaSenha))
                .ThrowsAsync(new Exception(mensagemErro));

            // Act & Assert
            var act = async () => await _handler.Handle(comando, CancellationToken.None);
            
            await act.Should().ThrowAsync<Exception>().WithMessage(mensagemErro);
            _servicoAcessosMock.Verify(s => s.AlterarSenhaComTokenRecuperacao(token, novaSenha), Times.Once);
        }
    }
}
