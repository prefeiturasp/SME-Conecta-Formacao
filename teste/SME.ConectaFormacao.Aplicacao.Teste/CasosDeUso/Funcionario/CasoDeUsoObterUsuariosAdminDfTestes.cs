using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Funcionario;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterUsuariosAdminDfTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterUsuariosAdminDf _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterUsuariosAdminDfTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterUsuariosAdminDf>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoRequisicaoValida_QuandoExecutar_EntaoDeveConsultarUsuariosPorPerfilAdminDfERetornarLista()
        {
            // Arrange
            var usuariosEsperados = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = "1234567", Nome = _faker.Person.FullName },
                new() { Login = "7654321", Nome = _faker.Person.FullName }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuariosPorPerfilQuery>(q => q.Perfis != null && q.Perfis.Contains(Perfis.ADMIN_DF)), default))
                .ReturnsAsync(usuariosEsperados);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(usuariosEsperados);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterUsuariosPorPerfilQuery>(q => q.Perfis != null && q.Perfis.Length == 1 && q.Perfis[0] == Perfis.ADMIN_DF),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoConsultaSemRegistros_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuariosPorPerfilQuery>(q => q.Perfis != null && q.Perfis.Contains(Perfis.ADMIN_DF)), default))
                .ReturnsAsync(Enumerable.Empty<RetornoUsuarioLoginNomeDTO>());

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterUsuariosPorPerfilQuery>(q => q.Perfis != null && q.Perfis.Length == 1 && q.Perfis[0] == Perfis.ADMIN_DF),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var mensagemErro = _faker.Lorem.Sentence();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuariosPorPerfilQuery>(), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
        }
    }
}
