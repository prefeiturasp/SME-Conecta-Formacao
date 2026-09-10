using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Autenticacaos
{
    public class CasoDeUsoAutenticarUsuarioTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAutenticarUsuario _casoDeUso;

        public CasoDeUsoAutenticarUsuarioTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoAutenticarUsuario>();
        }

        [Fact]
        public async Task DadoUsuarioNaoEncontrado_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var dto = new AutenticacaoDto { Login = "teste", Senha = "123" };
            var retornoVazio = new UsuarioAutenticacaoRetornoDto { Login = "" };
            
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioServicoAcessosPorLoginSenhaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoVazio);

            // Act
            var act = async () => await _casoDeUso.Executar(dto);

            // Assert
            var ex = await act.Should().ThrowAsync<NegocioException>();
            ex.WithMessage(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS);
            ex.Which.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DadoUsuarioEncontrado_QuandoExecutar_EntaoDeveRetornarTokenAcesso()
        {
            // Arrange
            var dto = new AutenticacaoDto { Login = "teste", Senha = "123" };
            var retorno = new UsuarioAutenticacaoRetornoDto { Login = "teste" };
            var retornoEsperado = new UsuarioPerfisRetornoDTO();
            
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuarioServicoAcessosPorLoginSenhaQuery>(q => q.Login == dto.Login && q.Senha == dto.Senha), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retorno);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterTokenAcessoQuery>(q => q.Login == retorno.Login && q.PerfilUsuarioId == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(dto);

            // Assert
            resultado.Should().Be(retornoEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterTokenAcessoQuery>(q => q.Login == retorno.Login && q.PerfilUsuarioId == null), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
