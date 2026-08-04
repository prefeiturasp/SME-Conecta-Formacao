using Moq;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterNomesFuncionarioPorRf;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterNomesFuncionarioPorRfQueryHandlerTestes
    {
        private readonly Mock<IServicoEol> servicoEol;
        private readonly Mock<ICacheDistribuido> cacheDistribuido;
        private readonly ObterNomesFuncionarioPorRfQueryHandler handler;

        public ObterNomesFuncionarioPorRfQueryHandlerTestes()
        {
            servicoEol = new Mock<IServicoEol>();
            cacheDistribuido = new Mock<ICacheDistribuido>();

            handler = new ObterNomesFuncionarioPorRfQueryHandler(
                servicoEol.Object,
                cacheDistribuido.Object);
        }

        [Fact]
        public async Task Handle_Deve_retornar_objeto_fornecido_pelo_cache()
        {
            var request = new ObterNomesFuncionarioPorRfQuery("1234567");
            var esperado = CriarFuncionarioNomesDto();

            cacheDistribuido
                .Setup(x => x.ObterAsync(
                    CacheDistribuidoNomes.NomesUsuario.Parametros(request.Rf),
                    It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(esperado);

            var resultado = await handler.Handle(request, CancellationToken.None);

            Assert.Same(esperado, resultado);

            cacheDistribuido.Verify(x => x.ObterAsync(
                CacheDistribuidoNomes.NomesUsuario.Parametros(request.Rf),
                It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);

            servicoEol.Verify(
                x => x.ObterNomesFuncionarioPorRegistroFuncional(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Quando_cache_executar_fallback_Deve_consultar_servico_eol()
        {
            var request = new ObterNomesFuncionarioPorRfQuery("1234567");
            var esperado = CriarFuncionarioNomesDto();

            servicoEol
                .Setup(x => x.ObterNomesFuncionarioPorRegistroFuncional(request.Rf))
                .ReturnsAsync(esperado);

            cacheDistribuido
                .Setup(x => x.ObterAsync(
                    CacheDistribuidoNomes.NomesUsuario.Parametros(request.Rf),
                    It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns<string, Func<Task<FuncionarioNomesDto?>>, int, bool>(
                    async (_, obterDados, __, ___) => await obterDados());

            var resultado = await handler.Handle(request, CancellationToken.None);

            Assert.Same(esperado, resultado);

            servicoEol.Verify(
                x => x.ObterNomesFuncionarioPorRegistroFuncional(request.Rf),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Quando_servico_eol_retornar_nulo_Deve_retornar_nulo()
        {
            var request = new ObterNomesFuncionarioPorRfQuery("1234567");

            servicoEol
                .Setup(x => x.ObterNomesFuncionarioPorRegistroFuncional(request.Rf))
                .ReturnsAsync((FuncionarioNomesDto?)null);

            cacheDistribuido
                .Setup(x => x.ObterAsync(
                    CacheDistribuidoNomes.NomesUsuario.Parametros(request.Rf),
                    It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns<string, Func<Task<FuncionarioNomesDto?>>, int, bool>(
                    async (_, obterDados, __, ___) => await obterDados());

            var resultado = await handler.Handle(request, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Handle_Deve_montar_chave_de_cache_com_rf_da_requisicao()
        {
            const string rf = "7654321";
            var request = new ObterNomesFuncionarioPorRfQuery(rf);
            var chaveEsperada = CacheDistribuidoNomes.NomesUsuario.Parametros(rf);

            cacheDistribuido
                .Setup(x => x.ObterAsync(
                    chaveEsperada,
                    It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync((FuncionarioNomesDto?)null);

            await handler.Handle(request, CancellationToken.None);

            cacheDistribuido.Verify(x => x.ObterAsync(
                chaveEsperada,
                It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Quando_cache_lancar_excecao_Deve_propagar_excecao()
        {
            var request = new ObterNomesFuncionarioPorRfQuery("1234567");
            var excecaoEsperada = new InvalidOperationException("Falha no cache");

            cacheDistribuido
                .Setup(x => x.ObterAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ThrowsAsync(excecaoEsperada);

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(request, CancellationToken.None));

            Assert.Same(excecaoEsperada, excecao);

            servicoEol.Verify(
                x => x.ObterNomesFuncionarioPorRegistroFuncional(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Quando_servico_eol_lancar_excecao_Deve_propagar_excecao()
        {
            var request = new ObterNomesFuncionarioPorRfQuery("1234567");
            var excecaoEsperada = new InvalidOperationException("Falha no EOL");

            servicoEol
                .Setup(x => x.ObterNomesFuncionarioPorRegistroFuncional(request.Rf))
                .ThrowsAsync(excecaoEsperada);

            cacheDistribuido
                .Setup(x => x.ObterAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns<string, Func<Task<FuncionarioNomesDto?>>, int, bool>(
                    async (_, obterDados, __, ___) => await obterDados());

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(request, CancellationToken.None));

            Assert.Same(excecaoEsperada, excecao);
        }

        [Fact]
        public async Task Handle_Com_token_cancelado_Deve_manter_comportamento_atual()
        {
            var request = new ObterNomesFuncionarioPorRfQuery("1234567");
            using var cancellationTokenSource = new CancellationTokenSource();
            await cancellationTokenSource.CancelAsync();

            cacheDistribuido
                .Setup(x => x.ObterAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync((FuncionarioNomesDto?)null);

            var resultado = await handler.Handle(
                request,
                cancellationTokenSource.Token);

            Assert.Null(resultado);

            cacheDistribuido.Verify(x => x.ObterAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<FuncionarioNomesDto?>>>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        private static FuncionarioNomesDto CriarFuncionarioNomesDto()
        {
            return (FuncionarioNomesDto)Activator.CreateInstance(
                typeof(FuncionarioNomesDto),
                nonPublic: true)!;
        }
    }
}