using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Contexto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterAreaPromotoraPaginadaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterAreaPromotoraPaginada _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterAreaPromotoraPaginadaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            
            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroPagina")).Returns("1");
            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroRegistros")).Returns("10");

            _sut = mocker.CreateInstance<CasoDeUsoObterAreaPromotoraPaginada>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoFiltrosValidos_QuandoExecutar_EntaoDeveRetornarAreasPromotorasPaginadas()
        {
            // Arrange
            var filtros = new AreaPromotoraFiltrosDTO();
            var resultadoPaginado = new PaginacaoResultadoDto<AreaPromotoraPaginadaDTO>(new List<AreaPromotoraPaginadaDTO>(), 0, 10);

            _mediatorMock.Setup(m => m.Send(It.Is<ObterAreasPromotorasPaginadasQuery>(q => 
                q.Filtros == filtros &&
                q.NumeroPagina == _sut.NumeroPagina &&
                q.NumeroRegistros == _sut.NumeroRegistros), default))
                .ReturnsAsync(resultadoPaginado);

            // Act
            var resultado = await _sut.Executar(filtros);

            // Assert
            resultado.Should().BeEquivalentTo(resultadoPaginado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreasPromotorasPaginadasQuery>(), default), Times.Once);
        }
    }
}
