using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterAreaPromotoraListaRedeParceriaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterAreaPromotoraListaRedeParceria _sut;

        public CasoDeUsoObterAreaPromotoraListaRedeParceriaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoObterAreaPromotoraListaRedeParceria>();
        }

        [Fact]
        public async Task DadoConsulta_QuandoExecutar_EntaoDeveRetornarListaDeAreaPromotoraRedeParceria()
        {
            // Arrange
            var listaEsperada = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Área Parceira 1" },
                new() { Id = 2, Descricao = "Área Parceira 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterAreaPromotoraListaQuery>(q => q.AreaPromotoraIdUsuarioLogado == null && q.Tipo == AreaPromotoraTipo.RedeParceria), default))
                .ReturnsAsync(listaEsperada);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(listaEsperada);
            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterAreaPromotoraListaQuery>(q => q.AreaPromotoraIdUsuarioLogado == null && q.Tipo == AreaPromotoraTipo.RedeParceria),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoConsultaSemRegistros_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterAreaPromotoraListaQuery>(q => q.AreaPromotoraIdUsuarioLogado == null && q.Tipo == AreaPromotoraTipo.RedeParceria), default))
                .ReturnsAsync(Enumerable.Empty<RetornoListagemDTO>());

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterAreaPromotoraListaQuery>(q => q.AreaPromotoraIdUsuarioLogado == null && q.Tipo == AreaPromotoraTipo.RedeParceria),
                default), Times.Once);
        }
    }
}
