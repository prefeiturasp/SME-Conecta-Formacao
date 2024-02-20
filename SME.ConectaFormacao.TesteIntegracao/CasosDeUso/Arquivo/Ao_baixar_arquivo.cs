using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo
{
    public class Ao_baixar_arquivo : TesteBase
    {
        public Ao_baixar_arquivo(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterEnderecoArquivoServicoArmazenamentoQuery, string>), typeof(ObterEnderecoArquivoServicoArmazenamentoQueryHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Arquivo - Deve retornar excecao quando o arquivo nao for encontrado")]
        public async Task Deve_retornar_excecao_arquivo_nao_encontrado()
        {
            // arrange
            var codigo = Guid.NewGuid();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoArquivoBaixar>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(codigo));

            // assert
            retorno.ShouldNotBeNull();
            retorno.Mensagens.Contains(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO).ShouldBeTrue();
        }

        [Fact(DisplayName = "Arquivo - Deve baixar arquivo valido")]
        public async Task Deve_baixar_arquivo_valido()
        {
            // arrange
            var arquivo = ArquivoMock.GerarArquivo();
            await InserirNaBase(arquivo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoArquivoBaixar>();

            // act
            var retorno = await casoDeUso.Executar(arquivo.Codigo);

            // assert
            retorno.Nome.ShouldBe(arquivo.Nome);
        }
    }
}
