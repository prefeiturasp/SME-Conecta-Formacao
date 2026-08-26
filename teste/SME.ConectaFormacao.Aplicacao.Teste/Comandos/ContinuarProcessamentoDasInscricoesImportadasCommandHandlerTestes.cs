using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoArquivo.AlterarSituacaoArquivosParaAguardandoProcessamento;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos
{
    public class ContinuarProcessamentoDasInscricoesImportadasCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ContinuarProcessamentoDasInscricoesImportadasCommandHandler _sut;

        public ContinuarProcessamentoDasInscricoesImportadasCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ContinuarProcessamentoDasInscricoesImportadasCommandHandler>();
        }

        [Fact]
        public async Task DadoArquivoNaoEncontrado_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            var comando = new ContinuarProcessamentoDasInscricoesImportadasCommand(1);

            // Act
            Func<Task> acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<NegocioException>();
        }

        [Fact]
        public async Task DadoArquivoComSituacaoInvalida_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            var comando = new ContinuarProcessamentoDasInscricoesImportadasCommand(1);
            var arquivo = new ImportacaoArquivo { Situacao = SituacaoImportacaoArquivo.Validando };

            _mocker.GetMock<IRepositorioImportacaoArquivo>()
                .Setup(m => m.ObterPorId(1))
                .ReturnsAsync(arquivo);

            // Act
            Func<Task> acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<NegocioException>();
        }

        [Fact]
        public async Task DadoArquivoValidado_QuandoExecutar_EntaoAtualizaSituacaoEPublicaFila()
        {
            // Arrange
            var comando = new ContinuarProcessamentoDasInscricoesImportadasCommand(1);
            var arquivo = new ImportacaoArquivo { Id = 1, Situacao = SituacaoImportacaoArquivo.Validado };

            _mocker.GetMock<IRepositorioImportacaoArquivo>()
                .Setup(m => m.ObterPorId(1))
                .ReturnsAsync(arquivo);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            arquivo.Situacao.Should().Be(SituacaoImportacaoArquivo.AguardandoProcessamento);
            _mocker.GetMock<IRepositorioImportacaoArquivo>().Verify(m => m.Atualizar(arquivo), Times.Once);
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
