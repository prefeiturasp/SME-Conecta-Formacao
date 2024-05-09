using Bogus;
using DiffEngine;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_propostas_para_dashboard : TestePropostaBase
    {
        public Ao_obter_propostas_para_dashboard(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Deve Exibir Todas Situações de Proposta no Dashboard")]
        public async Task Deve_obter_lista_de_cada_situacao_para_exibir_no_dashboard()
        {
            // arrange
            await CriarPropostaValida();
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);
            AdicionarPerfilUsuarioParecerista(Perfis.ADMIN_DF, usuario.Login);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostasDashboard>();
            var filtro = new PropostaFiltrosDashboardDTO();
            var situacoes = Enum.GetValues(typeof(SituacaoProposta)).Cast<SituacaoProposta>();

            // act 
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.ShouldNotBeNull();
            retorno.Count().ShouldBeEquivalentTo(situacoes.Count());

            foreach (var situacao in situacoes)
                retorno.Count(x => x.Situacao.Nome() == situacao.Nome()).ShouldBeEquivalentTo(1);

        }

        [Fact(DisplayName = "Proposta - Deve Exibir Todas Situações de Proposta no Dashboard por número de homologação")]
        public async Task Deve_obter_lista_proposta_dashboard_por_numero_homologacao()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);
            AdicionarPerfilUsuarioParecerista(Perfis.ADMIN_DF, usuario.Login);
            await CriarPropostaValida();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostasDashboard>();
            var filtro = new PropostaFiltrosDashboardDTO();

            var proposta = ObterTodos<Dominio.Entidades.Proposta>().FirstOrDefault();

            filtro.NumeroHomologacao = proposta.NumeroHomologacao;

            // act 
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.ShouldNotBeNull();
            retorno.Count().ShouldBe(1);
        }

        [Fact(DisplayName = "Proposta - Deve proposta no dashboard para usuário logado parecerista")]
        public async Task Deve_obter_lista_proposta_dashboard_para_usuario_logado_parecerista()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);
            AdicionarPerfilUsuarioParecerista(Perfis.PARECERISTA, usuario.Login);
            await CriarPropostaValida();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostasDashboard>();

            var proposta = ObterTodos<Dominio.Entidades.Proposta>().FirstOrDefault(p => p.Situacao == SituacaoProposta.AguardandoAnalisePeloParecerista);

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
            var retorno = await casoDeUso.Executar(new PropostaFiltrosDashboardDTO());

            // assert
            retorno.ShouldNotBeNull();
            retorno.Count().ShouldBe(1);
            retorno.FirstOrDefault().Situacao.ShouldBe(SituacaoProposta.AguardandoAnalisePeloParecerista);
            retorno.FirstOrDefault().Propostas.Count().ShouldBe(1);
        }


        private void AdicionarPerfilUsuarioParecerista(Guid perfil, string login)
        {
            var contextoAplicacao = ServiceProvider.GetService<IContextoAplicacao>();
            var variaveis = new Dictionary<string, object>
                {
                    { "PerfilUsuario", perfil.ToString() },
                    { "UsuarioLogado", login },
                };

            contextoAplicacao.AdicionarVariaveis(variaveis);

            PropostaSalvarMock.GrupoUsuarioLogadoId = perfil;
        }

        #region Criar Uma Proposta Valida de Cada Situação
        private async Task CriarPropostaValida()
        {
            var parametroComunicadoAcaoFormativaDescricao = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaDescricao);
            await InserirNaBase(parametroComunicadoAcaoFormativaDescricao);

            var parametroComunicadoAcaoFormativaUrl = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaUrl);
            await InserirNaBase(parametroComunicadoAcaoFormativaUrl);

            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var criterios = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5, false);
            await InserirNaBase(criterios);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO() { ComponenteCurricularId = t.Id });

            var listaDeSituacoesExistentes = Enum.GetValues(typeof(SituacaoProposta)).Cast<SituacaoProposta>();

            foreach (var situacao in listaDeSituacoesExistentes)
            {
                var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                    TipoFormacao.Curso,
                    Formato.Presencial,
                    dreDTO,
                    publicosAlvoDTO,
                    funcoesEspecificaDTO,
                    criteriosDTO,
                    vagasRemanecentesDTO,
                    palavrasChavesDTO,
                    modalidadesDTO,
                    anosTurmasDTO,
                    componentesCurricularesDTO,
                situacao);

                propostaDTO.NumeroHomologacao = new Random().NextInt64(100000, 9999999999);

                var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();
                await casoDeUso.Executar(propostaDTO);
            }
        }
        #endregion

    }
}