using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class PropostaControllerTests
    {
        private readonly PropostaController _controller;
        private readonly Faker _faker;

        public PropostaControllerTests()
        {
            _controller = new PropostaController();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoPropostaIdValido_QuandoObterInformacoesCadastrante_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterInformacoesCadastrante>();
            var propostaId = _faker.Random.Long();
            var dto = new PropostaInformacoesCadastranteDTO { UsuarioLogadoNome = _faker.Person.FullName };

            mockUseCase.Setup(x => x.Executar(propostaId)).ReturnsAsync(dto);

            // Act
            var resultado = await _controller.ObterInformacoesCadastrante(mockUseCase.Object, propostaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(dto, okResult.Value);
            mockUseCase.Verify(x => x.Executar(propostaId), Times.Once);
        }

        [Fact]
        public async Task DadoSolicitacaoValida_QuandoObterRoteiroPropostaFormativa_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterRoteiroPropostaFormativa>();
            var dto = new RoteiroPropostaFormativaDTO { Descricao = _faker.Lorem.Sentence() };

            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(dto);

            // Act
            var resultado = await _controller.ObterRoteiroPropostaFormativa(mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(dto, okResult.Value);
        }

        [Fact]
        public async Task DadoExibirOpcaoOutros_QuandoObterCriterioValidacaoInscricao_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterCriterioValidacaoInscricao>();
            var exibirOutros = true;
            var lista = new List<CriterioValidacaoInscricaoDTO>();

            mockUseCase.Setup(x => x.Executar(exibirOutros)).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterCriterioValidacaoInscricao(mockUseCase.Object, exibirOutros);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Same(lista, okResult.Value);
        }

        [Fact]
        public async Task DadoSolicitacaoValida_QuandoObterTipoFormacao_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterTipoFormacao>();
            var lista = new List<RetornoListagemDTO>();
            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterTipoFormacao(mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoSolicitacaoValida_QuandoObterTipoInscricao_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterTipoInscricao>();
            var lista = new List<RetornoListagemDTO>();
            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterTipoInscricao(mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoTipoFormacaoValido_QuandoObterformatos_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterFormatos>();
            var tipoFormacao = TipoFormacao.Curso;
            var lista = new List<RetornoListagemDTO>();
            mockUseCase.Setup(x => x.Executar(tipoFormacao)).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.Obterformatos(mockUseCase.Object, tipoFormacao);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoSolicitacaoValida_QuandoObterSituacoes_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterSituacoesProposta>();
            var lista = new List<RetornoListagemDTO>();
            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterSituacoes(mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoSolicitacaoValida_QuandoObterTipoEncontro_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterTipoEncontro>();
            var lista = new List<RetornoListagemDTO>();
            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterTipoEncontro(mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoSolicitacaoValida_QuandoObterFormacaoHomologada_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterFormacaoHomologada>();
            var lista = new List<RetornoListagemDTO>();
            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterFormacaoHomologada(mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoObterTurmas_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterTurmasProposta>();
            var id = _faker.Random.Long();
            var lista = new List<RetornoListagemDTO>();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterTurmas(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoObterPropostaPorId_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPropostaPorId>();
            var id = _faker.Random.Long();
            var dto = new PropostaCompletoDTO();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(dto);

            // Act
            var resultado = await _controller.ObterPropostaPorId(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoFiltros_QuandoObterPropostaPaginada_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPropostaPaginacao>();
            var filtro = new PropostaFiltrosDTO();
            var paginacao = new PaginacaoResultadoDto<PropostaPaginadaDTO>(new List<PropostaPaginadaDTO>(), 10, 10);
            mockUseCase.Setup(x => x.Executar(filtro)).ReturnsAsync(paginacao);

            // Act
            var resultado = await _controller.ObterPropostaPaginada(mockUseCase.Object, filtro);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaDto_QuandoInserirProposta_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoInserirProposta>();
            var dto = new PropostaDTO();
            var retorno = RetornoDTO.RetornarSucesso("Sucesso");
            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.InserirProposta(mockUseCase.Object, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaDtoEId_QuandoAlterarProposta_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoAlterarProposta>();
            var id = _faker.Random.Long();
            var dto = new PropostaDTO();
            var retorno = RetornoDTO.RetornarSucesso("Sucesso");
            mockUseCase.Setup(x => x.Executar(id, dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.AlterarProposta(mockUseCase.Object, id, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoDevolverDtoEId_QuandoDevolverProposta_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoDevolverProposta>();
            var id = _faker.Random.Long();
            var dto = new DevolverPropostaDTO { Justificativa = "Teste" };
            mockUseCase.Setup(x => x.Executar(id, dto)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.DevolverProposta(mockUseCase.Object, id, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoId_QuandoRemoverProposta_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoRemoverProposta>();
            var id = _faker.Random.Long();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.RemoverProposta(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoObterPropostaEncontrosPaginado_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPropostaEncontroPaginacao>();
            var id = _faker.Random.Long();
            var paginacao = new PaginacaoResultadoDto<PropostaEncontroDto>(new List<PropostaEncontroDto>(), 0, 10);
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(paginacao);

            // Act
            var resultado = await _controller.ObterPropostaEncontrosPaginado(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoEncontroDto_QuandoSalvarPropostaEncontro_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoSalvarPropostaEncontro>();
            var propostaId = _faker.Random.Long();
            var dto = new PropostaEncontroDto();
            mockUseCase.Setup(x => x.Executar(propostaId, dto)).ReturnsAsync(1);

            // Act
            var resultado = await _controller.SalvarPropostaEncontro(mockUseCase.Object, propostaId, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoIdEncontro_QuandoRemoverPropostaEncontro_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoRemoverPropostaEncontro>();
            var id = _faker.Random.Long();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.RemoverPropostaEncontro(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoObterComunicadoAcaoFormativaPorParametro_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterComunicadoAcaoFormativa>();
            var id = _faker.Random.Long();
            var dto = new ComunicadoAcaoFormativaDTO();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(dto);

            // Act
            var resultado = await _controller.ObterComunicadoAcaoFormativaPorParametro(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoRegistroFuncional_QuandoObterNomeProfissionalTutorRegente_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterNomeRegenteTutor>();
            var rf = "1234567";
            mockUseCase.Setup(x => x.Executar(rf)).ReturnsAsync("Nome Profissional");

            // Act
            var resultado = await _controller.ObterNomeProfissionalTutorRegente(rf, mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoRegenteDto_QuandoSalvarPropostaProfissionalRegente_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoSalvarPropostaRegente>();
            var propostaId = _faker.Random.Long();
            var dto = new PropostaRegenteDTO();
            mockUseCase.Setup(x => x.Executar(propostaId, dto)).ReturnsAsync(1);

            // Act
            var resultado = await _controller.SalvarPropostaProfissionalRegente(mockUseCase.Object, propostaId, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoObterPropostaRegentePaginado_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPropostaRegentePaginacao>();
            var id = _faker.Random.Long();
            var paginacao = new PaginacaoResultadoDto<PropostaRegenteDTO>(new List<PropostaRegenteDTO>(), 0, 10);
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(paginacao);

            // Act
            var resultado = await _controller.ObterPropostaRegentePaginado(id, mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoRegenteId_QuandoObterPropostaRegentePorId_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPropostaRegentePorId>();
            var id = _faker.Random.Long();
            var dto = new PropostaRegenteDTO();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(dto);

            // Act
            var resultado = await _controller.ObterPropostaRegentePorId(id, mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoRegenteId_QuandoExcluirRegente_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoRemoverPropostaRegente>();
            var id = _faker.Random.Long();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.ExcluirRegente(id, mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoTutorDto_QuandoSalvarPropostaProfissionalTutor_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoSalvarPropostaTutor>();
            var propostaId = _faker.Random.Long();
            var dto = new PropostaTutorDTO();
            mockUseCase.Setup(x => x.Executar(propostaId, dto)).ReturnsAsync(1);

            // Act
            var resultado = await _controller.SalvarPropostaProfissionalTutor(mockUseCase.Object, propostaId, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoTutorId_QuandoExcluirTutor_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoRemoverPropostaTutor>();
            var id = _faker.Random.Long();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.ExcluirTutor(id, mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoObterPropostaTutorPaginado_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPropostaTutorPaginacao>();
            var id = _faker.Random.Long();
            var paginacao = new PaginacaoResultadoDto<PropostaTutorDTO>(new List<PropostaTutorDTO>(), 0, 10);
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(paginacao);

            // Act
            var resultado = await _controller.ObterPropostaTutorPaginado(id, mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoTutorId_QuandoObterPropostaTutorPorId_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPropostaTutorPorId>();
            var id = _faker.Random.Long();
            var dto = new PropostaTutorDTO();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(dto);

            // Act
            var resultado = await _controller.ObterPropostaTutorPorId(id, mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoEnviarProposta_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoEnviarProposta>();
            var id = _faker.Random.Long();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.EnviarProposta(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoFiltrosDashboard_QuandoObterPropostasDashboard_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPropostasDashboard>();
            var filtro = new PropostaFiltrosDashboardDTO();
            var lista = new List<PropostaDashboardDTO>();
            mockUseCase.Setup(x => x.Executar(filtro)).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterPropostasDashboard(filtro, mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoParecerId_QuandoRemoverParecer_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoRemoverParecerDaProposta>();
            var id = _faker.Random.Long();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.RemoverParecer(id, mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoParecerDto_QuandoInserirPropostaParecer_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoSalvarPropostaPareceristaConsideracao>();
            var dto = new PropostaPareceristaConsideracaoCadastroDTO { Descricao = "Teste" };
            var retorno = RetornoDTO.RetornarSucesso("Sucesso");
            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.InserirPropostaParecer(mockUseCase.Object, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoParecerDto_QuandoAlterarPropostaParecer_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoSalvarPropostaPareceristaConsideracao>();
            var dto = new PropostaPareceristaConsideracaoCadastroDTO { Descricao = "Teste" };
            var retorno = RetornoDTO.RetornarSucesso("Sucesso");
            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.AlterarPropostaParecer(mockUseCase.Object, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoFiltrosParecer_QuandoObterPropostaPareceresPorPropostaIdECampo_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterPropostaParecer>();
            var filtro = new PropostaParecerFiltroDTO();
            var dto = new PropostaPareceristaConsideracaoCompletoDTO();
            mockUseCase.Setup(x => x.Executar(filtro)).ReturnsAsync(dto);

            // Act
            var resultado = await _controller.ObterPropostaPareceresPorPropostaIdECampo(mockUseCase.Object, filtro);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoObterSugestoesPareceristaAprovada_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterSugestaoParecerPareceristas>();
            var id = _faker.Random.Long();
            var lista = new List<PropostaPareceristaSugestaoDTO>();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterSugestoesPareceristaAprovada(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoEnviarPropostaParecerista_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoEnviarPropostaParecerista>();
            var id = _faker.Random.Long();
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.EnviarPropostaParecerista(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoJustificativa_QuandoAprovarPropostaParecerista_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoAprovarPropostaParecerista>();
            var id = _faker.Random.Long();
            var dto = new PropostaJustificativaDTO();
            mockUseCase.Setup(x => x.Executar(id, dto)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.AprovarPropostaParecerista(mockUseCase.Object, id, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoJustificativa_QuandoRecusarPropostaParecerista_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoRecusarPropostaParecerista>();
            var id = _faker.Random.Long();
            var dto = new PropostaJustificativaDTO();
            mockUseCase.Setup(x => x.Executar(id, dto)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.RecusarPropostaParecerista(mockUseCase.Object, id, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoJustificativa_QuandoAprovarProposta_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoAprovarProposta>();
            var id = _faker.Random.Long();
            var dto = new PropostaJustificativaDTO();
            mockUseCase.Setup(x => x.Executar(id, dto)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.AprovarProposta(mockUseCase.Object, id, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoJustificativa_QuandoRecusarProposta_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoRecusarProposta>();
            var id = _faker.Random.Long();
            var dto = new PropostaJustificativaDTO();
            mockUseCase.Setup(x => x.Executar(id, dto)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.RecusarProposta(mockUseCase.Object, id, dto);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoObterRelatorioLaudaDePublicacao_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterRelatorioPropostaLaudaPublicacao>();
            var id = _faker.Random.Long();
            var relatorio = "base64string";
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(relatorio);

            // Act
            var resultado = await _controller.ObterRelatorioLaudaDePublicacao(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoPropostaId_QuandoObterRelatorioLaudaCompleta_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterRelatorioPropostaLaudaCompleta>();
            var id = _faker.Random.Long();
            var relatorio = "base64string";
            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(relatorio);

            // Act
            var resultado = await _controller.ObterRelatorioLaudaCompleta(mockUseCase.Object, id);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoSolicitacaoValida_QuandoObterHorasTotaisProposta_EntaoDeveRetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterHorasTotaisProposta>();
            var lista = new List<RetornoListagemDTO>();
            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(lista);

            // Act
            var resultado = await _controller.ObterHorasTotaisProposta(mockUseCase.Object);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoUmTermoDeBuscaQualquer_QuandoChamarAutocompletarFormacao_DeveRetornarResultadoDeSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterAutocompletarFormacao>();
            var termoDeBusca = _faker.Lorem.Word();
            var resultadoDto = new PaginacaoResultadoDto<AutocompletarNumeroHomologacaoDto>(new List<AutocompletarNumeroHomologacaoDto>(), 0, 10);
            mockUseCase.Setup(x => x.ExecutarAsync(It.IsAny<FiltroAutocompletarNumeroHomologacaoDto>())).ReturnsAsync(resultadoDto);
            // Act
            var resultado = await _controller.AutocompletarFormacao(mockUseCase.Object, new() { NumeroPagina = 1, NumeroRegistros = 10, TermoBusca = termoDeBusca });
            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }
    }
}