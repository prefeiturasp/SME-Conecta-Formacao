using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_proposta_parecerista_consideracao : TestePropostaBase
    {
        public Ao_obter_proposta_parecerista_consideracao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Considerações do Parecista - Deve permitir inserir quando não tiver parecer sido inserido pelo parecerista")]
        public async Task Deve_permitir_inserir_quando_nao_tiver_sido_inserido_parecer_pelo_parecerista()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
                
            CriarClaimUsuario(perfilLogado, "1", "Parecerista1");

            await InserirUsuario("1", "Parecerista1");
            
            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerista, quantidadeParecerista: 1);

            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            
            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count().ShouldBe(0);
            propostaCompletoDTO.PodeInserir.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Não deve permitir inserir, mas pode alterar quando tiver parecer inserido pelo parecerista")]
        public async Task Nao_deve_permitir_inserir_mas_pode_alterar_quando_tiver_parecer_inserido_pelo_parecerista()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            
            CriarClaimUsuario(Perfis.PARECERISTA.ToString(), "1", "Parecerista1");

            await InserirUsuario("1", "Parecerista1");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado,situacao: SituacaoProposta.AguardandoAnaliseParecerista, quantidadeParecerista: 1);
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count().ShouldBe(1);
            propostaCompletoDTO.Itens.All(a=> a.PodeAlterar).ShouldBeTrue();
            propostaCompletoDTO.PodeInserir.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir inserir quando não tiver parecer inserido pelo parecerista e ter parecer por outro parecerista")]
        public async Task Deve_permitir_inserir_quando_nao_tiver_parecer_inserido_pelo_parecerista_e_ter_por_outro_parecerista()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado,situacao: SituacaoProposta.AguardandoAnaliseParecerista);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count().ShouldBe(1);
            propostaCompletoDTO.PodeInserir.ShouldBeTrue();
            propostaCompletoDTO.Itens.All(a=> a.PodeAlterar).ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir alterar quando parecerista já tiver inserido parecer e não deve permitir inserir novos pareceres")]
        public async Task Deve_permitir_alterar_quando_parecerista_ja_tiver_inserido_parecer_e_nao_deve_permitir_inserir_novos_pareceres()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado,situacao: SituacaoProposta.AguardandoAnaliseParecerista);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato,"1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoParecer.Formato,"2"));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count().ShouldBe(2);
            propostaCompletoDTO.PodeInserir.ShouldBeFalse();
            propostaCompletoDTO.Itens.Any(a=> a.PodeAlterar && a.Id == 2).ShouldBeTrue();
            propostaCompletoDTO.Itens.Any(a=> !a.PodeAlterar && a.Id == 1).ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Não deve permitir ao Admin DF ver pareceres que estão em situação pendente com pareceristas")]
        public async Task Nao_deve_admin_df_ver_pareceres_que_estao_em_situacao_pendente_com_pareceristas()
        {
            // arrange
            var perfilLogado = Perfis.ADMIN_DF.ToString();
            
            CriarClaimUsuario(perfilLogado, "3", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", perfilLogado);

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato,"1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoParecer.Formato,"2"));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count().ShouldBe(0);
            propostaCompletoDTO.PodeInserir.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir ao Admin DF ver somente pareceres enviados (pendente de DF) e não ver pareceres que estão em situação pendente com pareceristas")]
        public async Task Deve_permitir_ao_admin_df_ver_somente_pareceres_enviados_e_nao_ver_pareceres_que_estao_em_situacao_pendente_com_pareceristas()
        {
            // arrange
            var perfilLogado = Perfis.ADMIN_DF.ToString();
            
            CriarClaimUsuario(perfilLogado, "3", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", perfilLogado);

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.AguardandoValidacao));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato,"1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoParecer.Formato,"2"));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count(a=> a.PodeAlterar).ShouldBe(1);
            propostaCompletoDTO.Itens.Count(a=> !a.PodeAlterar).ShouldBe(0);
            propostaCompletoDTO.PodeInserir.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir ao perfil de Área Promotora ver somente pareceres enviados (pendente de DF) e não ver pareceres que estão em situação pendente com pareceristas")]
        public async Task Deve_permitir_ao_perfil_de_area_promotra_ver_somente_pareceres_enviados_e_nao_ver_pareceres_que_estao_em_situacao_pendente_com_pareceristas()
        {
            // arrange
            var perfilLogado = Perfis.COPED.ToString();
            
            CriarClaimUsuario(perfilLogado, "3", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", perfilLogado);

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1",SituacaoParecerista.AguardandoValidacao));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2",SituacaoParecerista.Enviada));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato,"1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoParecer.Formato,"2"));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count(a=> a.PodeAlterar).ShouldBe(0);
            propostaCompletoDTO.Itens.Count(a=> !a.PodeAlterar).ShouldBe(1);
            propostaCompletoDTO.PodeInserir.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Não deve permitir ao parecerista inserir quando não tiver pareceres e a proposta tiver outros pareceres e a proposta está na situação AguardandoAnaliseParecerDF")]
        public async Task Deve_permitir_ao_parecerista_somente_inserir_quando_nao_tiver_pareceres_do_parecerista_e_a_proposta_estiver_pendente_de_parecerista()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            
            CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoParecer.Formato, "2"));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count(a=> a.PodeAlterar).ShouldBe(0);
            propostaCompletoDTO.Itens.Count(a=> !a.PodeAlterar).ShouldBe(2);
            propostaCompletoDTO.PodeInserir.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir ao Admin DF alterar pareceres que estão em situação aguardando análise do parecer (DF)")]
        public async Task Deve_permitir_ao_admin_df_alterar_pareceres_que_estao_em_situacao_aguardando_analise_do_parecer_df()
        {
            // arrange
            var perfilLogado = Perfis.ADMIN_DF.ToString();
            
            CriarClaimUsuario(perfilLogado, "3", Perfis.ADMIN_DF.ToString());

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Admin DF");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoParecer.Formato, "2"));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count(a=> a.PodeAlterar).ShouldBe(2);
            propostaCompletoDTO.Itens.Count(a=> !a.PodeAlterar).ShouldBe(0);
            propostaCompletoDTO.PodeInserir.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Não deve permitir ao perfil Área Promotora alterar pareceres que estão em situação aguardando análise do parecer (DF)")]
        public async Task Nap_deve_permitir_a_area_promotora_alterar_pareceres_que_estao_em_situacao_aguardando_analise_do_parecer_df()
        {
            // arrange
            var perfilLogado = Perfis.COPED.ToString();
            
            CriarClaimUsuario(perfilLogado, "3", Perfis.ADMIN_DF.ToString());

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Área Promotora COPED");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoParecer.Formato, "2"));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count(a=> a.PodeAlterar).ShouldBe(0);
            propostaCompletoDTO.Itens.Count(a=> !a.PodeAlterar).ShouldBe(2);
            propostaCompletoDTO.PodeInserir.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir ao parecerista sem parecer, inserir novos e ver os outros pareceres sem auditoria")]
        public async Task Deve_permitir_ao_parecerista_sem_parecer_inserir_novos_e_ver_os_outros_pareceres_sem_auditoria()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            
            CriarClaimUsuario(perfilLogado, "3", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Parecerista3");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerista);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoParecer.Formato));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count(a=> a.PodeAlterar).ShouldBe(0);
            propostaCompletoDTO.Itens.Count(a=> !a.PodeAlterar).ShouldBe(2);
            propostaCompletoDTO.Itens.All(a=> a.Auditoria.EhNulo()).ShouldBeTrue();
            propostaCompletoDTO.PodeInserir.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir ao parecerista com parecer ver auditoria")]
        public async Task Deve_permitir_ao_parecerista_com_parecer_ver_auditoria()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            
            CriarClaimUsuario(perfilLogado, "1", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerista);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoParecer.Formato,"1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoParecer.Formato,"2"));

            var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(filtro);
            
            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
            propostaCompletoDTO.Itens.Count(a=> a.PodeAlterar).ShouldBe(1);
            propostaCompletoDTO.Itens.Count(a=> !a.PodeAlterar).ShouldBe(1);
            propostaCompletoDTO.Itens.Count(a=> a.Auditoria.NaoEhNulo()).ShouldBe(1);
            propostaCompletoDTO.Itens.Count(a=> a.Auditoria.EhNulo()).ShouldBe(1);
            propostaCompletoDTO.PodeInserir.ShouldBeFalse();
        }
        
        
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir ao parecerista inserir parecer quando não tem parecer cadastrado")]
        public async Task Deve_permitir_ao_parecerista_inserir_parecer_quando_nao_tem_parecer_cadastrado()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString());
            await InserirUsuario("1", "Parecerista 1");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerista);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            
            // act
            var filtro = new PropostaParecerFiltroDTO()
            {
                Campo = CampoParecer.FormacaoHomologada,
                PropostaId = proposta.Id
            };
            
            var retorno = await useCase.Executar(filtro);

            // assert
            retorno.PodeInserir.ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Não deve permitir ao parecerista inserir parecer quando tem parecer cadastrado pelo mesmo parecerista")]
        public async Task Nao_deve_permitir_ao_parecerista_inserir_parecer_quando_tem_parecer_cadastrado_pelo_mesmo_parecerista()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString(), "1", "Parecerista1");
            
            await InserirUsuario("1", "Parecerista1");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerista);

            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            
            var consideracaoDoParecista = PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoParecer.FormacaoHomologada, "1");
            await InserirNaBase(consideracaoDoParecista);
            
            // act
            var filtro = new PropostaParecerFiltroDTO()
            {
                Campo = CampoParecer.FormacaoHomologada,
                PropostaId = proposta.Id
            };
            
            var retorno = await useCase.Executar(filtro);

            // assert
            retorno.PodeInserir.ShouldBeFalse();
            retorno.PropostaId.ShouldBe(proposta.Id);
            retorno.Itens.FirstOrDefault().Id.ShouldBe(1);
            retorno.Itens.FirstOrDefault().Campo.ShouldBe(CampoParecer.FormacaoHomologada);
            retorno.Itens.FirstOrDefault().Descricao.ShouldBe(consideracaoDoParecista.Descricao);
            retorno.Itens.FirstOrDefault().PodeAlterar.ShouldBeTrue();
            retorno.Itens.FirstOrDefault().Auditoria.ShouldNotBeNull();
            retorno.Itens.FirstOrDefault().Auditoria.Id.ShouldBe(1);
            retorno.Itens.FirstOrDefault().Auditoria.CriadoLogin.ShouldBe("1");
            retorno.Itens.FirstOrDefault().Auditoria.CriadoEm.Date.ShouldBe(DateTimeExtension.HorarioBrasilia().Date);
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir ao cursista inserir parecer quando tem parecer cadastrado por outro parecerista")]
        public async Task Deve_permitir_ao_parecerista_inserir_parecer_quando_tem_parecer_cadastrado_por_outro_cursista()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString(), "2", "Parecerista2");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerista);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoParecer.FormacaoHomologada, "1"));
            
            // act
            var filtro = new PropostaParecerFiltroDTO()
            {
                Campo = CampoParecer.FormacaoHomologada,
                PropostaId = proposta.Id
            };
            
            var retorno = await useCase.Executar(filtro);

            // assert
            retorno.PodeInserir.ShouldBeTrue();
            retorno.PropostaId.ShouldBe(proposta.Id);
            retorno.Itens.Count().ShouldBe(1);
            retorno.Itens.All(a=> !a.PodeAlterar).ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir ao Admin DF alterar parecer e não permitir inserir parecer")]
        public async Task Deve_permitir_ao_admin_df_alterar_parecer()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.ADMIN_DF.ToString(), "2", "Admin DF");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Admin DF");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoParecer.FormacaoHomologada, "1"));
            
            // act
            var filtro = new PropostaParecerFiltroDTO()
            {
                Campo = CampoParecer.FormacaoHomologada,
                PropostaId = proposta.Id
            };
            
            var retorno = await useCase.Executar(filtro);

            // assert
            retorno.PodeInserir.ShouldBeFalse();
            retorno.PropostaId.ShouldBe(proposta.Id);
            retorno.Itens.FirstOrDefault().Id.ShouldBe(1);
            retorno.Itens.FirstOrDefault().Campo.ShouldBe(CampoParecer.FormacaoHomologada);
            // retorno.Itens.FirstOrDefault().Descricao.ShouldBe(inserirPropostaParecer.Descricao);
            retorno.Itens.FirstOrDefault().PodeAlterar.ShouldBeTrue();
            
            retorno.Itens.FirstOrDefault().Auditoria.ShouldNotBeNull();
            retorno.Itens.FirstOrDefault().Auditoria.Id.ShouldBe(1);
            retorno.Itens.FirstOrDefault().Auditoria.CriadoLogin.ShouldBe("1");
            retorno.Itens.FirstOrDefault().Auditoria.CriadoEm.Date.ShouldBe(DateTimeExtension.HorarioBrasilia().Date);
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve permitir Admin DF alterar parecer e não inserir parecer")]
        public async Task Deve_permitir_ao_admin_df_alterar_parecer_e_nao_inserir_parecer()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.ADMIN_DF.ToString(), "4", "Admin DF");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Área Promotora Perfil SINPEEM");
            await InserirUsuario("4", "Admin DF");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoParecer.FormacaoHomologada, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2, CampoParecer.FormacaoHomologada, "2"));
            
            // act
            var filtro = new PropostaParecerFiltroDTO()
            {
                Campo = CampoParecer.FormacaoHomologada,
                PropostaId = proposta.Id
            };
            
            var retorno = await useCase.Executar(filtro);

            // assert
            retorno.PodeInserir.ShouldBeFalse();
            retorno.PropostaId.ShouldBe(proposta.Id);
            retorno.Itens.Count().ShouldBe(2);
            retorno.Itens.Count(c=> c.Campo == CampoParecer.FormacaoHomologada).ShouldBe(2);
            retorno.Itens.Count(c=> c.PodeAlterar).ShouldBe(2);
            retorno.Itens.All(a=> a.Auditoria.EhNulo()).ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Não deve permitir a Área Promotora inserir a alterar parecer")]
        public async Task Nao_deve_permitir_a_area_promotora_inserir_e_alterar_parecer()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.SINPEEM.ToString(), "3", "Área Promotora Perfil SINPEEM");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Área Promotora Perfil SINPEEM");
            await InserirUsuario("4", "Admin DF");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerDF);
            
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(Dominio.Constantes.Perfis.SINPEEM);
            await InserirNaBase(areaPromotora);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoParecer.FormacaoHomologada, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2, CampoParecer.FormacaoHomologada, "2"));
      
            // act
            var filtro = new PropostaParecerFiltroDTO()
            {
                Campo = CampoParecer.FormacaoHomologada,
                PropostaId = proposta.Id
            };
            
            var retorno = await useCase.Executar(filtro);

            // assert
            retorno.PodeInserir.ShouldBeFalse();
            retorno.PropostaId.ShouldBe(proposta.Id);
            retorno.Itens.Count().ShouldBe(2);
            retorno.Itens.Count(c=> c.Campo == CampoParecer.FormacaoHomologada).ShouldBe(2);
            retorno.Itens.Count(c=> !c.PodeAlterar).ShouldBe(2);
            retorno.Itens.All(a=> a.Auditoria.EhNulo()).ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Considerações do Parecista - Deve retornar somente os pareceres do parecerista que os criou")]
        public async Task Deve_retornar_somente_os_pareceres_do_parecerista_que_os_criou()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString(), "2", "Parecerista2");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Área Promotora Perfil SINPEEM");
            await InserirUsuario("4", "Admin DF");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerista);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1", SituacaoParecerista.AguardandoValidacao));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.AguardandoValidacao));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoParecer.FormacaoHomologada, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2, CampoParecer.FormacaoHomologada, "2"));
            
            // act
            var filtro = new PropostaParecerFiltroDTO()
            {
                Campo = CampoParecer.FormacaoHomologada,
                PropostaId = proposta.Id
            };
            
            var retorno = await useCase.Executar(filtro);

            // assert
            retorno.PodeInserir.ShouldBeFalse();
            retorno.PropostaId.ShouldBe(proposta.Id);
            retorno.Itens.Count().ShouldBe(2);
            retorno.Itens.Count(c=> c.Campo == CampoParecer.FormacaoHomologada).ShouldBe(2);
            retorno.Itens.Count(c=> c.PodeAlterar).ShouldBe(1);
            retorno.Itens.All(a=> a.Auditoria.EhNulo()).ShouldBeFalse();
        }
    }
}
