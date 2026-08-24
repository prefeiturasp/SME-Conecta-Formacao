using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarTodasDeclaracoesCodafTestes
    {
        private readonly Mock<IRepositorioCodafDeclaracao> repositorioCodafDeclaracao;
        private readonly CasoDeUsoListarTodasDeclaracoesCodaf casoDeUso;

        public CasoDeUsoListarTodasDeclaracoesCodafTestes()
        {
            repositorioCodafDeclaracao = new Mock<IRepositorioCodafDeclaracao>(MockBehavior.Strict);

            casoDeUso = new CasoDeUsoListarTodasDeclaracoesCodaf(
                repositorioCodafDeclaracao.Object);
        }

        [Fact]
        public async Task ExecutarAsync_Deve_Consultar_Repositorio_E_Aplicar_Mascara_Nos_Documentos()
        {
            // Arrange
            var filtro = CriarFiltro();

            var cursistaRf = "1234567";
            var regenteRf = "7654321";

            var itens = new List<ListagemDeclaracoesCodafDto>
            {
                new()
                {
                    Id = 1,
                    CodigoFormacao = 100,
                    NumeroHomologacao = 200,
                    CodigoDeclaracao = 300,
                    TurmaId = 400,
                    EmissorId = 500,
                    DocumentoCursista = cursistaRf,
                    DocumentoRegente = regenteRf,
                    NomeCursista = "Cursista",
                    NomeRegente = "Regente",
                    NomeFormacao = "Formação",
                    DataEmissao = DateTime.Today,
                    TipoDeclaracao = default
                }
            };

            var retornoRepositorio =
                new ResultadoPaginado<ListagemDeclaracoesCodafDto>
                {
                    Itens = itens,
                    TotalRegistros = 1,
                    TamanhoPagina = filtro.TamanhoPagina,
                    PaginaAtual = 1
                };

            repositorioCodafDeclaracao
                .Setup(x => x.ObterTodasDeclaracoesAsync(filtro))
                .ReturnsAsync(retornoRepositorio);

            var documentoCursistaEsperado =
                ResolvedorDocumentoUsuario.FormatarValor(
                    cursistaRf,
                    TipoDocumentoUsuario.Rf);

            var documentoRegenteEsperado =
                ResolvedorDocumentoUsuario.FormatarValor(
                    regenteRf,
                    TipoDocumentoUsuario.Rf);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(filtro);

            // Assert
            Assert.NotNull(resultado);

            Assert.Equal(
                documentoCursistaEsperado,
                itens[0].DocumentoCursista);

            Assert.Equal(
                documentoRegenteEsperado,
                itens[0].DocumentoRegente);

            repositorioCodafDeclaracao.Verify(
                x => x.ObterTodasDeclaracoesAsync(filtro),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_Quando_Documentos_Estiverem_Vazios_Nao_Deve_Alterar_Valores()
        {
            // Arrange
            var filtro = CriarFiltro();

            var itens = new List<ListagemDeclaracoesCodafDto>
            {
                new()
                {
                    Id = 1,
                    DocumentoCursista = string.Empty,
                    DocumentoRegente = "   "
                }
            };

            var retornoRepositorio =
                new ResultadoPaginado<ListagemDeclaracoesCodafDto>
                {
                    Itens = itens,
                    TotalRegistros = 1,
                    TamanhoPagina = filtro.TamanhoPagina,
                    PaginaAtual = 1
                };

            repositorioCodafDeclaracao
                .Setup(x => x.ObterTodasDeclaracoesAsync(filtro))
                .ReturnsAsync(retornoRepositorio);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(filtro);

            // Assert
            Assert.NotNull(resultado);

            Assert.Equal(string.Empty, itens[0].DocumentoCursista);
            Assert.Equal("   ", itens[0].DocumentoRegente);

            repositorioCodafDeclaracao.Verify(
                x => x.ObterTodasDeclaracoesAsync(filtro),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_Quando_Nao_Houver_Itens_Deve_Retornar_Paginacao_Vazia()
        {
            // Arrange
            var filtro = CriarFiltro();

            var itens = new List<ListagemDeclaracoesCodafDto>();

            var retornoRepositorio =
                new ResultadoPaginado<ListagemDeclaracoesCodafDto>
                {
                    Itens = itens,
                    TotalRegistros = 0,
                    TamanhoPagina = filtro.TamanhoPagina,
                    PaginaAtual = 1
                };

            repositorioCodafDeclaracao
                .Setup(x => x.ObterTodasDeclaracoesAsync(filtro))
                .ReturnsAsync(retornoRepositorio);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(filtro);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(itens);

            repositorioCodafDeclaracao.Verify(
                x => x.ObterTodasDeclaracoesAsync(filtro),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_Quando_Repositorio_Lancar_Excecao_Deve_Propagar_Excecao()
        {
            // Arrange
            var filtro = CriarFiltro();

            var excecaoEsperada = new InvalidOperationException(
                "Erro ao consultar declarações.");

            repositorioCodafDeclaracao
                .Setup(x => x.ObterTodasDeclaracoesAsync(filtro))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var excecaoObtida = await Assert.ThrowsAsync<InvalidOperationException>(
                () => casoDeUso.ExecutarAsync(filtro));

            // Assert
            Assert.Same(excecaoEsperada, excecaoObtida);

            repositorioCodafDeclaracao.Verify(
                x => x.ObterTodasDeclaracoesAsync(filtro),
                Times.Once);
        }

        private static FiltroListagemTodasDeclaracoesCodafDto CriarFiltro()
        {
            return new FiltroListagemTodasDeclaracoesCodafDto
            {
                Pagina = 1,
                TamanhoPagina = 10
            };
        }
    }
}
