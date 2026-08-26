using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos;
using SME.ConectaFormacao.Aplicacao.Comandos.Usuarios.AlterarEmailEducacional;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos
{
    public class AlterarEmailEducacionalCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly AlterarEmailEducacionalCommandHandler _sut;

        public AlterarEmailEducacionalCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<AlterarEmailEducacionalCommandHandler>();
        }

        [Fact]
        public async Task DadoEmailInvalido_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            var comando = new AlterarEmailEducacionalCommand("teste@gmail.com", "login");

            // Act
            Func<Task> acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<NegocioException>();
        }

        [Fact]
        public async Task DadoEmailValidoEErroAtualizacao_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            var comando = new AlterarEmailEducacionalCommand("teste@edu.sme.prefeitura.sp.gov.br", "login");

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(m => m.AtualizarEmailEducacional("login", "teste@edu.sme.prefeitura.sp.gov.br"))
                .ReturnsAsync(false);

            // Act
            Func<Task> acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<NegocioException>();
        }

        [Fact]
        public async Task DadoEmailValidoEAtualizacaoComSucesso_QuandoExecutar_EntaoRetornaTrue()
        {
            // Arrange
            var comando = new AlterarEmailEducacionalCommand("teste@edu.sme.prefeitura.sp.gov.br", "login");

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(m => m.AtualizarEmailEducacional(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
