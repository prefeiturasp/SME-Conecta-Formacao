using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CargoFuncao
{
    public class CasoDeUsoObterCargoFuncaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterCargoFuncao _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterCargoFuncaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterCargoFuncao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoParametrosValidos_QuandoExecutar_EntaoDeveRetornarCargosFuncoes()
        {
            // Arrange
            var tipo = CargoFuncaoTipo.Cargo;
            var exibirOpcaoOutros = true;
            var cargosFuncoesEsperados = new List<CargoFuncaoDto> 
            { 
                new CargoFuncaoDto() 
            };

            _mediatorMock.Setup(m => m.Send(
                It.Is<ObterCargoFuncaoPorTipoQuery>(q => q.Tipo == tipo && q.ExibirOutros == exibirOpcaoOutros), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoesEsperados);

            // Act
            var resultado = await _casoDeUso.Executar(tipo, exibirOpcaoOutros);

            // Assert
            resultado.Should().BeEquivalentTo(cargosFuncoesEsperados);
            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterCargoFuncaoPorTipoQuery>(q => q.Tipo == tipo && q.ExibirOutros == exibirOpcaoOutros), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
