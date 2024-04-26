using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

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
            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF);

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

        [Fact(DisplayName = "Proposta - Deve obter por id válido com total de pareceres")]
        public async Task Deve_obter_por_id_com_total_pareceres()
        {
            // arrange
            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF);

            var proposta = await InserirNaBaseProposta();

            await GerarPropostaParecer(proposta.Id, CampoParecer.FormacaoHomologada);

            await GerarPropostaParecer(proposta.Id, CampoParecer.TipoFormacao);

            await GerarPropostaParecer(proposta.Id, CampoParecer.Formato);
            
            await GerarPropostaParecer(proposta.Id, CampoParecer.TiposInscricao);

            await GerarPropostaParecer(proposta.Id, CampoParecer.IntegrarNoSGA);

            await GerarPropostaParecer(proposta.Id, CampoParecer.Dres);

            await GerarPropostaParecer(proposta.Id, CampoParecer.NomeFormacao);

            await GerarPropostaParecer(proposta.Id, CampoParecer.PublicosAlvo);

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();

            propostaCompletoDTO.TotalDePareceres.Count().ShouldBeGreaterThan(0);

            foreach (var totalParecer in propostaCompletoDTO.TotalDePareceres)
                totalParecer.Quantidade.ShouldBe((int)totalParecer.Campo);
        }

        [Fact(DisplayName = "Proposta - Deve obter por id válido com permissões do perfil logado pareceristas")]
        public async Task Deve_obter_por_id_com_permissoes_parecer_perfil_logado_pareceristas()
        {
            // arrange
            AdicionarPerfilUsuarioContextoAplicacao(Perfis.PARECERISTA);

            var proposta = await InserirNaBaseProposta();

            var pemissoesParecerista = new[] {
                PermissaoTela.CONSULTA,
                PermissaoTela.INCLUSAO,
                PermissaoTela.EXCLUSAO,
                PermissaoTela.ALTERACAO
            };

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();

            propostaCompletoDTO.PermissaoParecerPerfilLogado.Count().ShouldBe(4);

            foreach (var permissao in propostaCompletoDTO.PermissaoParecerPerfilLogado)
                pemissoesParecerista.Contains(permissao).ShouldBeTrue();
        }


        [Fact(DisplayName = "Proposta - Deve obter por id válido com permissões do perfil logado adm df")]
        public async Task Deve_obter_por_id_com_permissoes_parecer_perfil_logado_adm()
        {
            // arrange
            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF);

            var proposta = await InserirNaBaseProposta();

            var pemissoesParecerista = new[] {
                PermissaoTela.CONSULTA,
                PermissaoTela.EXCLUSAO,
                PermissaoTela.ALTERACAO
            };

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();

            propostaCompletoDTO.PermissaoParecerPerfilLogado.Count().ShouldBe(3);

            foreach (var permissao in propostaCompletoDTO.PermissaoParecerPerfilLogado)
                pemissoesParecerista.Contains(permissao).ShouldBeTrue();
        }


        [Fact(DisplayName = "Proposta - Deve obter por id válido com permissões do perfil logado área promotora")]
        public async Task Deve_obter_por_id_com_permissoes_parecer_perfil_logado_area_promotora()
        {
            // arrange
            var perfil = Guid.NewGuid();
            AdicionarPerfilUsuarioContextoAplicacao(perfil);

            await InserirNaBase(AreaPromotoraMock.GerarAreaPromotora(perfil));

            var proposta = await InserirNaBaseProposta();

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PermissaoParecerPerfilLogado.Count().ShouldBe(1);
            propostaCompletoDTO.PermissaoParecerPerfilLogado.First().ShouldBe(PermissaoTela.CONSULTA);
        }

        private async Task GerarPropostaParecer(long propostaId, CampoParecer campo)
        {
            for (int contador = 0; contador < (int)campo; contador++)
            {
                var inserirPropostaParecer = PropostaParecerMock.GerarPropostaParecer();
                inserirPropostaParecer.PropostaId = propostaId;
                inserirPropostaParecer.Campo = campo;
                await InserirNaBase(inserirPropostaParecer);
            }
        }

        private void AdicionarPerfilUsuarioContextoAplicacao(Guid perfil)
        {
            var contextoAplicacao = ServiceProvider.GetService<IContextoAplicacao>();
            var variaveis = new Dictionary<string, object>
                {
                    { "PerfilUsuario", perfil.ToString() }
                };

            contextoAplicacao.AdicionarVariaveis(variaveis);
        }
    }
}
