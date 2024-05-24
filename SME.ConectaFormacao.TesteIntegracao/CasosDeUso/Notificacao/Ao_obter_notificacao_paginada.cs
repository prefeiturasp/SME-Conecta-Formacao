using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao
{
    public class Ao_obter_notificacao_paginada : TesteBase
    {
        public Ao_obter_notificacao_paginada(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Notificacao - Deve obter notificações com filtro com sucesso")]
        public async Task Deve_obter_notificacoes_com_filtro_com_sucesso()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterTotalNotificacaoMock.UsuarioLogado = usuario;

            var notificacoes = NotificacaoMock.GerarNotificacoes(10);
            await InserirNaBase(notificacoes);

            var notificacoesUsuarios = NotificacaoMock.GerarNotificacaoUsuarios(usuario.Login, notificacoes);
            await InserirNaBase(notificacoesUsuarios);

            var notificacaoFiltro = notificacoes.FirstOrDefault();

            var filtro = new NotificacaoFiltroDTO
            {
                Id = notificacaoFiltro.Id,
                Titulo = notificacaoFiltro.Titulo,
                Categoria = notificacaoFiltro.Categoria,
                Tipo = notificacaoFiltro.Tipo,
                Situacao = notificacoesUsuarios.FirstOrDefault(t => t.NotificacaoId == notificacaoFiltro.Id).Situacao
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNotificacaoPaginada>();

            // act 
            var retorno = await casoDeUso.Executar(filtro);

            // assert 
            retorno.TotalRegistros.ShouldBe(1);
        }

        [Fact(DisplayName = "Notificacao - Deve obter notificações sem filtro com sucesso")]
        public async Task Deve_obter_notificacoes_com_filtro_sem_sucesso()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterTotalNotificacaoMock.UsuarioLogado = usuario;

            var notificacoes = NotificacaoMock.GerarNotificacoes(10);
            await InserirNaBase(notificacoes);

            var notificacoesUsuarios = NotificacaoMock.GerarNotificacaoUsuarios(usuario.Login, notificacoes);
            await InserirNaBase(notificacoesUsuarios);

            var filtro = new NotificacaoFiltroDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNotificacaoPaginada>();

            // act 
            var retorno = await casoDeUso.Executar(filtro);

            // assert 
            retorno.TotalRegistros.ShouldBe(10);
        }
    }
}
