using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_enviar_proposta : TestePropostaBase
    {
        public Ao_enviar_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarFakes(IServiceCollection services)
        {
            base.RegistrarFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }


        [Fact(DisplayName = "Proposta - Deve Enviar para o DF uma Proposta com Situação Cadastrada homologada")]
        public async Task Enviar_para_df_proposta_cadastrada_homologada()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(dataInscricaoForaPeriodo: true);

            var casoUsoEnviarProposta = ObterCasoDeUso<ICasoDeUsoEnviarProposta>();

            // act
            await casoUsoEnviarProposta.Executar(proposta.Id);

            // assert
            var obterPropostaDepois = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            obterPropostaDepois.Situacao.ShouldBeEquivalentTo(SituacaoProposta.AguardandoAnaliseDf);
        }

        [Fact(DisplayName = "Proposta - Deve Enviar para o Publicada uma Proposta com Situação Cadastrada não homologada")]
        public async Task Enviar_para_publicada_proposta_cadastrada_nao_homologada()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(formacaoHomologada: FormacaoHomologada.NaoCursosPorIN, dataInscricaoForaPeriodo: true);

            var casoUsoEnviarProposta = ObterCasoDeUso<ICasoDeUsoEnviarProposta>();

            // act
            await casoUsoEnviarProposta.Executar(proposta.Id);

            // assert
            var obterPropostaDepois = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            obterPropostaDepois.Situacao.ShouldBeEquivalentTo(SituacaoProposta.Publicada);
        }

        [Fact(DisplayName = "Proposta - Não Deve Enviar uma Proposta com Situação Diferente de Cadastrada")]
        public async Task Nao_deve_enviar_proposta_diferente_de_cadastrada()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(SituacaoProposta.Rascunho);

            var casoUsoEnviarProposta = ObterCasoDeUso<ICasoDeUsoEnviarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoUsoEnviarProposta.Executar(proposta.Id));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_CADASTRADA_NEM_DEVOLVIDA).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Não Deve Enviar uma Proposta não encontrada")]
        public async Task Nao_deve_enviar_uma_proposta_nao_encontrada()
        {
            // arrange
            var casoUsoEnviarDf = ObterCasoDeUso<ICasoDeUsoEnviarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoUsoEnviarDf.Executar(99));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve alterar proposta aguardando df homologada em aguardando parecerista")]
        public async Task Deve_alterar_proposta_aguardando_df_homologada_em_aguardando_parecerista()
        {
            //arrange
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseDf, formacaoHomologada: FormacaoHomologada.Sim, dataInscricaoForaPeriodo: true, quantidadeParecerista: 2);

            var casoUsoEnviarProposta = ObterCasoDeUso<ICasoDeUsoEnviarProposta>();

            // act
            await casoUsoEnviarProposta.Executar(proposta.Id);

            // assert
            var obterPropostaDepois = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            obterPropostaDepois.Situacao.ShouldBeEquivalentTo(SituacaoProposta.AguardandoAnaliseParecerista);
        }
    }
}