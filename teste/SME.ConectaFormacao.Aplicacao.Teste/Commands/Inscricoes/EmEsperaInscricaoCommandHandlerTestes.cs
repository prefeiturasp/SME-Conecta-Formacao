using Bogus;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Email.InscricaoEmEspera;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class EmEsperaInscricaoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioInscricao> _repositorioInscricaoMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly EmEsperaInscricaoCommandHandler _emEsperaInscricaoCommandHandler;
        private readonly Faker _faker;

        public EmEsperaInscricaoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioInscricaoMock = mocker.GetMock<IRepositorioInscricao>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _emEsperaInscricaoCommandHandler = mocker.CreateInstance<EmEsperaInscricaoCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoInscricaoNull_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new EmEsperaInscricaoCommand(_faker.Random.Long(1, 1000));
            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _emEsperaInscricaoCommandHandler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoUmaInscricaoExcluida_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new EmEsperaInscricaoCommand(_faker.Random.Long(1, 1000));
            var inscricao = new Inscricao
            {
                Excluido = true
            };
            _repositorioInscricaoMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(inscricao);
            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _emEsperaInscricaoCommandHandler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoUmaInscricaoComSituacaoDiferenteDeAguardandoAnalise_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new EmEsperaInscricaoCommand(_faker.Random.Long(1, 1000));
            var inscricao = new Inscricao
            {
                Excluido = false,
                Situacao = SituacaoInscricao.Confirmada
            };
            _repositorioInscricaoMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(inscricao);
            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _emEsperaInscricaoCommandHandler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoUmaInscricaoValida_QuandoExecutarHandle_DeveAtualizarStatusEEnviarEmail()
        {
            // Arrange
            var comando = new EmEsperaInscricaoCommand(_faker.Random.Long(1, 1000));
            var inscricao = new Inscricao
            {
                Excluido = false,
                Situacao = SituacaoInscricao.AguardandoAnalise
            };
            _repositorioInscricaoMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(inscricao);

            // Act
            var resultado = await _emEsperaInscricaoCommandHandler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _repositorioInscricaoMock.Verify(r => r.Atualizar(It.Is<Inscricao>(i => i.Situacao == SituacaoInscricao.EmEspera)), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarEmailInscricaoEmEsperaCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}