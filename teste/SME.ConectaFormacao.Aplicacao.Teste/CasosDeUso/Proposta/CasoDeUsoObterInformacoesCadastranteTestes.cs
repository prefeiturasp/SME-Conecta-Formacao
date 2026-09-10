using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterInformacoesCadastranteTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterInformacoesCadastrante _casoDeUso;

        public CasoDeUsoObterInformacoesCadastranteTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterInformacoesCadastrante>();
        }

        [Fact]
        public async Task DadoPropostaId_QuandoChamarExecutar_EntaoDeveRetornarInformacoesCadastrante()
        {
            // Arrange
            long? propostaId = 123;
            var informacoesRetorno = new PropostaInformacoesCadastranteDTO();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterInformacoesCadastranteQuery>(q => q.PropostaId == propostaId), default))
                .ReturnsAsync(informacoesRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(propostaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(informacoesRetorno);
            
            _mediatorMock.Verify(m => m.Send(It.Is<ObterInformacoesCadastranteQuery>(q => q.PropostaId == propostaId), default), Times.Once);
        }
        
        [Fact]
        public async Task DadoPropostaIdNulo_QuandoChamarExecutar_EntaoDevePassarNuloParaQuery()
        {
            // Arrange
            long? propostaId = null;
            var informacoesRetorno = new PropostaInformacoesCadastranteDTO();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterInformacoesCadastranteQuery>(q => q.PropostaId == null), default))
                .ReturnsAsync(informacoesRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(propostaId);

            // Assert
            resultado.Should().NotBeNull();
            
            _mediatorMock.Verify(m => m.Send(It.Is<ObterInformacoesCadastranteQuery>(q => q.PropostaId == null), default), Times.Once);
        }
    }
}
