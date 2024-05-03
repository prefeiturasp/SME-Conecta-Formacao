using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Enumerados;
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

        [Fact(DisplayName = "Proposta - Deve obter por id válido com total de pareceres")]
        public async Task Deve_obter_por_id_com_total_pareceres()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

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

        [Fact(DisplayName = "Proposta - Deve obter por id válido com permissões do perfil logado adm df")]
        public async Task Deve_obter_por_id_com_permissoes_parecer_perfil_logado_adm()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnaliseParecerDF, quantidadeParecerista: 1);

            await GerarPropostaParecer(proposta.Id, CampoParecer.FormacaoHomologada);

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();

            propostaCompletoDTO.ExibirParecer.ShouldBeTrue();
        }


        [Fact(DisplayName = "Proposta - Deve obter por id válido com permissões do perfil logado área promotora")]
        public async Task Deve_obter_por_id_com_permissoes_parecer_perfil_logado_area_promotora()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var perfil = Guid.NewGuid();
            AdicionarPerfilUsuarioContextoAplicacao(perfil, usuario.Login);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            await InserirNaBase(AreaPromotoraMock.GerarAreaPromotora(perfil));

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AnaliseParecerAreaPromotora, quantidadeParecerista: 1);

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.ExibirParecer.ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve obter por id válido verificando se pode enviar parecer")]
        public async Task Deve_obter_por_id_verificado_pode_enviar_parecer()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta, "3");
            await InserirNaBase(parametro);

            await InserirNaBase(AreaPromotoraMock.GerarAreaPromotora(Perfis.ADMIN_DF));

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnaliseDf, quantidadeParecerista: 2);

            await GerarPropostaParecer(proposta.Id, CampoParecer.FormacaoHomologada);

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            propostaCompletoDTO.PodeEnviarParecer.ShouldBeTrue();
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

        private async Task GerarPropostaParecer(long propostaId, CampoParecer campo, SituacaoParecer situacao = SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF, long usuarioParecerista = 0)
        {
            for (int contador = 0; contador < (int)campo; contador++)
            {
                var inserirPropostaParecer = PropostaParecerMock.GerarPropostaParecer();
                inserirPropostaParecer.PropostaId = propostaId;
                inserirPropostaParecer.Campo = campo;
                inserirPropostaParecer.Situacao = situacao;
                inserirPropostaParecer.UsuarioPareceristaId = usuarioParecerista;
                await InserirNaBase(inserirPropostaParecer);
            }
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
