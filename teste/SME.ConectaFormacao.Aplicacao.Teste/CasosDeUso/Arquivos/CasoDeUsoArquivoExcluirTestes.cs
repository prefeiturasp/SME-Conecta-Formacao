using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Arquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Arquivos
{
    public class CasoDeUsoArquivoExcluirTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoArquivoExcluir _casoDeUso;

        public CasoDeUsoArquivoExcluirTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoArquivoExcluir>();
        }

        [Fact]
        public async Task DadoNenhumArquivoEncontrado_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var codigos = new Guid[] { Guid.NewGuid() };
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterArquivosPorCodigosQuery>(q => q.Codigos == codigos), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<SME.ConectaFormacao.Dominio.Entidades.Arquivo>());

            // Act
            var act = async () => await _casoDeUso.Executar(codigos);

            // Assert
            var ex = await act.Should().ThrowAsync<NegocioException>();
            ex.WithMessage(MensagemNegocio.ARQUIVO_NENHUM_ARQUIVO_ENCONTRADO);
        }

        [Fact]
        public async Task DadoArquivosEncontrados_QuandoExecutar_EntaoDeveRemoverArquivosERetornarTrue()
        {
            // Arrange
            var codigos = new Guid[] { Guid.NewGuid() };
            var arquivos = new List<SME.ConectaFormacao.Dominio.Entidades.Arquivo> { new SME.ConectaFormacao.Dominio.Entidades.Arquivo() };
            
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterArquivosPorCodigosQuery>(q => q.Codigos == codigos), It.IsAny<CancellationToken>()))
                .ReturnsAsync(arquivos);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverArquivosCommand>(c => c.Arquivos == arquivos), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(codigos);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverArquivosCommand>(c => c.Arquivos == arquivos), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
