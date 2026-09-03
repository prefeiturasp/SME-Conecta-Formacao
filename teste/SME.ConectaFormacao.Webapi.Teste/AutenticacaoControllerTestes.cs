using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class AutenticacaoControllerTestes
    {
        private readonly Mock<ICasoDeUsoAutenticarUsuario> _mockAutenticarUsuario;
        private readonly Mock<ICasoDeUsoAutenticarRevalidar> _mockAutenticarRevalidar;
        private readonly Mock<ICasoDeUsoAutenticarAlterarPerfil> _mockAlterarPerfil;
        private readonly AutenticacaoController _sut;

        public AutenticacaoControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockAutenticarUsuario = mocker.GetMock<ICasoDeUsoAutenticarUsuario>();
            _mockAutenticarRevalidar = mocker.GetMock<ICasoDeUsoAutenticarRevalidar>();
            _mockAlterarPerfil = mocker.GetMock<ICasoDeUsoAutenticarAlterarPerfil>();
            _sut = mocker.CreateInstance<AutenticacaoController>();
        }

        [Fact]
        public async Task DadoDtoValido_QuandoAutenticar_EntaoRetornaOk()
        {
            // Arrange
            var dto = new AutenticacaoDto { Login = "login", Senha = "pwd" };
            var retorno = new UsuarioPerfisRetornoDTO();
            _mockAutenticarUsuario.Setup(m => m.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.Autenticar(_mockAutenticarUsuario.Object, dto) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockAutenticarUsuario.Verify(m => m.Executar(dto), Times.Once);
        }

        [Fact]
        public async Task DadoTokenValido_QuandoRevalidar_EntaoRetornaOk()
        {
            // Arrange
            var dto = new AutenticacaoRevalidarDTO { Token = "token_teste" };
            var retorno = new UsuarioPerfisRetornoDTO();
            _mockAutenticarRevalidar.Setup(m => m.Executar(dto.Token)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.Revalidar(_mockAutenticarRevalidar.Object, dto) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockAutenticarRevalidar.Verify(m => m.Executar(dto.Token), Times.Once);
        }

        [Fact]
        public async Task DadoIdPerfilValido_QuandoAtualizarPerfil_EntaoRetornaOk()
        {
            // Arrange
            var perfilUsuarioId = Guid.NewGuid();
            var retorno = new UsuarioPerfisRetornoDTO();
            _mockAlterarPerfil.Setup(m => m.Executar(perfilUsuarioId)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.AtualizarPerfil(_mockAlterarPerfil.Object, perfilUsuarioId) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockAlterarPerfil.Verify(m => m.Executar(perfilUsuarioId), Times.Once);
        }
    }
}
