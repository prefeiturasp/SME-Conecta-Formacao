using AutoMapper;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoListarInscritosTurmaCodafConferenciaTestes
    {
        private readonly Mock<IRepositorioCodafConferenciaInscricao> _repositorioMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoListarInscritosTurmaCodafConferencia _sut;

        public CasoDeUsoListarInscritosTurmaCodafConferenciaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafConferenciaInscricao>();
            _mapperMock = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<CasoDeUsoListarInscritosTurmaCodafConferencia>();
        }

        [Fact]
        public async Task DadoTurmaValida_QuandoExecutar_EntaoDeveRetornarInscritosPaginados()
        {
            // Arrange
            long turmaId = 10;
            long codafId = 20;
            var itemRepo = new ResultadoInscritoTurmaCodafConferenciaDto { Id = 1, Nome = "Teste", Cpf = "123", Login = "123", Participou = true };
            var itemDto = new CodafConferenciaInscritoTurmaDto { Id = 1, Nome = "Teste", Documento = "123", Participou = true };

            _repositorioMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(turmaId, 1, 10, codafId))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafConferenciaDto>
                {
                    Itens = [itemRepo],
                    TotalRegistros = 1,
                    PaginaAtual = 1,
                    TamanhoPagina = 10
                });

            _mapperMock
                .Setup(m => m.Map<List<CodafConferenciaInscritoTurmaDto>>(It.IsAny<IEnumerable<ResultadoInscritoTurmaCodafConferenciaDto>>()))
                .Returns([itemDto]);

            // Act
            var resultado = await _sut.ExecutarAsync(turmaId, 1, 10, codafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados!.Items.Should().ContainSingle().Which.Should().BeEquivalentTo(itemDto);
        }
    }
}
