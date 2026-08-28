using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Email
{
    public class EnviarEmailCancelarInscricaoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioInscricao> _repositorioInscricao;
        private readonly Mock<IMediator> _mediator;
        private readonly EnviarEmailCancelarInscricaoCommandHandler _sut;

        public EnviarEmailCancelarInscricaoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();

            _repositorioInscricao = mocker.GetMock<IRepositorioInscricao>();
            _mediator = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<EnviarEmailCancelarInscricaoCommandHandler>();
        }

        [Fact]
        public async Task DadoEmailEMotivoPreenchidos_QuandoProcessarComando_EntaoDevePublicarNaFilaComMotivoERetornarTrue()
        {
            // Arrange
            var comando = new EnviarEmailCancelarInscricaoCommand(1, "Conflito de agenda");
            var dadosEmail = CriarDadosInscricao("valido@teste.com", "Formação XPTO", "João Silva");

            ConfigurarRetornoRepositorio(comando.InscricaoId, dadosEmail);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mediator.Verify(m => m.Send(
                It.Is<PublicarNaFilaRabbitCommand>(c =>
                    c.Rota == RotasRabbit.EnviarEmail &&
                    ValidarDtoEmail((EnviarEmailDto)c.Filtros, "valido@teste.com", "Cancelamento de inscrição | Formação Formação XPTO ", "Conflito de agenda")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoEmailPreenchidoEMotivoVazio_QuandoProcessarComando_EntaoDevePublicarNaFilaSemMotivoERetornarTrue()
        {
            // Arrange
            var comando = new EnviarEmailCancelarInscricaoCommand(1, string.Empty);
            var dadosEmail = CriarDadosInscricao("valido@teste.com", "Formação ABC", "Maria Silva");

            ConfigurarRetornoRepositorio(comando.InscricaoId, dadosEmail);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mediator.Verify(m => m.Send(
                It.Is<PublicarNaFilaRabbitCommand>(c =>
                    c.Rota == RotasRabbit.EnviarEmail &&
                    ValidarDtoEmailSemMotivo((EnviarEmailDto)c.Filtros)),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoEmailVazio_QuandoProcessarComando_EntaoNaoDevePublicarNaFilaERetornarTrue()
        {
            // Arrange
            var comando = new EnviarEmailCancelarInscricaoCommand(1, "Motivo irrelevante");
            var dadosEmail = CriarDadosInscricao(string.Empty, "Formação Teste", "Sem Email");

            ConfigurarRetornoRepositorio(comando.InscricaoId, dadosEmail);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mediator.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #region Factory Methods

        private static List<DadosEmailInscricaoDto> CriarDadosInscricao(string email, string nomeFormacao, string nomeDestinatario)
        {
            return
            [
                new()
                {
                    Email = email,
                    NomeFormacao = nomeFormacao,
                    NomeDestinatario = nomeDestinatario
                }
            ];
        }

        private void ConfigurarRetornoRepositorio(long inscricaoId, List<DadosEmailInscricaoDto> retorno)
        {
            _repositorioInscricao
                .Setup(r => r.ObterDadosEmailInscricaoPorInscricaoId(inscricaoId))
                .ReturnsAsync(retorno);
        }

        private static bool ValidarDtoEmail(EnviarEmailDto dto, string emailEsperado, string tituloEsperado, string motivoEsperado)
        {
            return dto.EmailDestinatario == emailEsperado &&
                   dto.Titulo == tituloEsperado &&
                   dto.Texto.Contains($"<p>Motivo: {motivoEsperado}</p>");
        }

        private static bool ValidarDtoEmailSemMotivo(EnviarEmailDto dto)
        {
            return !dto.Texto.Contains("<p>Motivo:");
        }

        #endregion
    }
}
