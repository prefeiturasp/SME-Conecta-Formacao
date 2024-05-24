using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class ao_obter_nome_profissional : TesteBase
    {
        public ao_obter_nome_profissional(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterNomeCpfProfissionalPorRegistroFuncionalQuery, RetornoUsuarioDTO>), typeof(ObterNomeProfissionalPorRegistroFuncionalQueryHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Deve Obter Nome do Regente/Tutor com RF Válido")]
        public async Task Deve_obter_nome_regente_tutor_com_ff_valido()
        {
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNomeRegenteTutor>();
            var consulta = await casoDeUso.Executar("111111");
            consulta.ShouldNotBeNull();
            consulta.ShouldBeEquivalentTo("Nome do Profissional");
        }
        [Fact(DisplayName = "Proposta - Não Deve Obter Nome do Regente/Tutor RF Inválido")]
        public async Task Deve_retornar_excessao_rf_invalido()
        {
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNomeRegenteTutor>();

            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(""));

            excessao.ShouldNotBeNull();
            excessao.Mensagens.Contains(MensagemNegocio.PROFISSIONAL_NAO_LOCALIZADO_RF_INVALIDO).ShouldBeTrue();
        }
    }
}