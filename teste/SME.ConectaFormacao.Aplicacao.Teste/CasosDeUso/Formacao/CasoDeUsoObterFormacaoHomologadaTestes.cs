using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Formacao
{
    public class CasoDeUsoObterFormacaoHomologadaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterFormacaoHomologada _sut;

        public CasoDeUsoObterFormacaoHomologadaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterFormacaoHomologada>();
        }

        [Fact]
        public async Task DadoChamada_QuandoExecutar_EntaoDeveRetornarFormacoesHomologadas()
        {
            // Arrange
            var formacoesHomologadas = new List<RetornoListagemDTO>
            {
                new RetornoListagemDTO { Id = 1, Descricao = "Formação 1" },
                new RetornoListagemDTO { Id = 2, Descricao = "Formação 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(ObterFormacaoHomologadaQuery.Instancia, default))
                .ReturnsAsync(formacoesHomologadas);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().BeEquivalentTo(formacoesHomologadas);
            
            _mediatorMock.Verify(m => m.Send(ObterFormacaoHomologadaQuery.Instancia, default), Times.Once);
        }
    }
}
