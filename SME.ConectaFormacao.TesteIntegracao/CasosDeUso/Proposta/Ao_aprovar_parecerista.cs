using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_aprovar_parecerista : TestePropostaBase
    {
        public Ao_aprovar_parecerista(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFakerEnviarParecerista), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Deve aprovar o parecer do parecerista sem parecerista pendente alterando situação da proposta para AguardandoAnaliseParecerDF")]
        public async Task Ao_aprovar_parecer_parecerista_alterando_proposta_aguardando_analise_parecer_df()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);
            PropostaEnviarPareceristaMock.UsuarioLogado = usuario;

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnaliseParecerista);

            var propostaParecerista = PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, usuario.Login, usuario.Nome);
            await InserirNaBase(propostaParecerista);

            var justificativa = PropostaJustificativaMock.GerarPropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAprovarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id, justificativa);

            // assert
            var pareceristaBanco = ObterPorId<PropostaParecerista, long>(propostaParecerista.Id);
            pareceristaBanco.Situacao.ShouldBe(SituacaoParecerista.Aprovada);

            var propostaBanco = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            propostaBanco.Situacao.ShouldBe(SituacaoProposta.AguardandoAnaliseParecerDF);
        }

        [Fact(DisplayName = "Proposta - Deve aprovr o parecer do parecerista com parecerista pendente sem alterar situação da proposta")]
        public async Task Ao_aprovar_parecer_parecerista_sem_alterar_situacao_proposta()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);
            PropostaEnviarPareceristaMock.UsuarioLogado = usuario;

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnaliseParecerista);

            var propostaParecerista = PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, usuario.Login, usuario.Nome);
            await InserirNaBase(propostaParecerista);

            var usuario2 = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario2);
            var propostaParecerista2 = PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, usuario2.Login, usuario2.Nome);
            await InserirNaBase(propostaParecerista2);

            var justificativa = new PropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAprovarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id, justificativa);

            // assert
            var pareceristaBanco = ObterPorId<PropostaParecerista, long>(propostaParecerista.Id);
            pareceristaBanco.Situacao.ShouldBe(SituacaoParecerista.Aprovada);

            var propostaBanco = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            propostaBanco.Situacao.ShouldBe(SituacaoProposta.AguardandoAnaliseParecerista);
        }

        [Fact(DisplayName = "Proposta - Deve aprovar o parecer do parecerista reanalise sem parecerista pendente alterando situação da proposta para AguardandoAnaliseParecerFinalDF")]
        public async Task Ao_aprovar_parecer_parecerista_alterando_proposta_aguardando_analise_parecer_final_df()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);
            PropostaEnviarPareceristaMock.UsuarioLogado = usuario;

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoReanaliseParecerista);

            var propostaParecerista = PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, usuario.Login, usuario.Nome, SituacaoParecerista.Enviada);
            await InserirNaBase(propostaParecerista);

            var justificativa = new PropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAprovarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id, justificativa);

            // assert
            var pareceristaBanco = ObterPorId<PropostaParecerista, long>(propostaParecerista.Id);
            pareceristaBanco.Situacao.ShouldBe(SituacaoParecerista.Aprovada);

            var propostaBanco = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            propostaBanco.Situacao.ShouldBe(SituacaoProposta.AguardandoAnaliseParecerFinalPelaDF);
        }

        [Fact(DisplayName = "Proposta - Deve aprovar o parecer do parecerista reanalise com parecerista pendente sem alterar situação da proposta")]
        public async Task Ao_aprovar_parecer_parecerista_reanalise_sem_alterar_situacao_proposta()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);
            PropostaEnviarPareceristaMock.UsuarioLogado = usuario;

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoReanaliseParecerista);

            var propostaParecerista = PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, usuario.Login, usuario.Nome, SituacaoParecerista.Enviada);
            await InserirNaBase(propostaParecerista);

            var usuario2 = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario2);
            var propostaParecerista2 = PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, usuario2.Login, usuario2.Nome, SituacaoParecerista.Enviada);
            await InserirNaBase(propostaParecerista2);

            var justificativa = new PropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAprovarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id, justificativa);

            // assert
            var pareceristaBanco = ObterPorId<PropostaParecerista, long>(propostaParecerista.Id);
            pareceristaBanco.Situacao.ShouldBe(SituacaoParecerista.Aprovada);

            var propostaBanco = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            propostaBanco.Situacao.ShouldBe(SituacaoProposta.AguardandoReanaliseParecerista);
        }
    }
}
