using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioMeusDadosTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioMeusDados _sut;
        private readonly Faker _faker;

        public CasoDeUsoUsuarioMeusDadosTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoUsuarioMeusDados>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoLoginValido_QuandoExecutar_EntaoDeveEnviarQueryERetornarDadosUsuario()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var dadosUsuarioEsperados = new DadosUsuarioDTO
            {
                Login = login,
                Nome = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Cpf = "12345678900",
                NomeUnidade = _faker.Company.CompanyName()
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterMeusDadosServicoAcessosPorLoginQuery>(q => q.Login == login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dadosUsuarioEsperados);

            // Act
            var resultado = await _sut.Executar(login);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(dadosUsuarioEsperados);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterMeusDadosServicoAcessosPorLoginQuery>(q => q.Login == login), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterMeusDadosServicoAcessosPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Erro no serviço de acessos."));

            // Act
            Func<Task> act = async () => await _sut.Executar(login);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro no serviço de acessos.");
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterMeusDadosServicoAcessosPorLoginQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
