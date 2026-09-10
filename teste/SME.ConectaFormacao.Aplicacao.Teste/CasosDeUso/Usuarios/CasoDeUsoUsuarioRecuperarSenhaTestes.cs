using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioRecuperarSenhaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioRecuperarSenha _sut;
        private readonly Faker _faker;

        public CasoDeUsoUsuarioRecuperarSenhaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoUsuarioRecuperarSenha>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoRecuperacaoSenhaDtoValido_QuandoExecutar_EntaoDeveAlterarSenhaObterTokenERetornarUsuarioPerfis()
        {
            // Arrange
            var dto = new RecuperacaoSenhaDto
            {
                Token = Guid.NewGuid(),
                NovaSenha = "NovaSenhaForte123!"
            };
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var usuarioPerfisEsperado = new UsuarioPerfisRetornoDTO
            {
                UsuarioLogin = login,
                UsuarioNome = _faker.Person.FullName,
                Token = Guid.NewGuid().ToString(),
                Autenticado = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarSenhaServicoAcessosPorTokenCommand>(c => c.Token == dto.Token && c.NovaSenha == dto.NovaSenha), It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterTokenAcessoQuery>(q => q.Login == login && q.PerfilUsuarioId == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisEsperado);

            // Act
            var resultado = await _sut.Executar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(usuarioPerfisEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<AlterarSenhaServicoAcessosPorTokenCommand>(c => c.Token == dto.Token && c.NovaSenha == dto.NovaSenha), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterTokenAcessoQuery>(q => q.Login == login && q.PerfilUsuarioId == null), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoAlterarSenha_QuandoExecutar_EntaoDevePropagarExcecaoENaoObterToken()
        {
            // Arrange
            var dto = new RecuperacaoSenhaDto
            {
                Token = Guid.NewGuid(),
                NovaSenha = "NovaSenhaForte123!"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AlterarSenhaServicoAcessosPorTokenCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Token expirado."));

            // Act
            Func<Task> act = async () => await _sut.Executar(dto);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage("Token expirado.");
            _mediatorMock.Verify(m => m.Send(It.IsAny<AlterarSenhaServicoAcessosPorTokenCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterTokenAcessoQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoFalhaAoObterTokenAcesso_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var dto = new RecuperacaoSenhaDto
            {
                Token = Guid.NewGuid(),
                NovaSenha = "NovaSenhaForte123!"
            };
            var login = _faker.Random.Number(1000000, 9999999).ToString();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AlterarSenhaServicoAcessosPorTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterTokenAcessoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Erro ao gerar token de acesso."));

            // Act
            Func<Task> act = async () => await _sut.Executar(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro ao gerar token de acesso.");
            _mediatorMock.Verify(m => m.Send(It.IsAny<AlterarSenhaServicoAcessosPorTokenCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterTokenAcessoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
