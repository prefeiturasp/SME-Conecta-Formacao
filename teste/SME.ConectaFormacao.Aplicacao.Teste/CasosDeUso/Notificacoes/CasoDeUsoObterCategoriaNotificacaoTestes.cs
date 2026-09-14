using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Notificacoes
{
    public class CasoDeUsoObterCategoriaNotificacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterCategoriaNotificacao _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterCategoriaNotificacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterCategoriaNotificacao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoExecucao_QuandoChamada_EntaoDeveRetornarCategoriasNotificacao()
        {
            // Arrange
            var categoriasEsperadas = new List<RetornoListagemDTO> 
            { 
                new RetornoListagemDTO() 
            };

            _mediatorMock.Setup(m => m.Send(
                It.IsAny<ObterNotificacaoCategoriaQuery>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoriasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.Should().BeEquivalentTo(categoriasEsperadas);
            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterNotificacaoCategoriaQuery>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
