using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;
using SME.ConectaFormacao.Dominio.Entidades;
using DiffEngine;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_proposta : TestePropostaBase
    {
        public Ao_obter_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve obter por id válido")]
        public async Task Deve_obter_por_id_valido()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            ValidarPropostaCompletoDTO(propostaCompletoDTO, proposta.Id);
            ValidarPropostaPublicoAlvoDTO(propostaCompletoDTO.PublicosAlvo, proposta.Id);
            ValidarPropostaFuncaoEspecificaDTO(propostaCompletoDTO.FuncoesEspecificas, proposta.Id);
            ValidarPropostaVagaRemanecenteDTO(propostaCompletoDTO.VagasRemanecentes, proposta.Id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaCompletoDTO.CriteriosValidacaoInscricao, proposta.Id);
            ValidarPropostaPalavrasChavesDTO(propostaCompletoDTO.PalavrasChaves, proposta.Id);
            ValidarAuditoriaDTO(proposta, propostaCompletoDTO.Auditoria);
            ValidarPropostaTurmasDTO(propostaCompletoDTO.Turmas, proposta.Id);
            ValidarPropostaModalidadesDTO(propostaCompletoDTO.Modalidades, proposta.Id);
            ValidarPropostaAnosTurmasDTO(propostaCompletoDTO.AnosTurmas, proposta.Id);
            ValidarPropostaComponentesCurricularesDTO(propostaCompletoDTO.ComponentesCurriculares, proposta.Id);
            ValidarPropostaTipoInscricaoDTO(propostaCompletoDTO.TiposInscricao, proposta.Id);
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção ao obter por id inválido")]
        public async Task Deve_retornar_excecao_ao_obter_por_id_invalido()
        {
            // arrange 
            var idAleatorio = PropostaSalvarMock.GerarIdAleatorio();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(idAleatorio));

            // assert 
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA).ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil Admin DF somente total de considerações enviadas - todas enviadas")]
        public async Task Deve_obter_proposta_completa_com_perfil_admin_df_somente_total_de_consideracoes_enviadas_todas_enviadas()
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
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TiposInscricao, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.IntegrarNoSGA, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Dres, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TipoFormacao, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Dres, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.NomeFormacao, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.PublicosAlvo, "2"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Formato, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.FormacaoHomologada, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TipoFormacao, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TiposInscricao, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.IntegrarNoSGA, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(8);
            propostaCompletoDTO.TotalDeConsideracoes.All(a=> a.Quantidade == 3).ShouldBeTrue();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
            propostaCompletoDTO.PodeEnviar.ShouldBeTrue();
            propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
            propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
            propostaCompletoDTO.PodeAprovar.ShouldBeTrue();
            propostaCompletoDTO.PodeRecusar.ShouldBeTrue();
            propostaCompletoDTO.LabelAprovar.ShouldBe("Aprovar");
            propostaCompletoDTO.LabelRecusar.ShouldBe("Recusar");
        }
        
        [Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil de Área Promotora somente total de considerações enviadas - parcialmente enviadas")]
		public async Task Deve_obter_proposta_completa_com_perfil_de_area_promotora_somente_total_de_consideracoes_enviadas_parcialmente_enviadas()
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
				SituacaoProposta.AguardandoAnaliseParecerPelaDF,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3", SituacaoParecerista.AguardandoValidacao));

			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TiposInscricao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.IntegrarNoSGA, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Dres, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TipoFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Dres, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.NomeFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.PublicosAlvo, "2"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Formato, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.FormacaoHomologada, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TipoFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TiposInscricao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.IntegrarNoSGA, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));

			// act 
			var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
			var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

			// assert 
			propostaCompletoDTO.ShouldNotBeNull();
			propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(8);
			propostaCompletoDTO.TotalDeConsideracoes.All(a=> a.Quantidade == 2).ShouldBeTrue();
			propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
			propostaCompletoDTO.PodeEnviar.ShouldBeFalse();
			propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
			propostaCompletoDTO.PodeAprovar.ShouldBeFalse();
			propostaCompletoDTO.PodeRecusar.ShouldBeFalse();
			propostaCompletoDTO.LabelAprovar.ShouldBe("Aprovar");
			propostaCompletoDTO.LabelRecusar.ShouldBe("Recusar");
		}

		[Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil de Área Promotora com todas as considerações enviadas")]
		public async Task Deve_obter_proposta_completa_com_perfil_de_area_promotora_com_todas_as_consideracoes_enviadas()
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
				SituacaoProposta.AguardandoAnaliseParecerPelaDF,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3", SituacaoParecerista.Enviada));
		
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TiposInscricao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.IntegrarNoSGA, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Dres, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TipoFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Dres, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.NomeFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.PublicosAlvo, "2"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Formato, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.FormacaoHomologada, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TipoFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TiposInscricao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.IntegrarNoSGA, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));
		
			// act 
			var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
			var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);
		
			// assert 
			propostaCompletoDTO.ShouldNotBeNull();
			propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(8);
			propostaCompletoDTO.TotalDeConsideracoes.All(a=> a.Quantidade == 3).ShouldBeTrue();
			propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
			propostaCompletoDTO.PodeEnviar.ShouldBeFalse();
			propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
			propostaCompletoDTO.PodeAprovar.ShouldBeFalse();
			propostaCompletoDTO.PodeRecusar.ShouldBeFalse();
			propostaCompletoDTO.LabelAprovar.ShouldBe("Aprovar");
			propostaCompletoDTO.LabelRecusar.ShouldBe("Recusar");
		}
		
		[Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil Parecerista com todas as considerações - enviadas parcialmente e permitir enviar suas considerações")]
		public async Task Deve_obter_proposta_completa_com_perfil_parecerista_com_todas_as_consideracoes_enviadas_parcialmente_permitir_enviar_suas_consideracoes()
		{
			// arrange
			var perfilLogado = Perfis.PARECERISTA.ToString();
			CriarClaimUsuario(perfilLogado, "3", "Parecerista3");

			await InserirUsuario("1", "Parecerista1");
			await InserirUsuario("2", "Parecerista2");
			await InserirUsuario("3", "Parecerista3");

			await InserirParametrosProposta();

			var proposta = await InserirNaBaseProposta(
				SituacaoProposta.AguardandoAnalisePeloParecerista,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3", SituacaoParecerista.AguardandoValidacao));

			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TiposInscricao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.IntegrarNoSGA, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Dres, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TipoFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Dres, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.NomeFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.PublicosAlvo, "2"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Formato, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.FormacaoHomologada, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TipoFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TiposInscricao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.IntegrarNoSGA, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));

			// act 
			var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
			var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

			// assert 
			propostaCompletoDTO.ShouldNotBeNull();
			propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(8);
			propostaCompletoDTO.TotalDeConsideracoes.All(a=> a.Quantidade == 3).ShouldBeTrue();
			propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
			propostaCompletoDTO.PodeEnviar.ShouldBeFalse();
			propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeTrue();
			propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
			propostaCompletoDTO.PodeAprovar.ShouldBeFalse();
			propostaCompletoDTO.PodeRecusar.ShouldBeFalse();
			propostaCompletoDTO.LabelAprovar.ShouldBe("Sugerir aprovação");
			propostaCompletoDTO.LabelRecusar.ShouldBe("Sugerir recusa");
		}
		
		[Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil Parecerista com todas as considerações - enviadas parcialmente - não permitir enviar suas considerações")]
		public async Task Deve_obter_proposta_completa_com_perfil_parecerista_com_todas_as_consideracoes_enviadas_parcialmente_nao_permitir_enviar_suas_recomendacoes()
		{
			// arrange
			var perfilLogado = Perfis.PARECERISTA.ToString();
			CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

			await InserirUsuario("1", "Parecerista1");
			await InserirUsuario("2", "Parecerista2");
			await InserirUsuario("3", "Parecerista3");

			await InserirParametrosProposta();

			var proposta = await InserirNaBaseProposta(
				SituacaoProposta.AguardandoAnalisePeloParecerista,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3", SituacaoParecerista.AguardandoValidacao));

			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TiposInscricao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.IntegrarNoSGA, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Dres, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TipoFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Dres, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.NomeFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.PublicosAlvo, "2"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Formato, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.FormacaoHomologada, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TipoFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TiposInscricao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.IntegrarNoSGA, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));

			// act 
			var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
			var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

			// assert 
			propostaCompletoDTO.ShouldNotBeNull();
			propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(8);
			propostaCompletoDTO.TotalDeConsideracoes.All(a=> a.Quantidade == 3).ShouldBeTrue();
			propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
			propostaCompletoDTO.PodeEnviar.ShouldBeFalse();
			propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
			propostaCompletoDTO.PodeAprovar.ShouldBeFalse();
			propostaCompletoDTO.PodeRecusar.ShouldBeFalse();
			propostaCompletoDTO.LabelAprovar.ShouldBe("Sugerir aprovação");
			propostaCompletoDTO.LabelRecusar.ShouldBe("Sugerir recusa");
		}
		
		[Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil Parecerista sem considerações e permitir sugerir aprovação e recusa")]
		public async Task Deve_obter_proposta_completa_com_perfil_parecerista_sem_consideracoes_e_permitir_sugerir_aprovacao_e_recusa()
		{
			// arrange
			var perfilLogado = Perfis.PARECERISTA.ToString();
			CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

			await InserirUsuario("1", "Parecerista1");
			await InserirUsuario("2", "Parecerista2");
			await InserirUsuario("3", "Parecerista3");

			await InserirParametrosProposta();

			var proposta = await InserirNaBaseProposta(
				SituacaoProposta.AguardandoAnalisePeloParecerista,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3"));
			
			// act 
			var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
			var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

			// assert 
			propostaCompletoDTO.ShouldNotBeNull();
			propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(0);
			propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
			propostaCompletoDTO.PodeEnviar.ShouldBeFalse();
			propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
			propostaCompletoDTO.PodeAprovar.ShouldBeTrue();
			propostaCompletoDTO.PodeRecusar.ShouldBeTrue();
			propostaCompletoDTO.LabelAprovar.ShouldBe("Sugerir aprovação");
			propostaCompletoDTO.LabelRecusar.ShouldBe("Sugerir recusa");
		}
		
		[Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil Parecerista com considerações de outros pareceristas e permitir sugerir aprovação e recusa")]
		public async Task Deve_obter_proposta_completa_com_perfil_parecerista_com_consideracoes_de_outros_pareceristas_e_permitir_sugerir_aprovacao_e_recusa()
		{
			// arrange
			var perfilLogado = Perfis.PARECERISTA.ToString();
			CriarClaimUsuario(perfilLogado, "3", "Parecerista3");

			await InserirUsuario("1", "Parecerista1");
			await InserirUsuario("2", "Parecerista2");
			await InserirUsuario("3", "Parecerista3");

			await InserirParametrosProposta();

			var proposta = await InserirNaBaseProposta(
				SituacaoProposta.AguardandoAnalisePeloParecerista,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TiposInscricao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.IntegrarNoSGA, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Dres, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TipoFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Dres, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.NomeFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.PublicosAlvo, "2"));
	
			// act 
			var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
			var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

			// assert 
			propostaCompletoDTO.ShouldNotBeNull();
			propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(8);
			propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
			propostaCompletoDTO.PodeEnviar.ShouldBeFalse();
			propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
			propostaCompletoDTO.PodeAprovar.ShouldBeTrue();
			propostaCompletoDTO.PodeRecusar.ShouldBeTrue();
			propostaCompletoDTO.LabelAprovar.ShouldBe("Sugerir aprovação");
			propostaCompletoDTO.LabelRecusar.ShouldBe("Sugerir recusa");
		}
		
		[Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil Admin DF sem considerações e permitir aprovar e recusar")]
		public async Task Deve_obter_proposta_completa_com_perfil_admin_df_sem_consideracoes_e_permitir_sugerir_aprovacao_e_recusa()
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
				SituacaoProposta.AguardandoAnaliseDf,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3"));
			
			// act 
			var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
			var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

			// assert 
			propostaCompletoDTO.ShouldNotBeNull();
			propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(0);
			propostaCompletoDTO.ExibirConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.PodeEnviar.ShouldBeTrue();
			propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
			propostaCompletoDTO.PodeAprovar.ShouldBeFalse();
			propostaCompletoDTO.PodeRecusar.ShouldBeFalse();
			propostaCompletoDTO.LabelAprovar.ShouldBe("Aprovar");
			propostaCompletoDTO.LabelRecusar.ShouldBe("Recusar");
		}
		
		[Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil Admin DF em aguardando análise final pela DF e permitir aprovar e recusar")]
		public async Task Deve_obter_proposta_completa_com_perfil_admin_df_em_aguardando_analise_final_pela_df_e_permitir_sugerir_aprovacao_e_recusa()
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
				SituacaoProposta.AguardandoValidacaoFinalPelaDF,
				FormacaoHomologada.Sim,
				TipoInscricao.Externa);
			
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
			await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3"));
			
			// act 
			var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
			var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

			// assert 
			propostaCompletoDTO.ShouldNotBeNull();
			propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(0);
			propostaCompletoDTO.ExibirConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.PodeEnviar.ShouldBeFalse();
			propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
			propostaCompletoDTO.PodeAprovar.ShouldBeTrue();
			propostaCompletoDTO.PodeRecusar.ShouldBeTrue();
			propostaCompletoDTO.LabelAprovar.ShouldBe("Aprovar");
			propostaCompletoDTO.LabelRecusar.ShouldBe("Recusar");
		}
		
		[Fact(DisplayName = "Proposta - Deve obter proposta completa com perfil de Área Promotora e permitir enviar para reanálise")]
		public async Task Deve_obter_proposta_completa_com_perfil_de_area_promotora_e_permitir_enviar_para_reanalise()
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
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TiposInscricao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.IntegrarNoSGA, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Dres, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "1"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TipoFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Dres, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.NomeFormacao, "2"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.PublicosAlvo, "2"));
			
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Formato, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.FormacaoHomologada, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TipoFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.TiposInscricao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.IntegrarNoSGA, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
			await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));

			// act 
			var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
			var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

			// assert 
			propostaCompletoDTO.ShouldNotBeNull();
			propostaCompletoDTO.TotalDeConsideracoes.Count().ShouldBe(8);
			propostaCompletoDTO.TotalDeConsideracoes.All(a=> a.Quantidade == 3).ShouldBeTrue();
			propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
			propostaCompletoDTO.PodeEnviar.ShouldBeTrue();
			propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
			propostaCompletoDTO.QtdeLimitePareceristaProposta.ShouldBe(3);
			propostaCompletoDTO.PodeAprovar.ShouldBeFalse();
			propostaCompletoDTO.PodeRecusar.ShouldBeFalse();
			propostaCompletoDTO.LabelAprovar.ShouldBe("Aprovar");
			propostaCompletoDTO.LabelRecusar.ShouldBe("Recusar");
		}

        [Fact(DisplayName = "Proposta - Deve obter por id válido com permissões do perfil logado adm df")]
        public async Task Deve_obter_por_id_com_permissoes_parecer_perfil_logado_adm()
        {
	        // arrange
	        var perfilLogado = Perfis.ADMIN_DF.ToString();
	        CriarClaimUsuario(perfilLogado, "3", "Admin DF");

	        await InserirUsuario("1", "Parecerista1");
	        await InserirUsuario("2", "Parecerista2");
	        await InserirUsuario("3", "Admin DF");

	        await InserirParametrosProposta();

	        var proposta = await InserirNaBaseProposta(
		        SituacaoProposta.AguardandoAnaliseParecerPelaDF,
		        FormacaoHomologada.Sim,
		        TipoInscricao.Externa);
	
	        await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
	        await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));

	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "1"));
	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
	
	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
        }


        [Fact(DisplayName = "Proposta - Deve obter por id válido com permissões do perfil logado área promotora")]
        public async Task Deve_obter_por_id_com_permissoes_parecer_perfil_logado_area_promotora()
        {
	        // arrange
	        var perfilLogado = Perfis.COPED.ToString();
	        CriarClaimUsuario(perfilLogado, "4", "Área promotora");

	        await InserirUsuario("1", "Parecerista1");
	        await InserirUsuario("2", "Parecerista2");
	        await InserirUsuario("4", "Área promotora");

	        await InserirParametrosProposta();

	        var proposta = await InserirNaBaseProposta(
		        SituacaoProposta.AnaliseParecerPelaAreaPromotora,
		        FormacaoHomologada.Sim,
		        TipoInscricao.Externa);
	
	        await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
	        await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));

	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.NomeFormacao, "1"));
	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
	
	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
	        
            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve obter por id válido verificando se pode enviar considerações")]
        public async Task Deve_obter_por_id_verificado_pode_enviar_consideracoes()
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

	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.PublicosAlvo, "1"));
	
	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
	        await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
	        
            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
        }


        [Fact(DisplayName = "Proposta - Deve obter por id válido verificando se pode enviar sem parecerista")]
        public async Task Deve_obter_por_id_verificado_pode_enviar_sem_parecerista()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnaliseDf);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviar.ShouldBeFalse();
        }

        [Fact(DisplayName = "Proposta - Deve obter por id válido verificando se pode enviar com parecerista")]
        public async Task Deve_obter_por_id_verificado_pode_enviar_com_parecerista()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnaliseDf);

            var parecerista = new PropostaParecerista
            {
                PropostaId = proposta.Id,
                NomeParecerista = $"Parecerista {usuario.Nome}",
                RegistroFuncional = usuario.Login,
                CriadoPor = proposta.CriadoPor,
                CriadoEm = proposta.CriadoEm,
                CriadoLogin = proposta.CriadoLogin
            };

            await InserirNaBase(parecerista);

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviar.ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve obter por id válido do tipo inscrição externo")]
        public async Task Deve_obter_por_id_valido_tipo_inscricao_externo()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.Cadastrada,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            ValidarPropostaCompletoDTO(propostaCompletoDTO, proposta.Id);
            ValidarPropostaTipoInscricaoDTO(propostaCompletoDTO.TiposInscricao, proposta.Id);
        }
        
        [Fact(DisplayName = "Proposta - Deve permitir enviar quando a proposta estiver na situação cadastrada")]
        public async Task Deve_permitir_enviar_quando_a_proposta_estiver_na_situacao_cadastrada()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.Cadastrada,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviar.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Deve permitir enviar quando a proposta estiver na situação devolvida")]
        public async Task Deve_permitir_enviar_quando_a_proposta_estiver_na_situacao_devolvida()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.Devolvida,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviar.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Deve permitir enviar quando a proposta estiver na situação Aguardando Analise Df com pareceristas")]
        public async Task Deve_permitir_enviar_quando_a_proposta_estiver_na_situacao_aguardando_analise_df_com_parecerista()
        {
            // arrange
            var perfilLogado = Perfis.ADMIN_DF.ToString();

            CriarClaimUsuario(perfilLogado, "3", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", perfilLogado);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnaliseDf,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviar.ShouldBeTrue();
        }
        
        [Theory(DisplayName = "Proposta - Não deve permitir enviar quando a proposta estiver na situação Aguardando Analise Df sem pareceristas")]
        [InlineData(Constantes.ADMIN_DF)]
        [InlineData(Constantes.PARECERISTA)]
        public async Task Nao_deve_permitir_enviar_quando_a_proposta_estiver_na_situacao_aguardando_analise_df_sem_parecerista(string perfilLogado)
        {
            // arrange
            CriarClaimUsuario(perfilLogado, "1", perfilLogado);

            await InserirUsuario("1", perfilLogado);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnaliseDf,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviar.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta - Deve permitir exibir parecer quando sou parecerista da proposta")]
        public async Task Deve_permitir_exibir_parecer_quando_sou_parecerista_da_proposta()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            
            CriarClaimUsuario(perfilLogado, "2", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnalisePeloParecerista,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Não deve permitir exibir parecer quando não sou parecerista da proposta")]
        public async Task Nao_deve_permitir_exibir_parecer_quando_nao_sou_parecerista_da_proposta()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            
            CriarClaimUsuario(perfilLogado, "3", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", perfilLogado);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnalisePeloParecerista,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeFalse();
        }
        
        [Theory(DisplayName = "Proposta - Deve permitir ao Admin DF e Área Promotora exibir parecer quando possuir parecerista na proposta e proposta está aguardando análise do parecer df")]
        [InlineData(Constantes.ADMIN_DF)]
        [InlineData(Constantes.AREA_PROMOTORA)]
        public async Task Deve_permitir_ao_admin_df_e_area_promotora_exibir_parecer_quando_possuir_parecerista_na_proposta_e_proposta_esta_aguardando_analise_do_parecer_df(string perfilLogado)
        {
            // arrange
            CriarClaimUsuario(perfilLogado, "3", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", perfilLogado);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnaliseParecerPelaDF,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa,
                perfilLogado:perfilLogado);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
	
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Deve permitir ao parecerista exibir parecer quando possuir parecerista na proposta e proposta está aguardando análise do parecer df")]
        public async Task Deve_permitir_ao_parecerista_exibir_parecer_quando_possuir_parecerista_na_proposta_e_proposta_esta_aguardando_analise_do_parecer_df()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnaliseParecerPelaDF,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Nao deve permitir a perfis diferentes de Admins DFs, Áreas Promotoras e Pareceristas exibir parecer quando possuir parecerista na proposta e proposta está aguardando análise do parecer df")]
        public async Task Nao_deve_permitir_a_perfis_diferentes_de_admin_df_area_promotora_e_parecerista_exibir_parecer_quando_possuir_parecerista_na_proposta_e_proposta_esta_aguardando_analise_do_parecer_df()
        {
            // arrange
            var perfilLogado = Perfis.SINPEEM.ToString();
            CriarClaimUsuario(perfilLogado, "3", "Não Parecerista, Admin DF e AP");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Não Parecerista, Admin DF e AP");

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnaliseParecerPelaDF,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeFalse();
        }
        
        [Theory(DisplayName = "Proposta - Deve permitir ao Admin DF e Área Promotora exibir consideracoes quando possuir parecerista na proposta e proposta está em análise parecer área promotora")]
        [InlineData(Constantes.ADMIN_DF)]
        [InlineData(Constantes.AREA_PROMOTORA)]
        public async Task Deve_permitir_ao_admin_df_e_area_promotora_exibir_consideracoes_quando_possuir_parecerista_na_proposta_e_proposta_esta_em_analise_parecer_area_promotora(string perfilLogado)
        {
            // arrange
            CriarClaimUsuario(perfilLogado, "3", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", perfilLogado);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AnaliseParecerPelaAreaPromotora,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa,
                perfilLogado:perfilLogado);
	
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
	
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.FormacaoHomologada, "2"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Deve permitir ao parecerista exibir parecer quando possuir parecerista na proposta e proposta está em análise parecer área promotora")]
        public async Task Deve_permitir_ao_parecerista_exibir_parecer_quando_possuir_parecerista_na_proposta_e_proposta_esta_em_analise_parecer_area_promotora()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AnaliseParecerPelaAreaPromotora,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
	
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
	
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Nao deve permitir a perfis diferentes de Admins DFs, Áreas Promotoras e Pareceristas exibir parecer quando possuir parecerista na proposta e proposta está em análise parecer área promotora")]
        public async Task Nao_deve_permitir_a_perfis_diferentes_de_admin_df_area_promotora_e_parecerista_exibir_parecer_quando_possuir_parecerista_na_proposta_e_proposta__esta_em_analise_parecer_area_promotora()
        {
            // arrange
            var perfilLogado = Perfis.SINPEEM.ToString();
            CriarClaimUsuario(perfilLogado, "3", "Não Parecerista, Admin DF e AP");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Não Parecerista, Admin DF e AP");

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AnaliseParecerPelaAreaPromotora,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
	
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
	
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirConsideracoes.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta - Deve permitir ao perfil parecerista enviar seus pareceres quando estiverem na situação pendente de envio pelo parecerista")]
        public async Task Deve_permitir_ao_perfil_parecerista_enviar_seus_pareceres_quando_estiverem_na_situacao_pendente_de_envio_pelo_parecerista()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            await InserirParametrosProposta();

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnalisePeloParecerista,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
	
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
	
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato,"1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato,"2"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Deve permitir ao perfil parecerista enviar seus pareceres quando estiverem na situação aguardando análise do parecer DF")]
        public async Task Nao_deve_permitir_ao_perfil_parecerista_enviar_seus_pareceres_quando_estiverem_na_situacao_aguardando_analise_parecer_df()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnaliseParecerPelaDF,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
	
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
	
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato));
            //TODO
            // await InserirNaBase(PropostaParecerMock.GerarPropostaParecer(proposta.Id, 2,CampoParecer.Formato, SituacaoParecerista.AguardandoAnaliseParecerPeloAdminDF));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta - Deve permitir ao perfil Admin DF enviar as considerações quando estiverem na situação aguardando análise do parecer pela DF")]
        public async Task Deve_permitir_ao_perfil_admin_df_enviar_as_consideracoes_quando_estiverem_na_situacao_aguardando_analise_parecer_pela_df()
        {
            // arrange
            var perfilLogado = Perfis.ADMIN_DF.ToString();
            CriarClaimUsuario(perfilLogado, "3", "Admin DF");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Admin DF");

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnaliseParecerPelaDF,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
	
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
	
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta - Não deve permitir ao perfil de Área Promotora enviar os pareceres quando estiverem na situação aguardando análise do parecer DF")]
        public async Task Nao_deve_permitir_ao_perfil_de_area_promotora_enviar_os_pareceres_quando_estiverem_na_situacao_aguardando_analise_parecer_df()
        {
            // arrange
            var perfilLogado = Perfis.COPED.ToString();
            CriarClaimUsuario(perfilLogado, "3", "Área Promotora");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Área Promotora");

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.AguardandoAnaliseParecerPelaDF,
                FormacaoHomologada.Sim,
                TipoInscricao.Externa);
	
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
	
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.Formato, "2"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviarConsideracoes.ShouldBeFalse();
        }

        private void AdicionarPerfilUsuarioContextoAplicacao(Guid perfil, string login)
        {
            var contextoAplicacao = ServiceProvider.GetService<IContextoAplicacao>();
            var variaveis = new Dictionary<string, object>
                {
                    { "PerfilUsuario", perfil.ToString() },
                     { "UsuarioLogado", login }
                };

            contextoAplicacao.AdicionarVariaveis(variaveis);
        }
    }
}
