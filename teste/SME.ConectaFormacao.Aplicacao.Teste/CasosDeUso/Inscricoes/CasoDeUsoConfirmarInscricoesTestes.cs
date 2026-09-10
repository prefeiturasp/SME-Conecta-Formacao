using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.ConfirmarInscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoConfirmarInscricoesTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoConfirmarInscricoes _sut;

        public CasoDeUsoConfirmarInscricoesTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoConfirmarInscricoes>();
        }

        [Fact]
        public async Task DadoIdsValidos_QuandoExecutar_EntaoDeveEnviarComandosERetornarSucesso()
        {
            // Arrange
            var ids = new long[] { 2, 1 };

            // Act
            var resultado = await _sut.Executar(ids);

            // Assert
            resultado.Mensagem.Should().Be(MensagemNegocio.INSCRICOES_CONFIRMADAS_COM_SUCESSO);
            
            _mediatorMock.Verify(m => m.Send(It.Is<ConfirmarInscricaoCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ConfirmarInscricaoCommand>(c => c.Id == 2), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoApenasUmIdComErro_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var ids = new long[] { 1 };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ConfirmarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Erro ao confirmar"));

            // Act
            Func<Task> act = async () => await _sut.Executar(ids);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage("*Erro ao confirmar*");
        }

        [Fact]
        public async Task DadoMultiplosIdsComUmErro_QuandoExecutar_EntaoDeveRetornarFaltaDeVaga()
        {
            // Arrange
            var ids = new long[] { 1, 2 };
            _mediatorMock
                .Setup(m => m.Send(It.Is<ConfirmarInscricaoCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Erro ao confirmar 1"));

            // Act
            var resultado = await _sut.Executar(ids);

            // Assert
            resultado.Mensagem.Should().Be(MensagemNegocio.INSCRICOES_NAO_CONFIRMADAS_POR_FALTA_DE_VAGA);
            _mediatorMock.Verify(m => m.Send(It.Is<ConfirmarInscricaoCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ConfirmarInscricaoCommand>(c => c.Id == 2), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
