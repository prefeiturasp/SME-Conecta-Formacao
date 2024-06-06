using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_relatorio : TestePropostaBase
    {
        public Ao_obter_relatorio(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterRelatorioProspostaLaudaPublicacaoQuery, string>), typeof(ObterRelatorioProspostaLaudaPublicacaoHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterRelatorioProspostaLaudaCompletaQuery, string>), typeof(ObterRelatorioProspostaLaudaCompletoHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Deve obter relatório de lauda de publicação")]
        public async Task Deve_obter_relatório_lauda_publicacao()
        {
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterRelatorioPropostaLaudaPublicacao>();

            // act 
            var retorno = await casoDeUso.Executar(1);

            // assert
            retorno.ShouldNotBeNull();
            retorno.ShouldBe("url relatorio");
        }

        [Fact(DisplayName = "Proposta - Deve obter relatório de lauda de completo")]
        public async Task Deve_obter_relatório_lauda_completo()
        {
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterRelatorioPropostaLaudaCompleta>();

            // act 
            var retorno = await casoDeUso.Executar(1);

            // assert
            retorno.ShouldNotBeNull();
            retorno.ShouldBe("url relatorio lauda completo");
        }
    }
}
