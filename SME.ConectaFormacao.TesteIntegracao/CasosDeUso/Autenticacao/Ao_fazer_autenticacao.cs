using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao
{
    public class Ao_fazer_autenticacao : TesteUsuarioBase
    {
        public Ao_fazer_autenticacao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            AutenticacaoMock.Montar();
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
            autenticacaoDto.Login = "9988776";
            // act
            var retorno = await casoDeUsoAutenticarUsuario.Executar(autenticacaoDto);

            // assert
            retorno.ShouldNotBeNull();
            retorno.UsuarioLogin.ShouldBe(autenticacaoDto.Login);

            var usuarios = ObterTodos<Dominio.Entidades.Usuario>();
            usuarios.Any().ShouldBeTrue();

            var usuario = usuarios.FirstOrDefault();
            usuario.Nome.ShouldBe(retorno.UsuarioNome);
            usuario.Login.ShouldBe(retorno.UsuarioLogin);
            usuario.EmailEducacional.ShouldBe(ObterEmailEdu(retorno));
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

        [Fact(DisplayName = "Autenticação - Deve retornar erro 401 com login ou senha inválida")]
        public async Task Deve_retornar_erro_401_com_login_nao_encontrado()
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
    }
}
