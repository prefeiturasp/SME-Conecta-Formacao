using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_proposta_parecer : TestePropostaBase
    {
        public Ao_obter_proposta_parecer(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta parecer - Deve permitir inserir quando não tiver parecer sido inserido pelo parecerista")]
        public async Task Deve_permitir_inserir_quando_nao_tiver_sido_inserido_parecer_pelo_parecerista()
        {
            // arrange

            var perfilLogado = Perfis.PARECERISTA.ToString();
                
            CriarClaimUsuario(perfilLogado, "1", "Parecerista1");

            await InserirUsuario("1", "Parecerista1");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerista, quantidadeParecerista: 1);

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
        
        [Fact(DisplayName = "Proposta parecer - Não deve permitir inserir, mas pode alterar quando tiver parecer inserido pelo parecerista")]
        public async Task Nao_deve_permitir_inserir_mas_pode_alterar_quando_tiver_parecer_inserido_pelo_parecerista()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            
            CriarClaimUsuario(Perfis.PARECERISTA.ToString(), "1", "Parecerista1");

            await InserirUsuario("1", "Parecerista1");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado,situacao: SituacaoProposta.AguardandoAnaliseParecerista, quantidadeParecerista: 1);
            
            await InserirNaBase(PropostaParecerMock.GerarPropostaParecer(proposta.Id, 1,CampoParecer.Formato));

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
        
        [Fact(DisplayName = "Proposta parecer - Deve permitir inserir quando não tiver parecer inserido pelo parecerista e ter parecer por outro parecerista")]
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
            
            await InserirNaBase(PropostaParecerMock.GerarPropostaParecer(proposta.Id, 1,CampoParecer.Formato));

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
        
        [Fact(DisplayName = "Proposta parecer - Deve permitir alterar quando parecerista já tiver inserido parecer e não deve permitir inserir novos pareceres")]
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
            
            var propostaParecer = PropostaParecerMock.GerarPropostaParecer(proposta.Id, 1,CampoParecer.Formato);
            await InserirNaBase(propostaParecer);
            propostaParecer = PropostaParecerMock.GerarPropostaParecer(proposta.Id, 2,CampoParecer.Formato);
            await InserirNaBase(propostaParecer);

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
        
        [Fact(DisplayName = "Proposta parecer - Não deve permitir ao Admin DF ver pareceres que estão em situação pendente com pareceristas")]
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
            
            var propostaParecer = PropostaParecerMock.GerarPropostaParecer(proposta.Id, 1,CampoParecer.Formato);
            await InserirNaBase(propostaParecer);
            propostaParecer = PropostaParecerMock.GerarPropostaParecer(proposta.Id, 2,CampoParecer.Formato);
            await InserirNaBase(propostaParecer);

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
        
        [Fact(DisplayName = "Proposta parecer - Deve permitir ao Admin DF ver somente pareceres enviados (pendente de DF) e não ver pareceres que estão em situação pendente com pareceristas")]
        public async Task Deve_permitir_ao_admin_df_ver_somente_pareceres_enviados_e_nao_ver_pareceres_que_estao_em_situacao_pendente_com_pareceristas()
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
            
            var propostaParecer = PropostaParecerMock.GerarPropostaParecer(proposta.Id, 1,CampoParecer.Formato);
            propostaParecer.Situacao = SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF;
            await InserirNaBase(propostaParecer);
            propostaParecer = PropostaParecerMock.GerarPropostaParecer(proposta.Id, 2,CampoParecer.Formato);
            await InserirNaBase(propostaParecer);

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
        
        [Fact(DisplayName = "Proposta parecer - Deve permitir ao perfil de Área Promotora ver somente pareceres enviados (pendente de DF) e não ver pareceres que estão em situação pendente com pareceristas")]
        public async Task Deve_permitir_ao_perfil_de_area_promotra_ver_somente_pareceres_enviados_e_nao_ver_pareceres_que_estao_em_situacao_pendente_com_pareceristas()
        {
            // arrange
            var perfilLogado = Perfis.COPED.ToString();
            
            CriarClaimUsuario(perfilLogado, "3", perfilLogado);

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", perfilLogado);

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            await InserirNaBase(PropostaParecerMock.GerarPropostaParecer(proposta.Id, 1,CampoParecer.Formato,SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF));
            await InserirNaBase(PropostaParecerMock.GerarPropostaParecer(proposta.Id, 2,CampoParecer.Formato));

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
        
        [Fact(DisplayName = "Proposta parecer - Não deve permitir ao parecerista inserir quando não tiver pareceres e a proposta tiver outros pareceres e a proposta está na situação AguardandoAnaliseParecerDF")]
        public async Task Deve_permitir_ao_parecerista_somente_inserir_quando_nao_tiver_pareceres_do_parecerista_e_a_proposta_estiver_pendente_de_parecerista()
        {
            // arrange
            var perfilLogado = Perfis.PARECERISTA.ToString();
            
            CriarClaimUsuario(perfilLogado, "2", "Parecerista2");

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");

            var proposta = await InserirNaBaseProposta(perfilLogado: perfilLogado, situacao: SituacaoProposta.AguardandoAnaliseParecerDF);
            
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2"));
            
            await InserirNaBase(PropostaParecerMock.GerarPropostaParecer(proposta.Id, 1,CampoParecer.Formato, SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF));
            await InserirNaBase(PropostaParecerMock.GerarPropostaParecer(proposta.Id, 2,CampoParecer.Formato, SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF));

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
        
        
        //Admin DF pode alterar pareceres que estão em pendente de DF
        //AP não pode alterar pareceres que estão em pendente de DF
        
        // [Fact(DisplayName = "Proposta parecer - Não deve permitir inserir quando Admin Df, mas deve permitir alterar e excluir pareceresquando tiver parecer inserido pelo parecerista")]
        // public async Task Nao_deve_permitir_inserir_mas_pode_alterar_quando_tiver_parecer_inserido_pelo_parecerista()
        // {
        //     // arrange
        //     CriarClaimUsuario(Perfis.ADMIN_DF.ToString(), "2", "AdminDf");
        //
        //     await InserirUsuario("1", "Parecerista1");
        //     await InserirUsuario("2", "AdminDf");
        //
        //     var proposta = await InserirNaBaseProposta(quantidadeParecerista: 1);
        //     
        //     var propostaParecer = PropostaParecerMock.GerarPropostaParecer(proposta.Id, 1,CampoParecer.Formato);
        //     
        //     await InserirNaBase(propostaParecer);
        //
        //     var filtro = PropostaSalvarMock.GeradorPropostaParecerFiltroDTO(proposta.Id, CampoParecer.Formato);
        //     var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
        //     
        //     // act 
        //     var propostaCompletoDTO = await casoDeUso.Executar(filtro);
        //     
        //     // assert 
        //     propostaCompletoDTO.ShouldNotBeNull();
        //     propostaCompletoDTO.PropostaId.ShouldBe(proposta.Id);
        //     propostaCompletoDTO.Itens.Count().ShouldBe(1);
        //     propostaCompletoDTO.Itens.All(a=> a.PodeAlterar).ShouldBeTrue();
        //     propostaCompletoDTO.PodeInserir.ShouldBeFalse();
        // }
    }
}
