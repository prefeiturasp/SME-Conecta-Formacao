using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioAlterarSenhaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioAlterarSenha _sut;

        public CasoDeUsoUsuarioAlterarSenhaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoUsuarioAlterarSenha>();
        }

        [Fact]
        public async Task DadoSenhasDiferentes_QuandoExecutar_EntaoLancaNegocioExceptionDeSenhaInvalida()
        {
            // Arrange
            var dto = new AlterarSenhaUsuarioDTO
            {
                SenhaAtual = "Atual123!",
                SenhaNova = "Nova123!",
                ConfirmarSenha = "Nova456!"
            };

            // Act
            var acao = async () => await _sut.Executar("login123", dto);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.CONFIRMACAO_SENHA_INVALIDA);

            _mediatorMock.Verify(m => m.Send(It.IsAny<AlterarSenhaServicoAcessosCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoSenhaFraca_QuandoExecutar_EntaoLancaNegocioExceptionDeSeguranca()
        {
            // Arrange
            var dto = new AlterarSenhaUsuarioDTO
            {
                SenhaAtual = "Atual123!",
                SenhaNova = "fraca",
                ConfirmarSenha = "fraca"
            };

            // Act
            var acao = async () => await _sut.Executar("login123", dto);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.SENHA_NAO_ATENDE_CRITERIOS_SEGURANCA);

            _mediatorMock.Verify(m => m.Send(It.IsAny<AlterarSenhaServicoAcessosCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoErroNoServicoAcessos_QuandoExecutar_EntaoLancaNegocioExceptionDeNaoConferem()
        {
            // Arrange
            var dto = new AlterarSenhaUsuarioDTO
            {
                SenhaAtual = "Atual123!",
                SenhaNova = "SenhaForte1!",
                ConfirmarSenha = "SenhaForte1!"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<AlterarSenhaServicoAcessosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var acao = async () => await _sut.Executar("login123", dto);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.LOGIN_OU_SENHA_ATUAL_NAO_CONFEREM);
        }

        [Fact]
        public async Task DadoSucessoNoServicoAcessos_QuandoExecutar_EntaoRetornaVerdadeiro()
        {
            // Arrange
            var dto = new AlterarSenhaUsuarioDTO
            {
                SenhaAtual = "Atual123!",
                SenhaNova = "SenhaForte1!",
                ConfirmarSenha = "SenhaForte1!"
            };

            _mediatorMock.Setup(m => m.Send(It.Is<AlterarSenhaServicoAcessosCommand>(c =>
                    c.Login == "login123" &&
                    c.SenhaAtual == "Atual123!" &&
                    c.NovaSenha == "SenhaForte1!"),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar("login123", dto);

            // Assert
            resultado.Should().BeTrue();
        }
    }
}
