using Bogus;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterListagemFormacaoPaginadaTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly CasoDeUsoObterListagemFormacaoPaginada _useCase;
        private readonly Faker _faker;

        public CasoDeUsoObterListagemFormacaoPaginadaTests()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _faker = new Faker("pt_BR");

            _useCase = mocker.CreateInstance<CasoDeUsoObterListagemFormacaoPaginada>();
        }

        [Fact]
        public async Task DadoFiltrosValidosEPropostasEncontradas_QuandoExecutar_EntaoDeveRetornarListaPaginadaPreenchida()
        {
            // Arrange
            var filtro = new FiltroListagemFormacaoDTO
            {
                Titulo = _faker.Lorem.Sentence(),
                DataInicial = DateTime.Now,
                FormatosIds = [1]
            };

            var idsPropostas = new List<long> { 1, 2, 3 };
            var retornoRepositorio = new Infra.Dados.Dtos.ResultadoPaginado<long>
            {
                Itens = idsPropostas,
                TotalRegistros = 10
            };

            var retornoMediator = new List<RetornoListagemFormacaoDTO>
            {
                new() { Id = 1, Titulo = "Formação 1" },
                new() { Id = 2, Titulo = "Formação 2" },
                new() { Id = 3, Titulo = "Formação 3" }
            };

            // Setup Contexto (Não Cursista)
            _contextoAplicacaoMock.Setup(x => x.PerfilUsuario).Returns(Guid.NewGuid().ToString());
            _contextoAplicacaoMock.Setup(x => x.UsuarioLogado).Returns(_faker.Internet.UserName());

            // Setup Repositório
            _repositorioPropostaMock
                .Setup(x => x.ObterListagemFormacoesPorFiltro(It.IsAny<FiltroListaFormacaoPropostaDto>()))
                .ReturnsAsync(retornoRepositorio);

            // Setup Mediator
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterPropostasPorIdsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoMediator);

            // Act
            var resultado = await _useCase.Executar(filtro);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Items.Count());
            Assert.Equal(10, resultado.TotalRegistros);

            _repositorioPropostaMock.Verify(x => x.ObterListagemFormacoesPorFiltro(It.Is<FiltroListaFormacaoPropostaDto>(f =>
                f.Titulo == filtro.Titulo &&
                f.EhPerfilCursista == false
            )), Times.Once);

            _mediatorMock.Verify(x => x.Send(It.Is<ObterPropostasPorIdsQuery>(q =>
                q.PropostasIds.SequenceEqual(idsPropostas)
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoRepositorioRetornaVazio_QuandoExecutar_EntaoNaoDeveChamarMediatorERetornarVazio()
        {
            // Arrange
            var filtro = new FiltroListagemFormacaoDTO();
            var retornoRepositorio = new Infra.Dados.Dtos.ResultadoPaginado<long>
            {
                Itens = [],
                TotalRegistros = 0
            };

            _contextoAplicacaoMock.Setup(x => x.PerfilUsuario).Returns(Guid.NewGuid().ToString());

            _repositorioPropostaMock
                .Setup(x => x.ObterListagemFormacoesPorFiltro(It.IsAny<FiltroListaFormacaoPropostaDto>()))
                .ReturnsAsync(retornoRepositorio);

            // Act
            var resultado = await _useCase.Executar(filtro);

            // Assert
            Assert.Empty(resultado.Items);
            Assert.Equal(0, resultado.TotalRegistros);

            _repositorioPropostaMock.Verify(x => x.ObterListagemFormacoesPorFiltro(It.IsAny<FiltroListaFormacaoPropostaDto>()), Times.Once);

            // Verifica se o Mediator NÃO foi chamado pois a lista estava vazia
            _mediatorMock.Verify(x => x.Send(It.IsAny<ObterPropostasPorIdsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoUsuarioPerfilCursista_QuandoExecutar_EntaoDevePassarFlagVerdadeiraParaRepositorio()
        {
            // Arrange
            var filtro = new FiltroListagemFormacaoDTO();
            var retornoRepositorio = new Infra.Dados.Dtos.ResultadoPaginado<long>
            {
                Itens = [],
                TotalRegistros = 0
            };

            // Setup Contexto com GUID do Perfil Cursista
            _contextoAplicacaoMock.Setup(x => x.IdPerfilUsuario).Returns(PerfilAutomatico.PERIL_CURSISTA_CODIGO);

            _repositorioPropostaMock
                .Setup(x => x.ObterListagemFormacoesPorFiltro(It.IsAny<FiltroListaFormacaoPropostaDto>()))
                .ReturnsAsync(retornoRepositorio);

            // Act
            await _useCase.Executar(filtro);

            // Assert
            _repositorioPropostaMock.Verify(x => x.ObterListagemFormacoesPorFiltro(It.Is<FiltroListaFormacaoPropostaDto>(f =>
                f.EhPerfilCursista == true
            )), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioPerfilNaoCursista_QuandoExecutar_EntaoDevePassarFlagFalsaParaRepositorio()
        {
            // Arrange
            var filtro = new FiltroListagemFormacaoDTO();
            var retornoRepositorio = new Infra.Dados.Dtos.ResultadoPaginado<long>
            {
                Itens = [],
                TotalRegistros = 0
            };

            // Setup Contexto com GUID aleatório
            _contextoAplicacaoMock.Setup(x => x.PerfilUsuario).Returns(Guid.NewGuid().ToString());

            _repositorioPropostaMock
                .Setup(x => x.ObterListagemFormacoesPorFiltro(It.IsAny<FiltroListaFormacaoPropostaDto>()))
                .ReturnsAsync(retornoRepositorio);

            // Act
            await _useCase.Executar(filtro);

            // Assert
            _repositorioPropostaMock.Verify(x => x.ObterListagemFormacoesPorFiltro(It.Is<FiltroListaFormacaoPropostaDto>(f =>
                f.EhPerfilCursista == false
            )), Times.Once);
        }

        [Fact]
        public async Task DadoContextoSemPerfilUsuario_QuandoExecutar_EntaoDevePassarFlagFalsaParaRepositorio()
        {
            // Arrange
            var filtro = new FiltroListagemFormacaoDTO();
            var retornoRepositorio = new Infra.Dados.Dtos.ResultadoPaginado<long>
            {
                Itens = [],
                TotalRegistros = 0
            };

            // Setup Contexto com Perfil Nulo/Vazio
            _contextoAplicacaoMock.Setup(x => x.PerfilUsuario).Returns(string.Empty);

            _repositorioPropostaMock
                .Setup(x => x.ObterListagemFormacoesPorFiltro(It.IsAny<FiltroListaFormacaoPropostaDto>()))
                .ReturnsAsync(retornoRepositorio);

            // Act
            await _useCase.Executar(filtro);

            // Assert
            _repositorioPropostaMock.Verify(x => x.ObterListagemFormacoesPorFiltro(It.Is<FiltroListaFormacaoPropostaDto>(f =>
                f.EhPerfilCursista == false
            )), Times.Once);
        }
    }
}