using FluentAssertions;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoUsuarioAlterarTelefoneTestes
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly CasoDeUsoUsuarioAlterarTelefone casoDeUso;

        public CasoDeUsoUsuarioAlterarTelefoneTestes()
        {
            mediatorMock = new Mock<IMediator>();
            casoDeUso = new CasoDeUsoUsuarioAlterarTelefone(mediatorMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_true_quando_telefone_for_alterado()
        {
            var login = "123456";
            var telefone = "11999999999";

            mediatorMock
                .Setup(x => x.Send(
                    It.Is<SalvarUsuarioTelefoneParcialCommand>(
                        c => c.Login == login &&
                             c.Telefone == telefone),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await casoDeUso.Executar(login, telefone);

            resultado.Should().BeTrue();

            mediatorMock.Verify(x => x.Send(
                It.Is<SalvarUsuarioTelefoneParcialCommand>(
                    c => c.Login == login &&
                         c.Telefone == telefone),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_retornar_false_quando_telefone_nao_for_alterado()
        {
            var login = "123456";
            var telefone = "11999999999";

            mediatorMock
                .Setup(x => x.Send(
                    It.IsAny<SalvarUsuarioTelefoneParcialCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var resultado = await casoDeUso.Executar(login, telefone);

            resultado.Should().BeFalse();

            mediatorMock.Verify(x => x.Send(
                It.Is<SalvarUsuarioTelefoneParcialCommand>(
                    c => c.Login == login &&
                         c.Telefone == telefone),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
