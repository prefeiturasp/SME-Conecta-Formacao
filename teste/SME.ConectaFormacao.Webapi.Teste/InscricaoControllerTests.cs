using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class InscricaoControllerTests
    {
        private readonly InscricaoController _controller;
        private readonly Faker _faker;

        public InscricaoControllerTests()
        {
            _controller = new InscricaoController();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task Dado_SolicitacaoValida_Quando_ObterDadosUsuario_Entao_RetornarSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterDadosInscricao>();
            var resultadoEsperado = new DadosInscricaoDto
            {
                UsuarioNome = _faker.Person.FullName,
                UsuarioRf = _faker.Random.AlphaNumeric(7)
            };

            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.ObterDadosUsuario(mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(resultadoEsperado, okResult.Value);
        }

        [Fact]
        public async Task Dado_PropostaIdValido_Quando_ObterDadosInscricaoProposta_Entao_RetornarSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterDadosInscricaoParaProposta>();
            var propostaId = _faker.Random.Long(1, 100);
            var resultadoEsperado = new DadosInscricaoPropostaDto
            {
                UsuarioNome = _faker.Person.FullName,
                UsuarioRf = _faker.Random.AlphaNumeric(7),
                VagaRemanescente = false
            };

            mockUseCase.Setup(x => x.ExecutarAsync(propostaId)).ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.ObterDadosInscricaoProposta(propostaId, mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(resultadoEsperado, okResult.Value);
        }

        [Fact]
        public async Task Dado_FiltrosValidos_Quando_ObterTurmas_Entao_RetornarListaDeTurmas()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterTurmasInscricao>();
            var propostaId = _faker.Random.Long(1, 100);
            var codigoDre = _faker.Random.AlphaNumeric(5);
            var resultadoEsperado = new List<RetornoListagemDTO>();

            mockUseCase.Setup(x => x.Executar(propostaId, codigoDre)).ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.ObterTurmas(mockUseCase.Object, propostaId, codigoDre);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Same(resultadoEsperado, okResult.Value);
        }

        [Fact]
        public async Task Dado_SolicitacaoValida_Quando_ObterInscricoesPaginada_Entao_RetornarPaginacao()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterInscricaoPaginada>();
            var resultadoEsperado = new PaginacaoResultadoDto<InscricaoPaginadaDTO>([], 0, 10);

            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.ObterInscricoesPaginada(mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(resultadoEsperado, okResult.Value);
        }

        [Fact]
        public async Task Dado_SolicitacaoValida_Quando_ObterInscricoesProximasPaginada_Entao_RetornarPaginacao()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterInscricaoProximaPaginada>();
            var resultadoEsperado = new PaginacaoResultadoDto<InscricaoPaginadaDTO>([], 0, 10);

            var dto = new Faker<InscricaoProximaFiltroDTO>()
                .RuleFor(x => x.CodigoFormacao, f => f.Random.Long(1, 100))
                .Generate();

            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.ObterInscricoesProximasPaginada(mockUseCase.Object, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(resultadoEsperado, okResult.Value);
        }

        [Fact]
        public async Task Dado_SolicitacaoValida_Quando_ObterInscricoesFinalizadasPaginada_Entao_RetornarPaginacao()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterInscricaoFinalizadaPaginada>();
            var resultadoEsperado = new PaginacaoResultadoDto<InscricaoPaginadaDTO>([], 0, 10);

            var dto = new Faker<InscricaoFinalizadaFiltroDTO>()
                .RuleFor(x => x.NomeFormacao, f => f.Random.AlphaNumeric(7))
                .Generate();

            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.ObterInscricoesFinalizadasPaginada(mockUseCase.Object, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(resultadoEsperado, okResult.Value);
        }

        [Fact]
        public async Task Dado_InscricaoValida_Quando_SalvarInscricao_Entao_RetornarSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoSalvarInscricao>();
            var dto = new Faker<InscricaoDto>()
                .RuleFor(x => x.PropostaTurmaId, f => f.Random.Long(1, 100))
                .RuleFor(x => x.VagaRemanescente, f => f.Random.Bool())
                .Generate();

            var retorno = RetornoDTO.RetornarSucesso("Sucesso");

            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.SalvarInscricao(mockUseCase.Object, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_InscricaoManualValida_Quando_SalvarInscricaoManual_Entao_RetornarSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoSalvarInscricaoManual>();
            var dto = new Faker<InscricaoManualDTO>()
                .RuleFor(x => x.Cpf, f => f.Person.Cpf())
                .RuleFor(x => x.PropostaTurmaId, f => f.Random.Long())
                .Generate();

            var retorno = RetornoDTO.RetornarSucesso("Manual Sucesso");

            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.SalvarInscricaoManual(mockUseCase.Object, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_IdValido_Quando_CancelarInscricao_Entao_RetornarTrue()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoCancelarInscricao>();
            var id = _faker.Random.Long(1, 100);

            mockUseCase.Setup(x => x.Executar(id)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.CancelarInscricao(mockUseCase.Object, id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.True((bool)okResult.Value!);
        }

        [Fact]
        public async Task Dado_IdsValidos_Quando_ConfirmarInscricoes_Entao_RetornarSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoConfirmarInscricoes>();
            var ids = new long[] { 1, 2, 3 };
            var retorno = RetornoDTO.RetornarSucesso("Confirmado");

            mockUseCase.Setup(x => x.Executar(ids)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.ConfirmarInscricoes(mockUseCase.Object, ids);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_IdsValidos_Quando_EmEsperaInscricoes_Entao_RetornarSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoEmEsperaInscricoes>();
            var ids = new long[] { 1, 2, 3 };
            var retorno = RetornoDTO.RetornarSucesso("Em Espera");

            mockUseCase.Setup(x => x.Executar(ids)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.EmEsperaInscricoes(mockUseCase.Object, ids);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_IdsEMotivoValidos_Quando_CancelarInscricoes_Entao_RetornarSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoCancelarInscricoes>();
            var ids = new long[] { 1, 2 };
            var dto = new InscricaoMotivoCancelamentoDTO { Motivo = "Teste" };
            var retorno = RetornoDTO.RetornarSucesso("Cancelado em lote");

            mockUseCase.Setup(x => x.Executar(ids, dto.Motivo)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.CancelarInscricoes(mockUseCase.Object, ids, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_TransferenciaValida_Quando_TransferirInscricoes_Entao_RetornarOk()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoTransferirInscricao>();
            var dto = new InscricaoTransferenciaDTO();
            var retorno = new RetornoInscricaoDTO { Status = (int)HttpStatusCode.OK, Mensagem = "Sucesso" };

            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.TransferirInscricoes(mockUseCase.Object, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_TransferenciaInvalida_Quando_TransferirInscricoes_Entao_RetornarBadRequest()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoTransferirInscricao>();
            var dto = new InscricaoTransferenciaDTO();
            var retorno = new RetornoInscricaoDTO { Status = (int)HttpStatusCode.BadRequest, Mensagem = "Erro de validação" };

            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.TransferirInscricoes(mockUseCase.Object, dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal(retorno, badRequestResult.Value);
        }

        [Fact]
        public async Task Dado_ErroInterno_Quando_TransferirInscricoes_Entao_RetornarServerError()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoTransferirInscricao>();
            var dto = new InscricaoTransferenciaDTO();
            var retorno = new RetornoInscricaoDTO { Status = (int)HttpStatusCode.InternalServerError, Mensagem = "Erro interno" };

            mockUseCase.Setup(x => x.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.TransferirInscricoes(mockUseCase.Object, dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal(retorno, objectResult.Value);
        }

        [Fact]
        public async Task Dado_Filtros_Quando_ObterInscricaoPorIdPaginado_Entao_DeveAtribuirPropostaIdERetornarDados()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterInscricaoPorId>();
            var propostaId = _faker.Random.Long(1, 100);
            var filtro = new Faker<FiltroListagemInscricaoDto>()
                .CustomInstantiator(f => new FiltroListagemInscricaoDto
                {
                    PropostaId = 0, // Será sobrescrito
                    NumeroPagina = 1,
                    NumeroRegistros = 10
                })
                .Generate();

            var retorno = new PaginacaoResultadoDto<DadosListagemInscricaoDto>([], 0, 10);

            mockUseCase.Setup(x => x.ExecutarAsync(It.Is<FiltroListagemInscricaoDto>(f => f.PropostaId == propostaId)))
                       .ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.ObterInscricaoPorIdPaginado(propostaId, filtro, mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
            Assert.Equal(propostaId, filtro.PropostaId); // Verifica se o ID foi atribuído no controller
        }

        [Fact]
        public async Task Dado_Filtros_Quando_ObterFormacaoComTurmaPorFiltros_Entao_RetornarListaPaginada()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterDadosPaginadosComFiltros>();
            var filtro = new FiltroListagemInscricaoComTurmaDTO();
            var retorno = new PaginacaoResultadoDto<DadosListagemFormacaoComTurmaDTO>([], 0, 10);

            mockUseCase.Setup(x => x.Executar(filtro)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.ObterFormacaoComTurmaPorFiltros(filtro, mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_PropostaTurmaId_Quando_SortearInscricoes_Entao_RetornarSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoSortearInscricoes>();
            var propostaTurmaId = _faker.Random.Long();
            var retorno = RetornoDTO.RetornarSucesso("Sorteio realizado");

            mockUseCase.Setup(x => x.Executar(propostaTurmaId)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.SortearInscricoes(mockUseCase.Object, propostaTurmaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_Solicitacao_Quando_ObterInscricaoTipo_Entao_RetornarTipos()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterInscricaoTipo>();
            var retorno = new List<RetornoListagemDTO>();

            mockUseCase.Setup(x => x.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.ObterInscricaoTipo(mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Same(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_VinculoValido_Quando_AlterarVinculo_Entao_RetornarTrue()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoAlterarVinculoInscricao>();
            var id = _faker.Random.Long();
            var dto = new AlterarCargoFuncaoVinculoIncricaoDTO { CargoCodigo = "001" };

            mockUseCase.Setup(x => x.Executar(id, dto)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.AlterarVinculo(mockUseCase.Object, id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.True((bool)okResult.Value!);
        }

        [Fact]
        public async Task Dado_PropostaId_Quando_InscricoesEstaoAbertas_Entao_RetornarStatus()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoObterInformacoesInscricoesEstaoAbertasPorId>();
            var propostaId = _faker.Random.Long();
            var retorno = new PodeInscreverMensagemDTO { PodeInscrever = true };

            mockUseCase.Setup(x => x.Executar(propostaId)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.InscricoesEstaoAbertas(propostaId, mockUseCase.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }

        [Fact]
        public async Task Dado_IdsValidos_Quando_ReativarInscricoes_Entao_RetornarSucesso()
        {
            // Arrange
            var mockUseCase = new Mock<ICasoDeUsoReativarInscricoes>();
            var ids = new long[] { 10, 20 };
            var retorno = RetornoDTO.RetornarSucesso("Reativado");

            mockUseCase.Setup(x => x.Executar(ids)).ReturnsAsync(retorno);

            // Act
            var resultado = await _controller.ReativarInscricoes(mockUseCase.Object, ids);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(retorno, okResult.Value);
        }
    }
}