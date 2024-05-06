using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_salvar_parecer_proposta : TestePropostaBase
    {
        public Ao_salvar_parecer_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta parecer - Cadastrar um parecer")]
        public async Task Deve_cadastrar_proposta_parecer()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.COPED.ToString());
            await InserirUsuario("1", "Área Promotora");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var propostaParecerDto = PropostaSalvarMock.GerarParecerCadastro();
            propostaParecerDto.PropostaId = proposta.Id;
            
            // act
            var retorno = await useCase.Executar(propostaParecerDto);

            // assert
            retorno.EntidadeId.ShouldBeGreaterThan(0);
            var propostaParecer = ObterTodos<PropostaParecer>().FirstOrDefault();
            propostaParecer.Id.ShouldBe(1);
            propostaParecer.Campo.ShouldBe(propostaParecerDto.Campo);
            propostaParecer.Descricao.ShouldBe(propostaParecerDto.Descricao);
            propostaParecer.PropostaId.ShouldBe(proposta.Id);
            propostaParecer.Excluido.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta parecer - Alterar parecer")]
        public async Task Deve_alterar_proposta_parecer()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.COPED.ToString());
            await InserirUsuario("1", "Área Promotora");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var inserirPropostaParecer = PropostaParecerMock.GerarPropostasPareceres(1);
            await InserirPareceresDaProposta(inserirPropostaParecer, "1",proposta.Id);
            
            var alterarPropostaParecer = PropostaSalvarMock.GerarParecerCadastro();
            alterarPropostaParecer.PropostaId = proposta.Id;
            alterarPropostaParecer.Id = inserirPropostaParecer.FirstOrDefault().Id;
            
            // act
            var retorno = await useCase.Executar(alterarPropostaParecer);

            // assert
            retorno.EntidadeId.ShouldBeGreaterThan(0);
            var propostaParecer = ObterTodos<PropostaParecer>().FirstOrDefault();
            propostaParecer.Id.ShouldBe(1);
            propostaParecer.Descricao.ShouldBe(alterarPropostaParecer.Descricao);
            propostaParecer.PropostaId.ShouldBe(proposta.Id);
            propostaParecer.Excluido.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta parecer - Deve permitir ao cursista inserir parecer quando não tem parecer cadastrado")]
        public async Task Deve_permitir_ao_cursista_inserir_parecer_quando_nao_tem_parecer_cadastrado()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString());
            await InserirUsuario("1", "Área Promotora");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
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
        
        [Fact(DisplayName = "Proposta parecer - Não deve permitir ao cursista inserir parecer quando tem parecer cadastrado pelo mesmo cursista")]
        public async Task Nao_deve_permitir_ao_cursista_inserir_parecer_quando_tem_parecer_cadastrado_pelo_mesmo_cursista()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString(), "1", "Parecerista1");
            
            await InserirUsuario("1", "Parecerista1");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var inserirPropostaParecer = PropostaParecerMock.GerarPropostaParecer();
            inserirPropostaParecer.PropostaId = proposta.Id;
            inserirPropostaParecer.CriadoLogin = "1";
            inserirPropostaParecer.Campo = CampoParecer.FormacaoHomologada;
            await InserirNaBase(inserirPropostaParecer);
            
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
            retorno.Itens.FirstOrDefault().Descricao.ShouldBe(inserirPropostaParecer.Descricao);
            retorno.Itens.FirstOrDefault().PodeAlterar.ShouldBeTrue();
            retorno.Itens.FirstOrDefault().Auditoria.ShouldNotBeNull();
            retorno.Itens.FirstOrDefault().Auditoria.Id.ShouldBe(1);
            retorno.Itens.FirstOrDefault().Auditoria.CriadoLogin.ShouldBe("1");
            retorno.Itens.FirstOrDefault().Auditoria.CriadoEm.Date.ShouldBe(DateTimeExtension.HorarioBrasilia().Date);
        }
        
        [Fact(DisplayName = "Proposta parecer - Deve permitir ao cursista inserir parecer quando tem parecer cadastrado por outro cursista")]
        public async Task Deve_permitir_ao_cursista_inserir_parecer_quando_tem_parecer_cadastrado_por_outro_cursista()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString(), "2", "Parecerista2");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var inserirPropostaParecer = PropostaParecerMock.GerarPropostaParecer();
            inserirPropostaParecer.PropostaId = proposta.Id;
            inserirPropostaParecer.CriadoLogin = "1";
            inserirPropostaParecer.Campo = CampoParecer.FormacaoHomologada;
            await InserirNaBase(inserirPropostaParecer);
            
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
            retorno.Itens.Count().ShouldBe(0);
        }
        
        [Fact(DisplayName = "Proposta parecer - Deve permitir ao Admin DF alterar parecer e não permitir inserir parecer")]
        public async Task Deve_permitir_ao_admin_df_alterar_parecer()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.ADMIN_DF.ToString(), "2", "Admin DF");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Admin DF");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var inserirPropostaParecer = PropostaParecerMock.GerarPropostaParecer();
            inserirPropostaParecer.PropostaId = proposta.Id;
            inserirPropostaParecer.CriadoLogin = "1";
            inserirPropostaParecer.Campo = CampoParecer.FormacaoHomologada;
            await InserirNaBase(inserirPropostaParecer);
            
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
            retorno.Itens.FirstOrDefault().Descricao.ShouldBe(inserirPropostaParecer.Descricao);
            retorno.Itens.FirstOrDefault().PodeAlterar.ShouldBeTrue();
            
            retorno.Itens.FirstOrDefault().Auditoria.ShouldNotBeNull();
            retorno.Itens.FirstOrDefault().Auditoria.Id.ShouldBe(1);
            retorno.Itens.FirstOrDefault().Auditoria.CriadoLogin.ShouldBe("1");
            retorno.Itens.FirstOrDefault().Auditoria.CriadoEm.Date.ShouldBe(DateTimeExtension.HorarioBrasilia().Date);
        }
        
        [Fact(DisplayName = "Proposta parecer - Deve permitir Admin DF alterar parecer e não inserir parecer")]
        public async Task Deve_permitir_ao_admin_df_alterar_parecer_e_nao_inserir_parecer()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.ADMIN_DF.ToString(), "4", "Admin DF");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Área Promotora Perfil SINPEEM");
            await InserirUsuario("4", "Admin DF");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var inserirPropostasPareceresParecer = PropostaParecerMock.GerarPropostasPareceres(5);
            await InserirPareceresDaProposta(inserirPropostasPareceresParecer, "1",proposta.Id);
            
            inserirPropostasPareceresParecer = PropostaParecerMock.GerarPropostasPareceres(5);
            await InserirPareceresDaProposta(inserirPropostasPareceresParecer, "2",proposta.Id);
            
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
            // retorno.
            // retorno.Auditoria.Id.ShouldBe(10);
            // retorno.Auditoria.CriadoLogin.ShouldBe("2");
            // retorno.Auditoria.CriadoEm.Date.ShouldBe(DateTimeExtension.HorarioBrasilia().Date);
            retorno.Itens.Count().ShouldBe(10);
            retorno.Itens.Count(c=> c.Campo == CampoParecer.FormacaoHomologada).ShouldBe(10);
            retorno.Itens.Count(c=> c.PodeAlterar).ShouldBe(10);
            
            // retorno.Itens.Any().Auditoria.ShouldNotBeNull();
        }
        
        [Fact(DisplayName = "Proposta parecer - Não deve permitir a Área Promotora inserir a alterar parecer")]
        public async Task Nao_deve_permitir_a_area_promotora_inserir_e_alterar_parecer()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.SINPEEM.ToString(), "3", "Área Promotora Perfil SINPEEM");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Área Promotora Perfil SINPEEM");
            await InserirUsuario("4", "Admin DF");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(Dominio.Constantes.Perfis.SINPEEM);
            await InserirNaBase(areaPromotora);
            
            var inserirPropostasPareceresParecer = PropostaParecerMock.GerarPropostasPareceres(5);
            await InserirPareceresDaProposta(inserirPropostasPareceresParecer, "1",proposta.Id);
            
            inserirPropostasPareceresParecer = PropostaParecerMock.GerarPropostasPareceres(5);
            await InserirPareceresDaProposta(inserirPropostasPareceresParecer, "2",proposta.Id);
            
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
            // retorno.Auditoria.ShouldNotBeNull();
            // retorno.Auditoria.Id.ShouldBe(10);
            // retorno.Auditoria.CriadoLogin.ShouldBe("2");
            // retorno.Auditoria.CriadoEm.Date.ShouldBe(DateTimeExtension.HorarioBrasilia().Date);
            retorno.Itens.Count().ShouldBe(10);
            retorno.Itens.Count(c=> c.Campo == CampoParecer.FormacaoHomologada).ShouldBe(10);
            retorno.Itens.Count(c=> !c.PodeAlterar).ShouldBe(10);
        }
        
        [Fact(DisplayName = "Proposta parecer - Deve retornar somente os pareceres do parecerista que os criou")]
        public async Task Deve_retornar_somente_os_pareceres_do_parecerista_que_os_criou()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString(), "2", "Parecerista2");
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Área Promotora Perfil SINPEEM");
            await InserirUsuario("4", "Admin DF");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(Dominio.Constantes.Perfis.SINPEEM);
            await InserirNaBase(areaPromotora);
            
            var inserirPropostasPareceresParecer = PropostaParecerMock.GerarPropostasPareceres(5);
            await InserirPareceresDaProposta(inserirPropostasPareceresParecer, "1",proposta.Id);
            
            inserirPropostasPareceresParecer = PropostaParecerMock.GerarPropostasPareceres(5);
            await InserirPareceresDaProposta(inserirPropostasPareceresParecer, "2",proposta.Id);
            
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
            // retorno.Auditoria.ShouldNotBeNull();
            // retorno.Auditoria.Id.ShouldBe(10);
            // retorno.Auditoria.CriadoLogin.ShouldBe("2");
            // retorno.Auditoria.CriadoEm.Date.ShouldBe(DateTimeExtension.HorarioBrasilia().Date);
            retorno.Itens.Count().ShouldBe(5);
            retorno.Itens.Count(c=> c.Campo == CampoParecer.FormacaoHomologada).ShouldBe(5);
            retorno.Itens.Count(c=> c.PodeAlterar).ShouldBe(5);
        }

        private async Task InserirPareceresDaProposta(IEnumerable<PropostaParecer> inserirPropostasPareceresParecer, string login, long propostaId, CampoParecer campoParecer = CampoParecer.FormacaoHomologada)
        {
            foreach (var propostaParecer in inserirPropostasPareceresParecer)
            {
                propostaParecer.PropostaId = propostaId;
                propostaParecer.CriadoLogin = login;
                propostaParecer.Campo = campoParecer;
                await InserirNaBase(propostaParecer);
            }
        }
    }
}