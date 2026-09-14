using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.EmEsperaInscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoEmEsperaInscricoesTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEmEsperaInscricoes _sut;

        public CasoDeUsoEmEsperaInscricoesTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoEmEsperaInscricoes>();
        }

        [Fact]
        public async Task DadoIdsValidos_QuandoExecutar_EntaoDeveEnviarComandoERetornarSucesso()
        {
            // Arrange
            var ids = new long[] { 2, 1 };

            // Act
            var resultado = await _sut.Executar(ids);

            // Assert
            resultado.Mensagem.Should().Be(MensagemNegocio.INSCRICOES_EM_ESPERA_COM_SUCESSO);
            _mediatorMock.Verify(m => m.Send(It.Is<EmEsperaInscricaoCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<EmEsperaInscricaoCommand>(c => c.Id == 2), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoApenasUmIdComErro_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var ids = new long[] { 1 };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<EmEsperaInscricaoCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Erro na inscrição"));

            // Act
            Func<Task> act = async () => await _sut.Executar(ids);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage("*Erro na inscrição*");
        }

        [Fact]
        public async Task DadoMultiplosIdsComUmErro_QuandoExecutar_EntaoDeveRetornarInconsistencias()
        {
            // Arrange
            var ids = new long[] { 1, 2 };
            _mediatorMock
                .Setup(m => m.Send(It.Is<EmEsperaInscricaoCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Erro na inscrição 1"));

            // Act
            var resultado = await _sut.Executar(ids);

            // Assert
            resultado.Mensagem.Should().Be(MensagemNegocio.INSCRICOES_EM_ESPERA_COM_INCONSISTENCIAS);
            _mediatorMock.Verify(m => m.Send(It.Is<EmEsperaInscricaoCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<EmEsperaInscricaoCommand>(c => c.Id == 2), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
