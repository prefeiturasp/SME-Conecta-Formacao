using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
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
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }


        [Fact(DisplayName = "Proposta - Deve Enviar para o DF uma Proposta com Situação Cadastrada")]
        public async Task Enviar_para_df_proposta_cadastrada()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var casoUsoEnviarDf = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParaValidacao>();

            // act
            await casoUsoEnviarDf.Executar(proposta.Id);

            // assert
            var obterPropostaDepois = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            obterPropostaDepois.Situacao.ShouldBeEquivalentTo(SituacaoProposta.AguardandoAnaliseDf);
        }

        [Fact(DisplayName = "Proposta - Não Deve Enviar para o DF uma Proposta com Situação Diferente de Cadastrada")]
        public async Task Nao_deve_enviar_para_df_proposta_diferente_de_cadastrada()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(SituacaoProposta.Rascunho);

            var casoUsoEnviarDf = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParaValidacao>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoUsoEnviarDf.Executar(proposta.Id));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_CADASTRADA).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Não Deve Enviar para o DF uma Proposta Excluida")]
        public async Task Nao_deve_enviar_uma_proposta_excluida()
        {
            // arrange
            var casoUsoEnviarDf = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParaValidacao>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoUsoEnviarDf.Executar(99));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA).ShouldBeTrue();
        }
    }
}