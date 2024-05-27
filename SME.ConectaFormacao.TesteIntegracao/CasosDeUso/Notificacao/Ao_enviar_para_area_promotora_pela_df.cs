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
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Funcionario;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao
{
    public class Ao_enviar_para_area_promotora_pela_df : TestePropostaBase
    {
        public Ao_enviar_para_area_promotora_pela_df(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuariosPorPerfisServicoEolQuery, IEnumerable<UsuarioPerfilServicoEol>>), typeof(ObterUsuariosPorPerfisServicoEolQueryFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Notificacao - Deve notificar a Área Promotora que foi enviado parecer pela DF")]
        public async Task Deve_notificar_a_area_promotora_que_foi_enviado_parecer_pela_df()
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

            await InserirUsuario("1", "Parecerista1","parecerista1@email.com");
            await InserirUsuario("2", "Parecerista2","parecerista2@email.com");
            await InserirUsuario("3", "Parecerista3","parecerista3@email.com");
            await InserirUsuario("4", "CriadorProposta","criador.proposta@email.com");

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.AnaliseParecerPelaAreaPromotora, criadoLogin:"4");

            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1", "Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2", "Parecerista2", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3", "Parecerista3", SituacaoParecerista.Enviada));

            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoConsideracao.FormacaoHomologada, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1, CampoConsideracao.TipoFormacao, "1"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2, CampoConsideracao.Formato, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2, CampoConsideracao.FormacaoHomologada, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2, CampoConsideracao.TipoFormacao, "2"));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3, CampoConsideracao.Formato, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3, CampoConsideracao.FormacaoHomologada, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3, CampoConsideracao.TipoFormacao, "3"));

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoNotificarAreaPromotoraParaAnaliseParecer>();

            // act
            var mensagem = JsonSerializer.Serialize(proposta.Id);
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit(mensagem));
            
            // assert 
            retorno.ShouldBeTrue();
            
            var notificacoes = ObterTodos<Dominio.Entidades.Notificacao>();
            notificacoes.Count().ShouldBe(1);
            var notificacao = notificacoes.FirstOrDefault();
            
            notificacao.Mensagem.ShouldBe(string.Format("A proposta {0} - {1} foi analisada pela Comissão de Análise. Acesse <a href=\"{2}\">Aqui</a> o cadastro da proposta e verifique os comentários.",
                proposta.Id, 
                proposta.NomeFormacao,
                "http://conecta"));
            
            notificacao.Titulo.ShouldBe(string.Format("Proposta {0} - {1} foi analisada pela Comissão de Análise",proposta.Id, proposta.NomeFormacao));
            notificacao.Categoria.ShouldBe(NotificacaoCategoria.Aviso);
            notificacao.Tipo.ShouldBe(NotificacaoTipo.Proposta);
            notificacao.Parametros.ShouldNotBeEmpty();
            
            var notificacoesUsuarios = ObterTodos<NotificacaoUsuario>();
            notificacoesUsuarios.Count().ShouldBe(2);

            var propostaInserida = (ObterTodos<Dominio.Entidades.Proposta>()).FirstOrDefault();
            var usuarioDaProposta = (ObterTodos<Dominio.Entidades.Usuario>()).FirstOrDefault(f=> f.Login.Equals(propostaInserida.CriadoLogin));
            notificacoesUsuarios.Count(a=> a.Situacao.EhNaoLida() && a.NotificacaoId == 1).ShouldBe(2);
            notificacoesUsuarios.Count(a=> a.Situacao.EhNaoLida() && a.NotificacaoId == 1 && a.Login.NaoEstaPreenchido() && a.Nome.Equals(areaPromotora.Nome) && a.Email.Equals(areaPromotora.Email)).ShouldBe(1);
            notificacoesUsuarios.Count(a=> a.Situacao.EhNaoLida() && a.NotificacaoId == 1 && a.Login.EstaPreenchido() && a.Login.Equals(usuarioDaProposta.Login) && a.Nome.Equals(usuarioDaProposta.Nome) && a.Email.Equals(usuarioDaProposta.Email)).ShouldBe(1);
        }
        
        [Fact(DisplayName = "Notificacao - Não deve notificar a Área Promotora que foi enviado parecer pela DF")]
        public async Task Nao_deve_notificar_a_area_promotora_que_foi_enviado_parecer_pela_df()
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
