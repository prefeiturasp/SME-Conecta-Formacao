using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao
{
    public class Ao_obter_notificacao : TesteBase
    {
        public Ao_obter_notificacao(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Notificacao - Deve obter notificação com sucesso")]
        public async Task Deve_obter_notificacao_com_sucesso()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterTotalNotificacaoMock.UsuarioLogado = usuario;

            var notificacao = NotificacaoMock.GerarNotificacao();
            await InserirNaBase(notificacao);

            var notificacaoUsuario = NotificacaoMock.GerarNotificacaoUsuario(usuario.Login, usuario.Nome, notificacao);
            await InserirNaBase(notificacaoUsuario);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNotificacao>();

            // act 
            var retorno = await casoDeUso.Executar(notificacao.Id);

            // assert 
            retorno.ShouldNotBeNull();
            retorno.Id.ShouldBe(notificacao.Id);
            retorno.Titulo.ShouldBe(notificacao.Titulo);
            retorno.Tipo.ShouldBe(notificacao.Tipo);
            retorno.Categoria.ShouldBe(notificacao.Categoria);

            var notificacaoUsuarioBanco = ObterPorId<NotificacaoUsuario, long>(notificacaoUsuario.Id);

            notificacaoUsuarioBanco.Situacao.ShouldBe(Dominio.Enumerados.NotificacaoUsuarioSituacao.Lida);
        }

        [Fact(DisplayName = "Notificacao - Deve retornar exceção ao obter notificação não encontrada")]
        public async Task Deve_retornar_excecao_obter_notificacao_nao_encontrada()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterTotalNotificacaoMock.UsuarioLogado = usuario;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNotificacao>();

            // act 
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(99));

            // assert 
            retorno.Mensagens.Contains(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);
        }

        [Fact(DisplayName = "Notificacao - Deve retornar exceção ao obter notificação excluida")]
        public async Task Deve_retornar_excecao_obter_notificacao_excluida_nao_encontrada()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterTotalNotificacaoMock.UsuarioLogado = usuario;

            var notificacao = NotificacaoMock.GerarNotificacao();
            notificacao.Excluido = true;
            await InserirNaBase(notificacao);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNotificacao>();

            // act 
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(notificacao.Id));

            // assert 
            retorno.Mensagens.Contains(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);
        }

        [Fact(DisplayName = "Notificacao - Deve retornar exceção ao obter notificação não encontrada para o usuário logado")]
        public async Task Deve_retornar_excecao_obter_notificacao_nao_encontrada_usuario_logado()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterTotalNotificacaoMock.UsuarioLogado = usuario;

            var notificacao = NotificacaoMock.GerarNotificacao();
            await InserirNaBase(notificacao);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNotificacao>();

            // act 
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(99));

            // assert 
            retorno.Mensagens.Contains(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA_USUARIO);
        }
    }
}
