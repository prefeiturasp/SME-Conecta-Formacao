using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_enviar_parecer : TestePropostaBase
    {
        public Ao_enviar_parecer(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - O Parecerista deve enviar as suas considerações e a proposta deve ter a situação alterada para Aguardando Análise de Parecer pela DF")]
        public async Task O_parecerista_deve_enviar_as_suas_consideracoes_e_a_proposta_deve_ter_a_situacao_alterada_para_aguardando_analise_de_parecer_pela_df()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            CriarClaimUsuario(perfilLogado, "1", "Parecerista1");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            await InserirParametrosProposta();

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnalisePeloParecerista,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.AguardandoValidacao));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));

            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TiposInscricao, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.IntegrarNoSGA, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Dres, "1"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "2"));

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceristas = ObterTodos<PropostaParecerista>();
            pareceristas.All(a=> a.Situacao == SituacaoParecerista.Enviada).ShouldBeTrue();

            var propostas = ObterTodos<Dominio.Entidades.Proposta>();
            propostas.All(a=> a.Situacao.EstaAguardandoAnaliseParecerPelaDF()).ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - O Parecerista deve enviar as suas considerações e a proposta deve ter a situação alterada para Aguardando Análise pelo Parecerista")]
        public async Task O_parecerista_deve_enviar_as_suas_consideracoes_e_a_proposta_deve_ter_a_situacao_alterada_para_aguardando_analise_pelo_parecerista()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            CriarClaimUsuario(perfilLogado, "1", "Parecerista1");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            await InserirParametrosProposta();

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnalisePeloParecerista,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.AguardandoValidacao));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.AguardandoValidacao));

            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TiposInscricao, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.IntegrarNoSGA, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Dres, "1"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.NomeFormacao, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.PublicosAlvo, "2"));

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceristas = ObterTodos<PropostaParecerista>();
            pareceristas.Any(a=> a.Situacao.EstaEnviada()).ShouldBeTrue();
            pareceristas.Any(a=> a.Situacao.EstaAguardandoValidacao()).ShouldBeTrue();

            var propostas = ObterTodos<Dominio.Entidades.Proposta>();
            propostas.All(a=> a.Situacao.EstaAguardandoAnaliseParecerista()).ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - O perfil Admin DF deve permitir enviar parecer e a situação da proposta deve ser alterada para aguardando análise da área promotora")]
        public async Task O_perfil_admin_df_deve_enviar_parecer_e_a_situacao_da_proposta_deve_ser_alterada_para_aguardando_analise_da_area_promotora()
        {
           // arrange
			var perfilLogado = Perfis.ADMIN_DF.ToString();
			CriarClaimUsuario(perfilLogado, "4", "Admin DF");

			await InserirUsuario("1", "Parecerista1");
			await InserirUsuario("2", "Parecerista2");
			await InserirUsuario("3", "Parecerista3");
			await InserirUsuario("4", "Admin DF");

			await InserirParametrosProposta();

			var proposta = await InserirNaBaseProposta(
				SituacaoProposta.AguardandoAnaliseParecerPelaDF,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3", SituacaoParecerista.Enviada));

			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceristas = ObterTodos<PropostaParecerista>();
            pareceristas.All(a=> a.Situacao.EstaEnviada()).ShouldBeTrue();

            var propostas = ObterTodos<Dominio.Entidades.Proposta>();
            propostas.All(a=> a.Situacao.EstaAnaliseParecerPelaAreaPromotora()).ShouldBeTrue();
        }
        
         [Fact(DisplayName = "Proposta - A Área Promotora deve enviar parecer e a situação da proposta deve ser alterada para aguardando a reanálise do parecerista")]
        public async Task A_area_promotora_deve_enviar_parecer_e_a_situacao_da_proposta_deve_ser_alterada_para_aguardando_a_reanalise_do_parecerista()
        {
           // arrange
			var perfilLogado = Perfis.COPED.ToString();
			CriarClaimUsuario(perfilLogado, "4", "Área Promotora");

			await InserirUsuario("1", "Parecerista1");
			await InserirUsuario("2", "Parecerista2");
			await InserirUsuario("3", "Parecerista3");
			await InserirUsuario("4", "Área Promotora");

			await InserirParametrosProposta();

			var proposta = await InserirNaBaseProposta(
				SituacaoProposta.AnaliseParecerPelaAreaPromotora,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3", SituacaoParecerista.Enviada));

			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceristas = ObterTodos<PropostaParecerista>();
            pareceristas.All(a=> a.Situacao.EstaEnviada()).ShouldBeTrue();

            var propostas = ObterTodos<Dominio.Entidades.Proposta>();
            propostas.All(a=> a.Situacao.EstaAguardandoReanalisePeloParecerista()).ShouldBeTrue();
        }
    }
}
