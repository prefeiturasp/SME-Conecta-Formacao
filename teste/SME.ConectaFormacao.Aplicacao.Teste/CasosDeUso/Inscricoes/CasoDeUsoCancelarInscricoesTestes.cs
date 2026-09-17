using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoCancelarInscricoesTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoCancelarInscricoes _sut;

        public CasoDeUsoCancelarInscricoesTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoCancelarInscricoes>();
        }

        [Fact]
        public async Task DadoIdsValidos_QuandoExecutar_EntaoDeveEnviarComandosERetornarSucesso()
        {
            // Arrange
            var ids = new long[] { 2, 1 };
            var motivo = "Desistência";

            // Act
            var resultado = await _sut.Executar(ids, motivo);

            // Assert
            resultado.Mensagem.Should().Be(MensagemNegocio.INSCRICOES_CANCELADAS_COM_SUCESSO);

            _mediatorMock.Verify(m => m.Send(It.Is<CancelarInscricaoCommand>(c => c.Id == 1 && c.Motivo == motivo), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<CancelarInscricaoCommand>(c => c.Id == 2 && c.Motivo == motivo), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoApenasUmIdComErro_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var ids = new long[] { 1 };
            var motivo = "Motivo";
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CancelarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Erro ao cancelar"));

            // Act
            Func<Task> act = async () => await _sut.Executar(ids, motivo);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage("*Erro ao cancelar*");
        }

        [Fact]
        public async Task DadoMultiplosIdsComUmErro_QuandoExecutar_EntaoDeveRetornarInconsistencias()
        {
            // Arrange
            var ids = new long[] { 1, 2 };
            var motivo = "Motivo";
            _mediatorMock
                .Setup(m => m.Send(It.Is<CancelarInscricaoCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Erro ao cancelar 1"));

            // Act
            var resultado = await _sut.Executar(ids, motivo);

            // Assert
            resultado.Mensagem.Should().Be(MensagemNegocio.INSCRICOES_CANCELADAS_COM_INCONSISTENCIAS);
            _mediatorMock.Verify(m => m.Send(It.Is<CancelarInscricaoCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<CancelarInscricaoCommand>(c => c.Id == 2), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
