using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_enviar_proposta_para_df : TestePropostaBase
    {
        public Ao_enviar_proposta_para_df(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarFakes(IServiceCollection services)
        {
            base.RegistrarFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterParametroSistemaPorTipoEAnoQuery, Dominio.Entidades.ParametroSistema>), typeof(ObterParametroSistemaPorTipoEAnoQueryFaker), ServiceLifetime.Scoped));
        }

        
        [Fact(DisplayName = "Proposta - Deve Enviar para o DF uma Proposta com Situação Cadastrada")]
        public async Task Enviar_para_df_proposta_cadastrada()
        {
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOVazio(SituacaoProposta.Rascunho);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            var id = await casoDeUso.Executar(propostaDTO);

            var casoUsoEnviarDf = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParaDf>();
            await casoUsoEnviarDf.Executar(id);
        }
    }
}