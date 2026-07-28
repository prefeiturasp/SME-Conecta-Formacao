using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.AlterarNomeSocialServicoAcessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios
{
    public class AlterarNomeSocialServicoAcessosCommandHandlerTestes
    {
        private readonly Mock<IServicoAcessos> servicoAcessosMock;
        private readonly AlterarNomeSocialServicoAcessosCommandHandler sut;

        public AlterarNomeSocialServicoAcessosCommandHandlerTestes()
        {
            servicoAcessosMock = new Mock<IServicoAcessos>();
            sut = new AlterarNomeSocialServicoAcessosCommandHandler(
                servicoAcessosMock.Object);
        }

        [Fact]
        public async Task Handle_Quando_servico_retornar_verdadeiro_Deve_retornar_verdadeiro()
        {
            const string login = "52998224725";
            const string nomeSocial = "Maria da Silva";
            var command = new AlterarNomeSocialServicoAcessosCommand(login, nomeSocial);

            servicoAcessosMock
                .Setup(s => s.AlterarNomeSocialAsync(login, nomeSocial))
                .ReturnsAsync(true);

            var resultado = await sut.Handle(command, CancellationToken.None);

            Assert.True(resultado);
            servicoAcessosMock.Verify(
                s => s.AlterarNomeSocialAsync(login, nomeSocial),
                Times.Once);
            servicoAcessosMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Quando_servico_retornar_falso_Deve_retornar_falso()
        {
            const string login = "52998224725";
            const string nomeSocial = "Maria da Silva";
            var command = new AlterarNomeSocialServicoAcessosCommand(login, nomeSocial);

            servicoAcessosMock
                .Setup(s => s.AlterarNomeSocialAsync(login, nomeSocial))
                .ReturnsAsync(false);

            var resultado = await sut.Handle(command, CancellationToken.None);

            Assert.False(resultado);
            servicoAcessosMock.Verify(
                s => s.AlterarNomeSocialAsync(login, nomeSocial),
                Times.Once);
            servicoAcessosMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Quando_nome_social_for_nulo_Deve_encaminhar_nulo_ao_servico()
        {
            const string login = "52998224725";
            var command = new AlterarNomeSocialServicoAcessosCommand(login, null);

            servicoAcessosMock
                .Setup(s => s.AlterarNomeSocialAsync(login, null))
                .ReturnsAsync(true);

            var resultado = await sut.Handle(command, CancellationToken.None);

            Assert.True(resultado);
            servicoAcessosMock.Verify(
                s => s.AlterarNomeSocialAsync(login, null),
                Times.Once);
            servicoAcessosMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Quando_servico_lancar_excecao_Deve_propagar_excecao()
        {
            const string login = "52998224725";
            const string nomeSocial = "Maria da Silva";
            var command = new AlterarNomeSocialServicoAcessosCommand(login, nomeSocial);
            var excecaoEsperada = new InvalidOperationException("Erro ao alterar nome social");

            servicoAcessosMock
                .Setup(s => s.AlterarNomeSocialAsync(login, nomeSocial))
                .ThrowsAsync(excecaoEsperada);

            var excecaoObtida = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.Handle(command, CancellationToken.None));

            Assert.Same(excecaoEsperada, excecaoObtida);
            servicoAcessosMock.Verify(
                s => s.AlterarNomeSocialAsync(login, nomeSocial),
                Times.Once);
            servicoAcessosMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Quando_token_estiver_cancelado_Deve_encaminhar_requisicao_ao_servico()
        {
            const string login = "52998224725";
            const string nomeSocial = "Maria da Silva";
            var command = new AlterarNomeSocialServicoAcessosCommand(login, nomeSocial);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            servicoAcessosMock
                .Setup(s => s.AlterarNomeSocialAsync(login, nomeSocial))
                .ReturnsAsync(true);

            var resultado = await sut.Handle(command, cancellationTokenSource.Token);

            Assert.True(resultado);
            servicoAcessosMock.Verify(
                s => s.AlterarNomeSocialAsync(login, nomeSocial),
                Times.Once);
            servicoAcessosMock.VerifyNoOtherCalls();
        }
    }
}
