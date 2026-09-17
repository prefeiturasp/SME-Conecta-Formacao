using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoAlterarEmailEReenviarEmailParaValidacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAlterarEmailEReenviarEmailParaValidacao _casoDeUso;

        public CasoDeUsoAlterarEmailEReenviarEmailParaValidacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _casoDeUso = mocker.CreateInstance<CasoDeUsoAlterarEmailEReenviarEmailParaValidacao>();
        }

        [Fact]
        public async Task DadoLoginOuSenhaInvalidos_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var emailUsuarioDto = new AlterarEmailUsuarioDto { Login = "login", Senha = "123", Email = "a@a.com" };
            
            _mediatorMock.Setup(m => m.Send(It.Is<ObterUsuarioServicoAcessosPorLoginSenhaQuery>(q => q.Login == emailUsuarioDto.Login && q.Senha == emailUsuarioDto.Senha), default))
                .ReturnsAsync(new UsuarioAutenticacaoRetornoDto { Login = string.Empty });

            // Act
            var acao = async () => await _casoDeUso.Executar(emailUsuarioDto);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS);
            excecao.Which.StatusCode.Should().Be(401);
            
            _mediatorMock.Verify(m => m.Send(It.IsAny<AlterarEmailServicoAcessosCommand>(), default), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(), default), Times.Never);
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoExecutar_EntaoDeveAlterarEReenviarEmail()
        {
            // Arrange
            var emailUsuarioDto = new AlterarEmailUsuarioDto { Login = "loginValido", Senha = "123", Email = "novo@a.com" };
            
            _mediatorMock.Setup(m => m.Send(It.Is<ObterUsuarioServicoAcessosPorLoginSenhaQuery>(q => q.Login == emailUsuarioDto.Login && q.Senha == emailUsuarioDto.Senha), default))
                .ReturnsAsync(new UsuarioAutenticacaoRetornoDto { Login = "loginValido" });

            _mediatorMock.Setup(m => m.Send(It.Is<AlterarEmailServicoAcessosCommand>(c => c.Login == emailUsuarioDto.Login && c.Email == emailUsuarioDto.Email), default))
                .ReturnsAsync(true);

            _mediatorMock.Setup(m => m.Send(It.Is<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(c => c.Login == emailUsuarioDto.Login), default))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(emailUsuarioDto);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<AlterarEmailServicoAcessosCommand>(c => c.Login == emailUsuarioDto.Login && c.Email == emailUsuarioDto.Email), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(c => c.Login == emailUsuarioDto.Login), default), Times.Once);
        }
    }
}
