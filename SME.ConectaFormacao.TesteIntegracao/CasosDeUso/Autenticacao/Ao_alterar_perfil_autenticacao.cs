using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao
{
    public class Ao_alterar_perfil_autenticacao : TesteBase
    {
        public Ao_alterar_perfil_autenticacao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            AutenticacaoMock.Montar();
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterPerfisUsuarioServicoAcessosPorLoginQuery, UsuarioPerfisRetornoDTO>), typeof(ObterPerfisUsuarioServicoAcessosPorLoginQueryHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>), typeof(VincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Autenticação - Deve retornar erro 401 com login não encontrado ao alterar perfil sem usuário autenticado")]
        public async Task Deve_retornar_erro_401_com_login_nao_encontrado_ao_alterar_perfil_sem_usuario_autenticado()
        {
            // arrange
            AutenticacaoMock.UsuarioLogado = null;
            var casoDeUsoAutenticarUsuario = ObterCasoDeUso<ICasoDeUsoAutenticarAlterarPerfil>();

            // act
            var exception = await Should.ThrowAsync<NegocioException>(casoDeUsoAutenticarUsuario.Executar(Guid.NewGuid()));

            // assert
            exception.ShouldNotBeNull();
            exception.StatusCode.ShouldBe(401);
            exception.Mensagens.Contains(MensagemNegocio.LOGIN_NAO_ENCONTRADO).ShouldBeTrue();
        }

        [Fact(DisplayName = "Autenticação - Deve retornar token valido ao alterar perfil do usuário autenticado")]
        public async Task Deve_retornar_token_valido_ao_alterar_perfil_do_usuario_autenticado()
        {
            // arrange
            var casoDeUsoAutenticarUsuario = ObterCasoDeUso<ICasoDeUsoAutenticarAlterarPerfil>();

            // act
            var retorno = await casoDeUsoAutenticarUsuario.Executar(Guid.NewGuid());

            // assert
            retorno.ShouldNotBeNull();
            retorno.UsuarioLogin.ShouldBe(AutenticacaoMock.UsuarioLogado.Login);
        }
    }
}
