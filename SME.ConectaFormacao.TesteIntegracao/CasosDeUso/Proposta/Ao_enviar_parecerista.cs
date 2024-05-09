using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
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
    public class Ao_enviar_parecerista : TestePropostaBase
    {
        public Ao_enviar_parecerista(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFakerEnviarParecerista), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Deve enviar o parecer do parecerista sem parecerista pendente alterando situação da proposta para AguardandoAnaliseParecerDF")]
        public async Task Ao_enviar_parecer_parecerista_alterando_proposta_aguardando_analise_parecer_df()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);
            PropostaEnviarPareceristaMock.UsuarioLogado = usuario;

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnalisePeloParecerista);

            var propostaParecerista = PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, usuario.Login, usuario.Nome);
            await InserirNaBase(propostaParecerista);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceristaBanco = ObterPorId<PropostaParecerista, long>(propostaParecerista.Id);
            pareceristaBanco.Situacao.ShouldBe(SituacaoParecerista.Enviada);

            var propostaBanco = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            propostaBanco.Situacao.ShouldBe(SituacaoProposta.AguardandoAnaliseParecerPelaDF);
        }

        [Fact(DisplayName = "Proposta - Deve enviar o parecer do parecerista com parecerista pendente sem alterar situação da proposta")]
        public async Task Ao_enviar_parecer_parecerista_sem_alterar_situacao_proposta()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);
            PropostaEnviarPareceristaMock.UsuarioLogado = usuario;

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnalisePeloParecerista);

            var propostaParecerista = PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, usuario.Login, usuario.Nome);
            await InserirNaBase(propostaParecerista);

            var usuario2 = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario2);
            var propostaParecerista2 = PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, usuario2.Login, usuario2.Nome);
            await InserirNaBase(propostaParecerista2);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParecerista>();

            // act 
            await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceristaBanco = ObterPorId<PropostaParecerista, long>(propostaParecerista.Id);
            pareceristaBanco.Situacao.ShouldBe(SituacaoParecerista.Enviada);

            var propostaBanco = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            propostaBanco.Situacao.ShouldBe(SituacaoProposta.AguardandoAnalisePeloParecerista);
        }
    }
}
