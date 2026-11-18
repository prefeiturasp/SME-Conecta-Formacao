using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class ValidarEncontrosProfissionaisAoEnviarDfCommandHandlerTestes
    {
        private readonly Mock<IRepositorioProposta> _repositorioProposta = new();
        private readonly Mock<IRepositorioPropostaEncontro> _repositorioPropostaEncontro = new();
        private readonly ValidarEncontrosProfissionaisAoEnviarDfCommandHandler _sut;

        public ValidarEncontrosProfissionaisAoEnviarDfCommandHandlerTestes()
        {
            _sut = new ValidarEncontrosProfissionaisAoEnviarDfCommandHandler(
                _repositorioProposta.Object,
                _repositorioPropostaEncontro.Object);
        }

        [Fact]
        public async Task DadoPropostaConferenciaSemRegentes_QuandoValidar_NaoDeveRetornarErroDeRegente()
        {
            // Arrange
            var proposta = new Proposta
            {
                Id = 1,
                TipoFormacao = TipoFormacao.Conferencia,
                QuantidadeTurmas = 2
            };

            _repositorioProposta
                .Setup(r => r.ObterTotalRegentes(proposta.Id))
                .ReturnsAsync(0);

            _repositorioPropostaEncontro
                .Setup(r => r.ObterQuantidadeDeTurmasComEncontroAsync(proposta.Id))
                .ReturnsAsync(2);

            var comando = new ValidarEncontrosProfissionaisAoEnviarDfCommand(proposta);

            // Act
            var erros = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            erros.Should().NotContain(MensagemNegocio.QUANTIDADE_TURMAS_COM_REGENTE);
            erros.Should().BeEmpty();
        }

        [Theory]
        [InlineData(TipoFormacao.Curso)]
        [InlineData(TipoFormacao.Evento)]
        public async Task DadoPropostaCursoOuEventoSemRegentes_QuandoValidar_DeveRetornarErroDeRegente(TipoFormacao tipoFormacao)
        {
            // Arrange
            var proposta = new Proposta
            {
                Id = 1,
                TipoFormacao = tipoFormacao,
                QuantidadeTurmas = 2
            };

            _repositorioProposta
                .Setup(r => r.ObterTotalRegentes(proposta.Id))
                .ReturnsAsync(0);

            _repositorioPropostaEncontro
                .Setup(r => r.ObterQuantidadeDeTurmasComEncontroAsync(proposta.Id))
                .ReturnsAsync(2);

            var comando = new ValidarEncontrosProfissionaisAoEnviarDfCommand(proposta);

            // Act
            var erros = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            erros.Should().Contain(MensagemNegocio.QUANTIDADE_TURMAS_COM_REGENTE);
        }

        [Fact]
        public async Task DadoQuantidadeTurmasComEncontroDiferente_QuandoValidar_DeveRetornarErroDeEncontro()
        {
            // Arrange
            var proposta = new Proposta
            {
                Id = 1,
                TipoFormacao = TipoFormacao.Conferencia,
                QuantidadeTurmas = 2
            };

            _repositorioProposta
                .Setup(r => r.ObterTotalRegentes(proposta.Id))
                .ReturnsAsync(0);

            _repositorioPropostaEncontro
                .Setup(r => r.ObterQuantidadeDeTurmasComEncontroAsync(proposta.Id))
                .ReturnsAsync(1);

            var comando = new ValidarEncontrosProfissionaisAoEnviarDfCommand(proposta);

            // Act
            var erros = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            erros.Should().Contain(MensagemNegocio.QUANTIDADE_TURMAS_COM_ENCONTRO_DIFERENTE_QUANTIDADE_DE_TURMAS);
        }
    }
}
