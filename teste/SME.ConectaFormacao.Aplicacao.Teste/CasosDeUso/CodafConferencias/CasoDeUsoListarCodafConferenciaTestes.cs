using AutoMapper;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoListarCodafConferenciaTestes
    {
        private readonly Mock<IRepositorioCodafConferencia> _repositorioMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoListarCodafConferencia _sut;

        public CasoDeUsoListarCodafConferenciaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafConferencia>();
            _mapperMock = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<CasoDeUsoListarCodafConferencia>();
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoExecutar_EntaoDeveRetornarListaPaginada()
        {
            // Arrange
            var filtro = new FiltroCodafConferenciaDto
            {
                NomeFormacao = "Conferência",
                CodigoFormacao = 10,
                NumeroHomologacao = 2026,
                PropostaTurmaId = 1,
                AreaPromotoraId = 2,
                Status = StatusCodafConferencia.Iniciado,
                DataCriacao = DateTime.Today,
                NumeroPagina = 1,
                NumeroRegistros = 10
            };

            var filtroRepo = new FiltroListagemResultadoCodafConferenciaDto
            {
                Pagina = 1,
                TamanhoPagina = 10
            };

            var itemRepo = new ListagemResultadoCodafConferenciaDto
            {
                Id = 1,
                NomeTurma = "Turma 1",
                NomeAreaPromotora = "DRE"
            };

            var itemDto = new CodafConferenciaResumoDto
            {
                Id = 1,
                NomeTurma = "Turma 1",
                NomeAreaPromotora = "DRE"
            };

            _mapperMock
                .Setup(m => m.Map<FiltroListagemResultadoCodafConferenciaDto>(filtro))
                .Returns(filtroRepo);

            _repositorioMock
                .Setup(r => r.ObterListagemPorFiltroAsync(filtroRepo))
                .ReturnsAsync(new ResultadoPaginado<ListagemResultadoCodafConferenciaDto>
                {
                    Itens = [itemRepo],
                    TotalRegistros = 1,
                    PaginaAtual = 1,
                    TamanhoPagina = 10
                });

            _mapperMock
                .Setup(m => m.Map<List<CodafConferenciaResumoDto>>(It.IsAny<IEnumerable<ListagemResultadoCodafConferenciaDto>>()))
                .Returns([itemDto]);

            // Act
            var resultado = await _sut.ExecutarAsync(filtro);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.TotalRegistros.Should().Be(1);
            resultado.Dados.Items.Should().ContainSingle().Which.Id.Should().Be(1);
        }
    }
}
