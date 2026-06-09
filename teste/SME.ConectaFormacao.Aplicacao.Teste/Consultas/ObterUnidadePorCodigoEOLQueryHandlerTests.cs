using Bogus;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterUnidadePorCodigoEOLQueryHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ObterUnidadePorCodigoEOLQueryHandler _handler;
        private readonly Faker _faker;

        public ObterUnidadePorCodigoEOLQueryHandlerTestes()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<ObterUnidadePorCodigoEOLQueryHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoCodigoUnidadeExistente_QuandoExecutar_EntaoDeveRetornarUnidadeEol()
        {
            // Arrange
            var codigoUnidade = _faker.Random.AlphaNumeric(6);
            var query = new ObterUnidadePorCodigoEOLQuery(codigoUnidade);

            var unidadeEsperada = new UnidadeEol
            {
                Codigo = codigoUnidade,
                NomeUnidade = _faker.Company.CompanyName(),
                Sigla = "EMEF",
                Tipo = UnidadeEolTipo.Escola
            };

            // Mock do Cache simulando o retorno dos dados (seja do cache ou da execução da func interna)
            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.ObterAsync(
                    It.Is<string>(s => s.Contains(codigoUnidade)), // Verifica se a chave contém o código
                    It.IsAny<Func<Task<UnidadeEol>>>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(unidadeEsperada);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(unidadeEsperada.Codigo, resultado.Codigo);
            Assert.Equal(unidadeEsperada.NomeUnidade, resultado.NomeUnidade);

            _mocker.GetMock<ICacheDistribuido>().Verify(x => x.ObterAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<UnidadeEol>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task DadoUnidadeComNomeNuloOuVazio_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var codigoUnidade = _faker.Random.AlphaNumeric(6);
            var query = new ObterUnidadePorCodigoEOLQuery(codigoUnidade);

            // Simula retorno de uma unidade "vazia" ou não encontrada corretamente pelo EOL
            var unidadeInvalida = new UnidadeEol
            {
                Codigo = codigoUnidade,
                NomeUnidade = null! // Força null para testar a extensão EhNulo()
            };

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.ObterAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<UnidadeEol>>>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(unidadeInvalida);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(query, CancellationToken.None));

            Assert.Equal(MensagemNegocio.UNIDADE_NAO_LOCALIZADA_POR_CODIGO, excecao.Message);
        }

        [Fact]
        public async Task DadoServicoEol_QuandoCacheNaoEncontra_EntaoDeveChamarFuncDeBusca()
        {
            // Arrange
            var codigoUnidade = "123456";
            var query = new ObterUnidadePorCodigoEOLQuery(codigoUnidade);
            var chaveEsperada = string.Format(CacheDistribuidoNomes.UnidadeEol, codigoUnidade);

            var unidadeRetorno = new UnidadeEol { NomeUnidade = "Escola Teste" };

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(x => x.ObterAsync(It.IsAny<string>(), It.IsAny<Func<Task<UnidadeEol>>>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(unidadeRetorno);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mocker.GetMock<ICacheDistribuido>().Verify(x => x.ObterAsync(
                It.Is<string>(k => k == chaveEsperada), // Valida se a chave foi gerada corretamente
                It.IsAny<Func<Task<UnidadeEol>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>()), Times.Once);
        }
    }
}
