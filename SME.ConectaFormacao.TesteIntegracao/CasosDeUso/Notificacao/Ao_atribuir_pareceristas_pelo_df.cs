using System.Text.Json;
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
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao
{
    public class Ao_atribuir_pareceristas_pelo_df : TestePropostaBase
    {
        public Ao_atribuir_pareceristas_pelo_df(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Notificacao - Deve notificar os pareceristas que foram atribuídos pelo DF")]
        public async Task Deve_notificar_os_pareceristas_que_foram_atribuidos_pelo_df()
        {
            // arrange
            await InserirParametrosProposta();
            await InserirNaBase(ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.UrlConectaFormacao, "http://conecta"));

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

            await InserirUsuario("1", "Parecerista1", "parecerista1@email.com");
            await InserirUsuario("2", "Parecerista2", "parecerista2@email.com");
            await InserirUsuario("3", "Parecerista3", "parecerista3@email.com");

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.AguardandoAnalisePeloParecerista);

            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1", "Parecerista1"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2", "Parecerista2"));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3", "Parecerista3"));
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF>();
            var mapper = ObterCasoDeUso<IMapper>();

            var pareceristas = ObterTodos<PropostaParecerista>();
            
            var filtro = new NotificacaoPropostaPareceristasDTO(proposta.Id, mapper.Map<IEnumerable<PropostaPareceristaResumidoDTO>>(pareceristas));
            
            // act
            var mensagem = JsonSerializer.Serialize(filtro);
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit(mensagem));
            
            // assert 
            retorno.ShouldBeTrue();
            
            var notificacoes = ObterTodos<Dominio.Entidades.Notificacao>();
            notificacoes.Count().ShouldBe(1);
            var notificacao = notificacoes.FirstOrDefault();
            notificacao.Mensagem.ShouldNotBeEmpty();
            notificacao.Titulo.ShouldNotBeEmpty();
            notificacao.Categoria.ShouldBe(NotificacaoCategoria.Aviso);
            notificacao.Tipo.ShouldBe(NotificacaoTipo.Proposta);
            notificacao.Parametros.ShouldNotBeEmpty();
            
            var notificacoesUsuarios = ObterTodos<NotificacaoUsuario>();
            notificacoesUsuarios.Count().ShouldBe(3);
            notificacoesUsuarios.Any(a=> a.Login.Equals("1") && a.Email.Equals("parecerista1@email.com") && a.Situacao.EhNaoLida() && a.NotificacaoId == 1).ShouldBeTrue();
            notificacoesUsuarios.Any(a=> a.Login.Equals("2") && a.Email.Equals("parecerista2@email.com") && a.Situacao.EhNaoLida() && a.NotificacaoId == 1).ShouldBeTrue();
            notificacoesUsuarios.Any(a=> a.Login.Equals("3") && a.Email.Equals("parecerista3@email.com") && a.Situacao.EhNaoLida() && a.NotificacaoId == 1).ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Notificacao - Não deve notificar os pareceristas quando estiver na situação aguardando análise pelo DF")]
        public async Task Nao_deve_notificar_os_pareceristas_quando_estiver_na_situacao_aguardando_analise_pelo_df()
        {
            // arrange
            await InserirParametrosProposta();
            await InserirNaBase(ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.UrlConectaFormacao, "http://conecta"));

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
            var mapper = ObterCasoDeUso<IMapper>();

            var filtro = new NotificacaoPropostaPareceristasDTO(proposta.Id, null);
            
            // act
            var mensagem = JsonSerializer.Serialize(filtro);
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit(mensagem));
            
            // assert 
            retorno.ShouldBeFalse();
            
            var notificacoes = ObterTodos<Dominio.Entidades.Notificacao>();
            notificacoes.Count().ShouldBe(0);
            
            var notificacoesUsuarios = ObterTodos<NotificacaoUsuario>();
            notificacoesUsuarios.Count().ShouldBe(0);
        }
    }
}
