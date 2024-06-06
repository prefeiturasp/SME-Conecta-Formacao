using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Funcionario;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using System.Text.Json;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao
{
    public class Ao_enviar_parecer_pelo_parecerista_para_df : TestePropostaBase
    {
        public Ao_enviar_parecer_pelo_parecerista_para_df(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuariosPorPerfisServicoEolQuery, IEnumerable<UsuarioPerfilServicoEol>>), typeof(ObterUsuariosPorPerfisServicoEolQueryFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Notificacao - Deve notificar os Admins DFs que foi enviado parecer do parecerista")]
        public async Task Deve_notificar_os_admins_dfs_que_foi_enviado_parecer_do_parecerista()
        {
            // arrange
            await InserirParametrosProposta();
            await InserirNaBase(ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.UrlConectaFormacaoEdicaoProposta, "https://conectaformacao/cadastro/cadastro-de-propostas/editar/{0}"));

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

            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Parecerista3");
            await InserirUsuario("4", "ResponsavelDf");

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.AguardandoAnalisePeloParecerista, responsavelDF: "4");

            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1", "Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2", "Parecerista2"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3", "Parecerista3"));

            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoConsideracao.FormacaoHomologada, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoConsideracao.TipoFormacao, "1"));

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoNotificarDFPeloEnvioParecerPeloParecerista>();
            var mapper = ObterCasoDeUso<IMapper>();

            var pareceristas = ObterTodos<PropostaParecerista>();

            var filtro = new NotificacaoPropostaPareceristaDTO(proposta.Id, mapper.Map<PropostaPareceristaResumidoDTO>(pareceristas.FirstOrDefault()));

            // act
            var mensagem = JsonSerializer.Serialize(filtro);
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit(mensagem));

            // assert 
            retorno.ShouldBeTrue();

            var notificacoes = ObterTodos<Dominio.Entidades.Notificacao>();
            notificacoes.Count().ShouldBe(1);
            var notificacao = notificacoes.FirstOrDefault();

            notificacao.Mensagem.ShouldBe(string.Format("O Parecerista {0} ({1}) Inseriu comentários na proposta {2} - {3}. Acesse <a href=\"{4}\">Aqui</a> o cadastro da proposta.",
                pareceristas.FirstOrDefault().NomeParecerista,
                pareceristas.FirstOrDefault().RegistroFuncional,
                proposta.Id,
                proposta.NomeFormacao,
                $"https://conectaformacao/cadastro/cadastro-de-propostas/editar/{proposta.Id}"));

            notificacao.Titulo.ShouldBe(string.Format("Proposta {0} - {1} foi analisada pelo Parecerista", proposta.Id, proposta.NomeFormacao));
            notificacao.Categoria.ShouldBe(NotificacaoCategoria.Aviso);
            notificacao.Tipo.ShouldBe(NotificacaoTipo.Proposta);
            notificacao.Parametros.ShouldNotBeEmpty();

            var notificacoesUsuarios = ObterTodos<NotificacaoUsuario>();
            notificacoesUsuarios.Count().ShouldBe(1);
            notificacoesUsuarios.Count(a => a.Situacao.EhNaoLida() && a.NotificacaoId == 1).ShouldBe(1);
        }

        [Fact(DisplayName = "Notificacao - Não deve notificar os Admins DFs que foi enviado parecer do parecerista")]
        public async Task Nao_deve_notificar_os_admins_dfs_que_foi_enviado_parecer_do_parecerista()
        {
            // arrange
            await InserirParametrosProposta();
            await InserirNaBase(ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.UrlConectaFormacaoEdicaoProposta, "http://conecta"));

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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.AguardandoAnaliseDf);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF>();

            var filtro = new NotificacaoPropostaPareceristasDTO(proposta.Id, null);

            // act
            var mensagem = JsonSerializer.Serialize(filtro);
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit(mensagem));

            // assert 
            retorno.ShouldBeFalse();

            var notificacoes = ObterTodos<Dominio.Entidades.Notificacao>();
            notificacoes.Count().ShouldBe(0);

            var notificacoesUsuarios = ObterTodos<Dominio.Entidades.NotificacaoUsuario>();
            notificacoesUsuarios.Count().ShouldBe(0);
        }
    }
}
