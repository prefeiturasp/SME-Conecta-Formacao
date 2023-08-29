using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
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
    public class Ao_fazer_autenticacao : TesteBase
    {
        public Ao_fazer_autenticacao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            AutenticacaoMock.Montar();
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery, UsuarioAutenticacaoRetornoDTO>), typeof(ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterPerfisUsuarioServicoAcessosPorLoginQuery, UsuarioPerfisRetornoDTO>), typeof(ObterPerfisUsuarioServicoAcessosPorLoginQueryHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>), typeof(VincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Autenticação - Deve retornar erro 401 com login ou senha inválida")]
        public async Task Deve_retornar_erro_401_com_login_ou_senha_invalido()
        {
            // arrange
            var autenticacaoDto = AutenticacaoMock.AutenticacaoUsuarioDTOInvalido;
            var casoDeUsoAutenticarUsuario = ObterCasoDeUso<ICasoDeUsoAutenticarUsuario>();

            // act
            var exception = await Should.ThrowAsync<NegocioException>(casoDeUsoAutenticarUsuario.Executar(autenticacaoDto));

            // assert
            exception.ShouldNotBeNull();
            exception.StatusCode.ShouldBe(401);
        }

        [Fact(DisplayName = "Autenticação - Deve realizar autenticação com login e senha válida e salvar tabela usuario")]
        public async Task Deve_realizar_autenticacao_com_login_e_senha_valido()
        {
            // arrange
            var autenticacaoDto = AutenticacaoMock.AutenticacaoUsuarioDTOValido;
            var casoDeUsoAutenticarUsuario = ObterCasoDeUso<ICasoDeUsoAutenticarUsuario>();

            // act
            var retorno = await casoDeUsoAutenticarUsuario.Executar(autenticacaoDto);

            // assert
            retorno.ShouldNotBeNull();
            retorno.UsuarioLogin.ShouldBe(autenticacaoDto.Login);

            var usuarios = ObterTodos<Dominio.Usuario>();
            usuarios.Any().ShouldBeTrue();

            var usuario = usuarios.FirstOrDefault();
            usuario.Nome.ShouldNotBeNull(retorno.UsuarioNome);
            usuario.Login.ShouldNotBeNull(retorno.UsuarioLogin);
        }

        [Fact(DisplayName = "Autenticação - Deve atribuir o perfil Cursista ao autenticar com login e senha valido")]
        public async Task Deve_atribuir_perfil_cursista_ao_autenticar_com_login_e_senha_valido()
        {
            // arrange
            var autenticacaoDto = AutenticacaoMock.AutenticacaoUsuarioDTOValido;
            var casoDeUsoAutenticarUsuario = ObterCasoDeUso<ICasoDeUsoAutenticarUsuario>();

            // act
            var retorno = await casoDeUsoAutenticarUsuario.Executar(autenticacaoDto);

            // assert
            retorno.ShouldNotBeNull();
            retorno.UsuarioLogin.ShouldBe(autenticacaoDto.Login);

            retorno.PerfilUsuario.Any(t => t.Perfil == new Guid(PerfilAutomatico.PERFIL_CURSISTA_GUID)).ShouldBeTrue();
        }
    }
}
