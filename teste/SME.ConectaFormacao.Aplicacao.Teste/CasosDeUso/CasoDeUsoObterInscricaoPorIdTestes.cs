using AutoMapper;
using Bogus;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterInscricaoPorIdTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly Mock<IRepositorioInscricao> _repositorioInscricaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoObterInscricaoPorId _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterInscricaoPorIdTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _contextoAplicacaoMock = new Mock<IContextoAplicacao>();
            _repositorioInscricaoMock = new Mock<IRepositorioInscricao>();
            _mapperMock = new Mock<IMapper>();
            _faker = new Faker("pt_BR");

            _casoDeUso = new(
                _mediatorMock.Object,
                _contextoAplicacaoMock.Object,
                _repositorioInscricaoMock.Object,
                _mapperMock.Object
            )
            {
                // Define paginação padrão da classe base para os testes
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
        }

        [Fact]
        public async Task Dado_RepositorioRetornaVazio_Quando_ExecutarAsync_Entao_RetornarDtoVazioSemChamarAnexos()
        {
            // Arrange
            var filtro = GerarFiltroValido();

            // Simula retorno vazio do repositório (TotalRegistros = 0)
            var retornoVazio = new Infra.Dados.Dtos.ResultadoPaginado<Inscricao>();

            _repositorioInscricaoMock
                .Setup(r => r.ObterInscricoesPorPropostaPaginadasAsync(filtro))
                .ReturnsAsync(retornoVazio);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(filtro);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado.Items);
            Assert.Equal(0, resultado.TotalRegistros);

            _repositorioInscricaoMock.Verify(r => r.ObterSeInscricaoPossuiAnexoPorPropostasIds(It.IsAny<long[]>()), Times.Never);
            _mapperMock.Verify(m => m.Map<IEnumerable<DadosListagemInscricaoDto>>(It.IsAny<IEnumerable<Inscricao>>()), Times.Never);
        }

        [Fact]
        public async Task Dado_RepositorioRetornaRegistrosComAnexos_Quando_ExecutarAsync_Entao_RetornarMapeadoComAnexos()
        {
            // Arrange
            var filtro = GerarFiltroValido();
            var idInscricao = _faker.Random.Long(1, 1000);

            // 1. Mock das Entidades de Retorno
            var inscricaoEntidade = new Inscricao { Id = idInscricao };
            var listaInscricoes = new List<Inscricao> { inscricaoEntidade };

            var retornoPaginado = new Infra.Dados.Dtos.ResultadoPaginado<Inscricao>() { Itens = listaInscricoes, TotalRegistros = 1, TamanhoPagina = 10, PaginaAtual = 1 };

            _repositorioInscricaoMock
                .Setup(r => r.ObterInscricoesPorPropostaPaginadasAsync(filtro))
                .ReturnsAsync(retornoPaginado);

            var dtoMapeado = new DadosListagemInscricaoDto
            {
                InscricaoId = idInscricao,
                Anexos = []
            };

            _mapperMock
                .Setup(m => m.Map<IEnumerable<DadosListagemInscricaoDto>>(listaInscricoes))
                .Returns([dtoMapeado]);

            // 3. Mock dos Anexos
            var anexoDto = new InscricaoPossuiAnexoDTO
            {
                InscricaoId = idInscricao,
                NomeArquivo = "documento.pdf",
                Codigo = Guid.NewGuid()
            };

            _repositorioInscricaoMock
                .Setup(r => r.ObterSeInscricaoPossuiAnexoPorPropostasIds(It.Is<long[]>(ids => ids.Contains(idInscricao))))
                .ReturnsAsync([anexoDto]);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(filtro);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.TotalRegistros);

            var itemRetornado = resultado.Items.First();
            Assert.Equal(idInscricao, itemRetornado.InscricaoId);

            // Verifica se o anexo foi populado corretamente
            Assert.Single(itemRetornado.Anexos);
            Assert.Equal("documento.pdf", itemRetornado.Anexos.First().Nome);
            Assert.Equal(anexoDto.Codigo, itemRetornado.Anexos.First().Codigo);

            // Verificações de chamada
            _repositorioInscricaoMock.Verify(r => r.ObterInscricoesPorPropostaPaginadasAsync(filtro), Times.Once);
            _repositorioInscricaoMock.Verify(r => r.ObterSeInscricaoPossuiAnexoPorPropostasIds(It.IsAny<long[]>()), Times.Once);
        }

        [Fact]
        public async Task Dado_RepositorioRetornaRegistrosSemAnexos_Quando_ExecutarAsync_Entao_RetornarMapeadoListaAnexosVazia()
        {
            // Arrange
            var filtro = GerarFiltroValido();
            var idInscricao = _faker.Random.Long(1, 1000);

            var listaInscricoes = new List<Inscricao> { new() { Id = idInscricao } };
            var retornoPaginado = new Infra.Dados.Dtos.ResultadoPaginado<Inscricao>() { Itens = listaInscricoes, TotalRegistros = 1, TamanhoPagina = 10, PaginaAtual = 1 };

            _repositorioInscricaoMock
                .Setup(r => r.ObterInscricoesPorPropostaPaginadasAsync(filtro))
                .ReturnsAsync(retornoPaginado);

            var dtoMapeado = new DadosListagemInscricaoDto { InscricaoId = idInscricao, Anexos = [] };

            _mapperMock
                .Setup(m => m.Map<IEnumerable<DadosListagemInscricaoDto>>(listaInscricoes))
                .Returns([dtoMapeado]);

            // Mock retornando lista vazia de anexos
            _repositorioInscricaoMock
                .Setup(r => r.ObterSeInscricaoPossuiAnexoPorPropostasIds(It.IsAny<long[]>()))
                .ReturnsAsync([]);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(filtro);

            // Assert
            Assert.NotEmpty(resultado.Items);
            Assert.Empty(resultado.Items.First().Anexos);
        }

        private FiltroListagemInscricaoDto GerarFiltroValido()
        {
            return new FiltroListagemInscricaoDto
            {
                PropostaId = _faker.Random.Long(),
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
        }
    }
}