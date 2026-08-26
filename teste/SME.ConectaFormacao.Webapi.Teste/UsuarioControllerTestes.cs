using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class UsuarioControllerTestes
    {
        private readonly Mock<ICasoDeUsoInserirUsuarioExterno> _mockInserir;
        private readonly Mock<ICasoDeUsoUsuarioSolicitarRecuperacaoSenha> _mockSolicitar;
        private readonly Mock<ICasoDeUsoUsuarioValidacaoSenhaToken> _mockValidacaoSenha;
        private readonly Mock<ICasoDeUsoUsuarioValidacaoEmailToken> _mockValidacaoEmail;
        private readonly Mock<ICasoDeUsoUsuarioRecuperarSenha> _mockRecuperar;
        private readonly Mock<ICasoDeUsoUsuarioMeusDados> _mockMeusDados;
        private readonly Mock<ICasoDeUsoUsuarioAlterarSenha> _mockAlterarSenha;
        private readonly Mock<ICasoDeUsoUsuarioAlterarEmail> _mockAlterarEmail;
        private readonly Mock<ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao> _mockAlterarEmailEReenviar;
        private readonly Mock<ICasoDeUsoUsuarioAlterarEmailEducacional> _mockAlterarEmailEdu;
        private readonly Mock<ICasoDeUsoReenviarEmail> _mockReenviar;
        private readonly Mock<ICasoDeUsoUsuarioAlterarNome> _mockAlterarNome;
        private readonly Mock<ICasoDeUsoUsuarioAlterarNomeSocial> _mockAlterarNomeSocial;
        private readonly Mock<ICasoDeUsoUsuarioAlterarTelefone> _mockAlterarTelefone;
        private readonly Mock<ICasoDeUsoUsuarioAlterarTipoEmail> _mockAlterarTipoEmail;
        private readonly Mock<ICasoDeUsoUsuarioAlterarUnidadeEol> _mockAlterarUnidadeEol;
        private readonly Mock<ICasoDeUsoObterTiposEmail> _mockTiposEmail;
        private readonly Mock<ICasoDeUsoObterUsuariosPorEolUnidade> _mockObterUsuariosEol;
        private readonly Mock<ICasoDeUsoSalvarUsuarioAcessibilidade> _mockSalvarAcessibilidade;
        private readonly UsuarioController _sut;

        public UsuarioControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockInserir = mocker.GetMock<ICasoDeUsoInserirUsuarioExterno>();
            _mockSolicitar = mocker.GetMock<ICasoDeUsoUsuarioSolicitarRecuperacaoSenha>();
            _mockValidacaoSenha = mocker.GetMock<ICasoDeUsoUsuarioValidacaoSenhaToken>();
            _mockValidacaoEmail = mocker.GetMock<ICasoDeUsoUsuarioValidacaoEmailToken>();
            _mockRecuperar = mocker.GetMock<ICasoDeUsoUsuarioRecuperarSenha>();
            _mockMeusDados = mocker.GetMock<ICasoDeUsoUsuarioMeusDados>();
            _mockAlterarSenha = mocker.GetMock<ICasoDeUsoUsuarioAlterarSenha>();
            _mockAlterarEmail = mocker.GetMock<ICasoDeUsoUsuarioAlterarEmail>();
            _mockAlterarEmailEReenviar = mocker.GetMock<ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao>();
            _mockAlterarEmailEdu = mocker.GetMock<ICasoDeUsoUsuarioAlterarEmailEducacional>();
            _mockReenviar = mocker.GetMock<ICasoDeUsoReenviarEmail>();
            _mockAlterarNome = mocker.GetMock<ICasoDeUsoUsuarioAlterarNome>();
            _mockAlterarNomeSocial = mocker.GetMock<ICasoDeUsoUsuarioAlterarNomeSocial>();
            _mockAlterarTelefone = mocker.GetMock<ICasoDeUsoUsuarioAlterarTelefone>();
            _mockAlterarTipoEmail = mocker.GetMock<ICasoDeUsoUsuarioAlterarTipoEmail>();
            _mockAlterarUnidadeEol = mocker.GetMock<ICasoDeUsoUsuarioAlterarUnidadeEol>();
            _mockTiposEmail = mocker.GetMock<ICasoDeUsoObterTiposEmail>();
            _mockObterUsuariosEol = mocker.GetMock<ICasoDeUsoObterUsuariosPorEolUnidade>();
            _mockSalvarAcessibilidade = mocker.GetMock<ICasoDeUsoSalvarUsuarioAcessibilidade>();
            _sut = mocker.CreateInstance<UsuarioController>();
        }

        [Fact]
        public async Task DadoDtoValido_QuandoInserir_EntaoRetornaUsuarioExterno()
        {
            // Arrange
            var dto = new UsuarioExternoDTO();
            var retorno = new InserirUsuarioRetornoDTO();
            _mockInserir.Setup(m => m.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.Inserir(dto, _mockInserir.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockInserir.Verify(m => m.Executar(dto), Times.Once);
        }

        [Fact]
        public async Task DadoLoginValido_QuandoSolicitarRecuperacaoSenha_EntaoRetornaString()
        {
            // Arrange
            var login = "login";
            var retorno = "solicitado";
            _mockSolicitar.Setup(m => m.Executar(login)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.SolicitarRecuperacaoSenha(login, _mockSolicitar.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(retorno);
            _mockSolicitar.Verify(m => m.Executar(login), Times.Once);
        }

        [Fact]
        public async Task DadoTokenValido_QuandoTokenRecuperacaoSenhaEstaValido_EntaoRetornaTrue()
        {
            // Arrange
            var token = Guid.NewGuid();
            _mockValidacaoSenha.Setup(m => m.Executar(token)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.TokenRecuperacaoSenhaEstaValido(token, _mockValidacaoSenha.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockValidacaoSenha.Verify(m => m.Executar(token), Times.Once);
        }

        [Fact]
        public async Task DadoTokenValido_QuandoValidarEmailToken_EntaoRetornaTrue()
        {
            // Arrange
            var token = Guid.NewGuid();
            var retorno = new UsuarioPerfisRetornoDTO();
            _mockValidacaoEmail.Setup(m => m.Executar(token)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ValidarEmailToken(token, _mockValidacaoEmail.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockValidacaoEmail.Verify(m => m.Executar(token), Times.Once);
        }

        [Fact]
        public async Task DadoDtoValido_QuandoRecuperarSenha_EntaoRetornaRetorno()
        {
            // Arrange
            var dto = new RecuperacaoSenhaDto();
            var retorno = new UsuarioPerfisRetornoDTO();
            _mockRecuperar.Setup(m => m.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.RecuperarSenha(dto, _mockRecuperar.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockRecuperar.Verify(m => m.Executar(dto), Times.Once);
        }

        [Fact]
        public async Task DadoLoginValido_QuandoMeusDados_EntaoRetornaDadosUsuario()
        {
            // Arrange
            var login = "login";
            var retorno = new DadosUsuarioDTO();
            _mockMeusDados.Setup(m => m.Executar(login)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.MeusDados(login, _mockMeusDados.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockMeusDados.Verify(m => m.Executar(login), Times.Once);
        }

        [Fact]
        public async Task DadoDtoELoginValidos_QuandoAlterarSenha_EntaoRetornaTrue()
        {
            // Arrange
            var login = "login";
            var dto = new AlterarSenhaUsuarioDTO();
            _mockAlterarSenha.Setup(m => m.Executar(login, dto)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarSenha(login, dto, _mockAlterarSenha.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockAlterarSenha.Verify(m => m.Executar(login, dto), Times.Once);
        }

        [Fact]
        public async Task DadoDtoELoginValidos_QuandoAlterarEmailCoreSSO_EntaoRetornaTrue()
        {
            // Arrange
            var login = "login";
            var dto = new EmailUsuarioDTO { Email = "teste@teste.com" };
            _mockAlterarEmail.Setup(m => m.Executar(login, dto.Email)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarEmailCoreSSO(login, dto, _mockAlterarEmail.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockAlterarEmail.Verify(m => m.Executar(login, dto.Email), Times.Once);
        }

        [Fact]
        public async Task DadoDtoValido_QuandoAlterarEmailEReenviarEmailParaValidacao_EntaoRetornaTrue()
        {
            // Arrange
            var dto = new AlterarEmailUsuarioDto();
            _mockAlterarEmailEReenviar.Setup(m => m.Executar(dto)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarEmailEReenviarEmailParaValidacao(dto, _mockAlterarEmailEReenviar.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockAlterarEmailEReenviar.Verify(m => m.Executar(dto), Times.Once);
        }

        [Fact]
        public async Task DadoDtoELoginValidos_QuandoAlterarEmailEducacional_EntaoRetornaTrue()
        {
            // Arrange
            var login = "login";
            var dto = new EmailUsuarioDTO { Email = "teste@teste.com" };
            _mockAlterarEmailEdu.Setup(m => m.Executar(login, dto.Email)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarEmailEducacional(login, dto, _mockAlterarEmailEdu.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockAlterarEmailEdu.Verify(m => m.Executar(login, dto.Email), Times.Once);
        }

        [Fact]
        public async Task DadoLoginValido_QuandoReenviarEmailParaValidacao_EntaoRetornaDados()
        {
            // Arrange
            var login = "login";
            _mockReenviar.Setup(m => m.Executar(login)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.ReenviarEmailParaValidacao(login, _mockReenviar.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockReenviar.Verify(m => m.Executar(login), Times.Once);
        }

        [Fact]
        public async Task DadoDtoELoginValidos_QuandoAlterarNomeConectaECoreSSO_EntaoRetornaTrue()
        {
            // Arrange
            var login = "login";
            var dto = new NomeUsuarioDTO { Nome = "teste" };
            _mockAlterarNome.Setup(m => m.Executar(login, dto.Nome)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarNomeConectaECoreSSO(login, dto, _mockAlterarNome.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockAlterarNome.Verify(m => m.Executar(login, dto.Nome), Times.Once);
        }

        [Fact]
        public async Task DadoDtoELoginValidos_QuandoAlterarNomeSocialConectaECoreSSO_EntaoRetornaTrue()
        {
            // Arrange
            var login = "login";
            var dto = new NomeSocialUsuarioDto { NomeSocial = "teste social" };
            _mockAlterarNomeSocial.Setup(m => m.Executar(login, dto.NomeSocial)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarNomeSocialConectaECoreSSO(login, dto, _mockAlterarNomeSocial.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockAlterarNomeSocial.Verify(m => m.Executar(login, dto.NomeSocial), Times.Once);
        }

        [Fact]
        public async Task DadoDtoELoginValidos_QuandoAlterarTelefoneConectaECoreSSO_EntaoRetornaTrue()
        {
            // Arrange
            var login = "login";
            var dto = new TelefoneUsuarioDTO { Telefone = "11999999999" };
            _mockAlterarTelefone.Setup(m => m.Executar(login, dto.Telefone)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarTelefoneConectaECoreSSO(login, dto, _mockAlterarTelefone.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockAlterarTelefone.Verify(m => m.Executar(login, dto.Telefone), Times.Once);
        }

        [Fact]
        public async Task DadoDtoELoginValidos_QuandoAlterarTipoEmail_EntaoRetornaTrue()
        {
            // Arrange
            var login = "login";
            var dto = new TipoEmailUsuarioDTO { Tipo = 1 };
            _mockAlterarTipoEmail.Setup(m => m.Executar(login, dto.Tipo)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarTipoEmail(login, dto, _mockAlterarTipoEmail.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockAlterarTipoEmail.Verify(m => m.Executar(login, dto.Tipo), Times.Once);
        }

        [Fact]
        public async Task DadoDtoELoginValidos_QuandoAlterarUnidadeEol_EntaoRetornaTrue()
        {
            // Arrange
            var login = "login";
            var dto = new UnidadeEolUsuarioDTO { CodigoEolUnidade = "123" };
            _mockAlterarUnidadeEol.Setup(m => m.Executar(login, dto.CodigoEolUnidade)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.AlterarUnidadeEol(login, dto, _mockAlterarUnidadeEol.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockAlterarUnidadeEol.Verify(m => m.Executar(login, dto.CodigoEolUnidade), Times.Once);
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterListaTipoEmail_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockTiposEmail.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterListaTipoEmail(_mockTiposEmail.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockTiposEmail.Verify(m => m.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadoCodigoValido_QuandoObterUsuariosPorEolUnidadeAsync_EntaoRetornaLista()
        {
            // Arrange
            var codigoEol = "123";
            var retorno = new List<DadosLoginUsuarioDto>();
            _mockObterUsuariosEol.Setup(m => m.ExecutarAsync(codigoEol, null, null)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterUsuariosPorEolUnidadeAsync(_mockObterUsuariosEol.Object, codigoEol, null, null) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterUsuariosEol.Verify(m => m.ExecutarAsync(codigoEol, null, null), Times.Once);
        }

        [Fact]
        public async Task DadoDtoELoginValidos_QuandoSalvarAcessibilidadeDaInscricao_EntaoRetornaTrue()
        {
            // Arrange
            var login = "login";
            var dto = new UsuarioAcessibilidadeDto();
            var retorno = SME.ConectaFormacao.Dominio.Comum.Resultado.DeSucesso();
            _mockSalvarAcessibilidade.Setup(m => m.ExecutarAsync(login, dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.SalvarAcessibilidadeDaInscricao(login, dto, _mockSalvarAcessibilidade.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(retorno);
            _mockSalvarAcessibilidade.Verify(m => m.ExecutarAsync(login, dto), Times.Once);
        }
    }
}
