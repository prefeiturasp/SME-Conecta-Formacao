using AutoMapper;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    [ExcludeFromCodeCoverage]
    public class ObterRegentesPaginadoQueryHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ObterRegentesPaginadoQueryHandler _sut;

        public ObterRegentesPaginadoQueryHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ObterRegentesPaginadoQueryHandler>();
        }

        [Fact]
        public async Task DadoZeroRegistros_QuandoExecutar_EntaoRetornaListaVazia()
        {
            // Arrange
            var query = new ObterRegentesPaginadoQuery(1, 10, 1);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterTotalRegentes(1))
                .ReturnsAsync(0);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<IEnumerable<PropostaRegenteDTO>>(It.IsAny<IEnumerable<PropostaRegente>>()))
                .Returns([]);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Items.Should().BeEmpty();
            resultado.TotalRegistros.Should().Be(0);
        }

        [Fact]
        public async Task DadoRegistrosExistentes_QuandoExecutar_EntaoRetornaRegentesComTurmas()
        {
            // Arrange
            var query = new ObterRegentesPaginadoQuery(1, 10, 1);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterTotalRegentes(1))
                .ReturnsAsync(1);

            var regentes = new List<PropostaRegente>
            {
                new() { Id = 10, NomeRegente = "Regente 1" }
            };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterRegentesPaginado(10, 1, 1))
                .ReturnsAsync(regentes);

            var turmas = new List<PropostaRegenteTurma>
            {
                new() { Id = 100, PropostaRegenteId = 10 }
            };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterRegenteTurmasPorRegenteId(new[] { 10L }))
                .ReturnsAsync(turmas);

            var regentesDto = new List<PropostaRegenteDTO>
            {
                new() { Id = 10, NomeRegente = "Regente 1" }
            };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<IEnumerable<PropostaRegenteDTO>>(regentes))
                .Returns(regentesDto);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalRegistros.Should().Be(1);
            resultado.Items.Should().HaveCount(1);
            resultado.Items.First().Id.Should().Be(10);
        }
    }
}
