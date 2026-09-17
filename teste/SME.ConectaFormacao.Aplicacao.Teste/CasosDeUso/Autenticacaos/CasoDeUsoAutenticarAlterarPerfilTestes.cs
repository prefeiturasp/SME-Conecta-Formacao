using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Autenticacaos
{
    public class CasoDeUsoAutenticarAlterarPerfilTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAutenticarAlterarPerfil _casoDeUso;

        public CasoDeUsoAutenticarAlterarPerfilTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoAutenticarAlterarPerfil>();
        }

        [Fact]
        public async Task DadoUsuarioNaoLogado_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var perfilId = Guid.NewGuid();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var act = async () => await _casoDeUso.Executar(perfilId);

            // Assert
            var ex = await act.Should().ThrowAsync<NegocioException>();
            ex.WithMessage(MensagemNegocio.LOGIN_NAO_ENCONTRADO);
            ex.Which.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DadoUsuarioLogado_QuandoExecutar_EntaoDeveRetornarTokenAcesso()
        {
            // Arrange
            var perfilId = Guid.NewGuid();
            var usuario = new Usuario { Login = "usuario.teste" };
            var retornoEsperado = new UsuarioPerfisRetornoDTO();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterTokenAcessoQuery>(q => q.Login == usuario.Login && q.PerfilUsuarioId == perfilId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(perfilId);

            // Assert
            resultado.Should().Be(retornoEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterTokenAcessoQuery>(q => q.Login == usuario.Login && q.PerfilUsuarioId == perfilId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
