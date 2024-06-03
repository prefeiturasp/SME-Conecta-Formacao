using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao
{
    public class Ao_obter_total_notificacao_nao_lido : TesteBase
    {
        public Ao_obter_total_notificacao_nao_lido(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Notificacao - Deve obter o total de notificações não lidas com sucesso")]
        public async Task Deve_obter_total_notificacoes_nao_lidas_com_sucesso()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterTotalNotificacaoMock.UsuarioLogado = usuario;

            var notificacoes = NotificacaoMock.GerarNotificacoes(10);
            await InserirNaBase(notificacoes);

            var notificacoesUsuarios = NotificacaoMock.GerarNotificacaoUsuarios(usuario.Login, usuario.Nome, notificacoes);
            await InserirNaBase(notificacoesUsuarios);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTotalNotificacaoNaoLida>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            var total = notificacoesUsuarios.Where(t => t.Situacao.EhNaoLida()).DistinctBy(d => d.NotificacaoId).Count();
            retorno.ShouldBe(total);
        }
    }
}
