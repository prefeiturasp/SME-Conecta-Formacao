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
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }


        [Fact(DisplayName = "Proposta - Deve Enviar para o DF uma Proposta com Situação Cadastrada")]
        public async Task Enviar_para_df_proposta_cadastrada()
        {
            var parametroSistemaDescricao = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaDescricao);
            await InserirNaBase(parametroSistemaDescricao);
            var parametroSistemaUrl = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaUrl);
            await InserirNaBase(parametroSistemaUrl);

            var id = await CriarPropostaNaBase(SituacaoProposta.Cadastrada);
            var proposta = ObterPorId<Dominio.Entidades.Proposta,long>(id);
            var propostaRegente = PropostaMock.GerarRegente(id);
            await InserirNaBase(propostaRegente);
            
            var propostaRegenteTurma = PropostaMock.GerarRegenteTurmas(propostaRegente.First().Id,(short)proposta.QuantidadeTurmas);
            await InserirNaBase(propostaRegenteTurma);

            var encontrosProposta = PropostaMock.GerarEncontros(id);
            await InserirNaBase(encontrosProposta);

            var encontrosPropostaTurma = PropostaMock.GerarPropostaEncontroTurmas(encontrosProposta.First().Id,(short)proposta.QuantidadeTurmas);
            await InserirNaBase(encontrosPropostaTurma);

            var obterProposaAntes = ObterPorId<Dominio.Entidades.Proposta, long>(id);
            obterProposaAntes.Situacao.ShouldBeEquivalentTo(SituacaoProposta.Cadastrada);

            var casoUsoEnviarDf = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParaValidacao>();
            await casoUsoEnviarDf.Executar(id);

            var obterPropostaDepois = ObterPorId<Dominio.Entidades.Proposta, long>(id);
            obterPropostaDepois.Situacao.ShouldBeEquivalentTo(SituacaoProposta.AguardandoAnaliseDf);
        }
        
        [Fact(DisplayName = "Proposta - Não Deve Enviar para o DF uma Proposta com Situação Cadastrada sem Encontros e sem Tutor")]
        public async Task Nao_dev_enviar_para_df_proposta_cadastrada_sem_encontros_e_tutor()
        {
            var parametroSistemaDescricao = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaDescricao);
            await InserirNaBase(parametroSistemaDescricao);
            var parametroSistemaUrl = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaUrl);
            await InserirNaBase(parametroSistemaUrl);

            var id = await CriarPropostaNaBase(SituacaoProposta.Cadastrada);


            var obterProposaAntes = ObterPorId<Dominio.Entidades.Proposta, long>(id);
            obterProposaAntes.Situacao.ShouldBeEquivalentTo(SituacaoProposta.Cadastrada);

            var casoUsoEnviarDf = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParaValidacao>();
            var excecao = await Should.ThrowAsync<NegocioException>(casoUsoEnviarDf.Executar(id));
            excecao.ShouldNotBeNull();
            excecao.Mensagens?.Count()!.ShouldBeEquivalentTo(2);
            excecao.Mensagens!.Contains(MensagemNegocio.QUANTIDADE_TURMAS_COM_ENCONTRO_DIFERENTE_QUANTIDADE_DE_TURMAS).ShouldBeTrue();
            excecao.Mensagens!.Contains(MensagemNegocio.QUANTIDADE_TURMAS_COM_REGENTE).ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Não Deve Enviar para o DF uma Proposta com Situação Diferente de Cadastrada")]
        public async Task Nao_deve_enviar_para_df_proposta_diferente_de_cadastrada()
        {
            var parametroSistemaDescricao = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaDescricao);
            await InserirNaBase(parametroSistemaDescricao);
            var parametroSistemaUrl = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaUrl);
            await InserirNaBase(parametroSistemaUrl);

            var id = await CriarPropostaNaBase(SituacaoProposta.Rascunho);

            var casoUsoEnviarDf = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParaValidacao>();
            var excecao = await Should.ThrowAsync<NegocioException>(casoUsoEnviarDf.Executar(id));
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_CADASTRADA_OU_DEVOLVIDA).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Não Deve Enviar para o DF uma Proposta Excluida")]
        public async Task Nao_deve_enviar_uma_proposta_excluida()
        {
            var parametroSistemaDescricao = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaDescricao);
            await InserirNaBase(parametroSistemaDescricao);
            var parametroSistemaUrl = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaUrl);
            await InserirNaBase(parametroSistemaUrl);

            var id = await CriarPropostaNaBase(SituacaoProposta.Cadastrada);

            var useCaseDeleteProposta = ObterCasoDeUso<ICasoDeUsoRemoverProposta>();
            await useCaseDeleteProposta.Executar(id);

            var casoUsoEnviarDf = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParaValidacao>();
            var excecao = await Should.ThrowAsync<NegocioException>(casoUsoEnviarDf.Executar(id));
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA).ShouldBeTrue();
        }
    }
}