using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class SortearInscricaoCommandHandlerTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRepositorioInscricao> _repositorioInscricaoMock;
        private readonly SortearInscricaoCommandHandler _sut;

        public SortearInscricaoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _repositorioInscricaoMock = mocker.GetMock<IRepositorioInscricao>();

            _sut = mocker.CreateInstance<SortearInscricaoCommandHandler>();
        }

        [Fact]
        public async Task DadoTurmaNaoEncontrada_QuandoChamarHandle_EntaoLancaNegocioException()
        {
            // Arrange
            var comando = new SortearInscricaoCommand(1);

            // Act
            var acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.TURMA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaQueNaoPermiteSorteio_QuandoChamarHandle_EntaoLancaNegocioException()
        {
            // Arrange
            var comando = new SortearInscricaoCommand(1);
            var turma = new PropostaTurma { Id = 1, PropostaId = 10 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turma);

            var dadosTurmas = new List<ListagemFormacaoComTurmaDTO> { new() { PermiteSorteio = false } };
            _repositorioInscricaoMock.Setup(r => r.DadosListagemFormacaoComTurma(new long[] { 10 }, 1))
                .ReturnsAsync(dadosTurmas);

            // Act
            var acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_PERMITE_SORTEIO);
        }

        [Fact]
        public async Task DadoTurmaSemVagasDisponiveis_QuandoChamarHandle_EntaoLancaNegocioException()
        {
            // Arrange
            var comando = new SortearInscricaoCommand(1);
            var turma = new PropostaTurma { Id = 1, PropostaId = 10 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turma);

            var dadosTurmas = new List<ListagemFormacaoComTurmaDTO> { new ListagemFormacaoComTurmaDTO { PermiteSorteio = true, Disponiveis = 0 } };
            _repositorioInscricaoMock.Setup(r => r.DadosListagemFormacaoComTurma(new long[] { 10 }, 1))
                .ReturnsAsync(dadosTurmas);

            // Act
            var acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_TURMA_NAO_POSSUI_VAGA_DISPONIVEL_PARA_SORTEIO);
        }

        [Fact]
        public async Task DadoInscricoesAguardandoAnalise_QuandoChamarHandle_EntaoDeveSortearConfirmarECancelarRestante()
        {
            // Arrange
            var comando = new SortearInscricaoCommand(1);
            var turma = new PropostaTurma { Id = 1, PropostaId = 10 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turma);

            // Uma vaga disponivel, mas 2 inscricoes aguardando
            var dadosTurmas = new List<ListagemFormacaoComTurmaDTO> { new ListagemFormacaoComTurmaDTO { PermiteSorteio = true, Disponiveis = 1 } };
            _repositorioInscricaoMock.Setup(r => r.DadosListagemFormacaoComTurma(new long[] { 10 }, 1))
                .ReturnsAsync(dadosTurmas);

            var aguardando = new List<long> { 100, 200 };
            _repositorioInscricaoMock.Setup(r => r.ObterIdsInscricoesAguardandoAnalise(1))
                .ReturnsAsync(aguardando);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            // Devemos ter 1 ConfirmarInscricaoCommand e 1 CancelarInscricaoCommand
            _mediatorMock.Verify(m => m.Send(It.IsAny<ConfirmarInscricaoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<CancelarInscricaoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
