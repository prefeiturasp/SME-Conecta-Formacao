using AutoMapper;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoPesquisarCursistaPorPropostaTurmaIdTestes
    {
        private readonly Mock<IRepositorioInscricao> repositorioMock;
        private readonly Mock<IMapper> mapperMock;

        private readonly CasoDeUsoPesquisarCursistaPorPropostaTurmaId casoDeUso;

        public CasoDeUsoPesquisarCursistaPorPropostaTurmaIdTestes()
        {
            repositorioMock = new Mock<IRepositorioInscricao>();
            mapperMock = new Mock<IMapper>();

            casoDeUso = new CasoDeUsoPesquisarCursistaPorPropostaTurmaId(
                repositorioMock.Object,
                mapperMock.Object);
        }

        [Fact]
        public async Task Deve_pesquisar_cursistas_por_proposta_turma()
        {
            // Arrange
            const long propostaTurmaId = 100;
            const string termoBusca = "João";
            const int pagina = 2;
            const int registros = 15;

            var itensRepositorio = new List<InscricaoDadosCursistaDto>
            {
                new(),
                new()
            };

            var itensRetorno = new List<DadosInscricaoCursistaRetornoDto>
            {
                new(),
                new()
            };

            var paginacaoRepositorio = new ResultadoPaginado<InscricaoDadosCursistaDto>
            {
                Itens = itensRepositorio,
                TotalRegistros = 20,
                TamanhoPagina = registros
            };

            repositorioMock
                .Setup(x => x.PesquisarCursistaPorPropostaTurmaIdAsync(
                    propostaTurmaId,
                    termoBusca,
                    pagina,
                    registros))
                .ReturnsAsync(paginacaoRepositorio);

            mapperMock
                .Setup(x => x.Map<IEnumerable<DadosInscricaoCursistaRetornoDto>>(itensRepositorio))
                .Returns(itensRetorno);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(
                propostaTurmaId,
                termoBusca,
                pagina,
                registros);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);

            Assert.Equal(20, resultado.Dados.TotalRegistros);
            Assert.Equal(2, resultado.Dados.TotalPaginas);
            Assert.Equal(2, resultado.Dados.Items.Count());

            repositorioMock.Verify(x =>
                x.PesquisarCursistaPorPropostaTurmaIdAsync(
                    propostaTurmaId,
                    termoBusca,
                    pagina,
                    registros),
                Times.Once);

            mapperMock.Verify(x =>
                x.Map<IEnumerable<DadosInscricaoCursistaRetornoDto>>(itensRepositorio),
                Times.Once);
        }
    }
}
