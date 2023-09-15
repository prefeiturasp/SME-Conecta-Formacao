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
    public class Ao_excluir_arquivo : TesteBase
    {
        public Ao_excluir_arquivo(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<RemoverArquivoServicoArmazenamentoCommand, bool>), typeof(RemoverArquivoServicoArmazenamentoCommandHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Arquivo - Deve retornar excecao nenhum arquivo encontrado")]
        public async Task Deve_retornar_excecao_nenhum_arquivo_encontrado()
        {
            // arrange
            var codigos = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

            var casosDeUso = ObterCasoDeUso<ICasoDeUsoArquivoExcluir>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casosDeUso.Executar(codigos));

            // assert
            retorno.ShouldNotBeNull();
            retorno.Mensagens.Contains(MensagemNegocio.ARQUIVO_NENHUM_ARQUIVO_ENCONTRADO).ShouldBeTrue();
        }

        [Fact(DisplayName = "Arquivo - Deve remover arquivos validos")]
        public async Task Deve_remover_arquivos_valido()
        {
            // arrange
            var arquivos = ArquivoMock.GerarArquivo(10);
            await InserirNaBase(arquivos);

            var codigos = arquivos.Select(t => t.Codigo).ToArray();

            var casosDeUso = ObterCasoDeUso<ICasoDeUsoArquivoExcluir>();

            // act
            var retorno = await casosDeUso.Executar(codigos);

            // assert
            retorno.ShouldBeTrue();

            var arquivosBanco = ObterTodos<Dominio.Entidades.Arquivo>();

            arquivosBanco.Any(t => !t.Excluido).ShouldBeFalse();
        }
    }
}
