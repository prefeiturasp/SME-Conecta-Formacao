using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.AlterarNomeSocialServicoAcessos;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoUsuarioAlterarNomeSocialTestes
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly CasoDeUsoUsuarioAlterarNomeSocial sut;

        public CasoDeUsoUsuarioAlterarNomeSocialTestes()
        {
            mediatorMock = new Mock<IMediator>();
            sut = new CasoDeUsoUsuarioAlterarNomeSocial(mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_Quando_nome_social_for_informado_Deve_enviar_command_e_retornar_verdadeiro()
        {
            const string login = "52998224725";
            const string nomeSocial = "Maria da Silva";

            mediatorMock
                .Setup(m => m.Send(
                    It.Is<AlterarNomeSocialServicoAcessosCommand>(command =>
                        command.Login == login &&
                        command.NomeSocial == nomeSocial),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await sut.Executar(login, nomeSocial);

            Assert.True(resultado);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<AlterarNomeSocialServicoAcessosCommand>(command =>
                        command.Login == login &&
                        command.NomeSocial == nomeSocial),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Executar_Quando_nome_social_for_nulo_Deve_encaminhar_nulo_ao_command()
        {
            const string login = "52998224725";

            mediatorMock
                .Setup(m => m.Send(
                    It.Is<AlterarNomeSocialServicoAcessosCommand>(command =>
                        command.Login == login &&
                        command.NomeSocial == null),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await sut.Executar(login, null);

            Assert.True(resultado);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<AlterarNomeSocialServicoAcessosCommand>(command =>
                        command.Login == login &&
                        command.NomeSocial == null),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Executar_Quando_command_retornar_falso_Deve_retornar_verdadeiro_pois_resultado_e_ignorado()
        {
            const string login = "52998224725";
            const string nomeSocial = "Maria da Silva";

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarNomeSocialServicoAcessosCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var resultado = await sut.Executar(login, nomeSocial);

            Assert.True(resultado);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<AlterarNomeSocialServicoAcessosCommand>(command =>
                        command.Login == login &&
                        command.NomeSocial == nomeSocial),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Executar_Quando_mediator_lancar_excecao_Deve_propagar_excecao()
        {
            const string login = "52998224725";
            const string nomeSocial = "Maria da Silva";
            var excecaoEsperada = new InvalidOperationException("Erro no serviço de acessos");

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarNomeSocialServicoAcessosCommand>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            var excecaoObtida = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.Executar(login, nomeSocial));

            Assert.Same(excecaoEsperada, excecaoObtida);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<AlterarNomeSocialServicoAcessosCommand>(command =>
                        command.Login == login &&
                        command.NomeSocial == nomeSocial),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.VerifyNoOtherCalls();
        }
    }
}
