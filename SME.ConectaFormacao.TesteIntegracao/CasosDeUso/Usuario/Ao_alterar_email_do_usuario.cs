using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario
{
    public class Ao_alterar_email_do_usuario : TesteBase
    {
        public Ao_alterar_email_do_usuario(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
            UsuarioAlterarEmailMock.Montar();
        }

        [Fact(DisplayName = "Usuário - Deve retornar exceção ao alterar com email inválido")]
        public async Task Deve_retornar_excecao_ao_alterar_com_email_invalido()
        {
            // arrange
            var login = UsuarioAlterarEmailMock.Login;
            var emailInvalido = UsuarioAlterarEmailMock.EmailInvalido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarEmail>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(login, emailInvalido));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.Contains("Email inválido");
        }

        [Fact(DisplayName = "Usuário - Deve alterar o email do usuário")]
        public async Task Deve_alterar_o_email_do_usuario()
        {
            // arrange
            var login = UsuarioAlterarEmailMock.Login;
            var emailValido = UsuarioAlterarEmailMock.EmailValido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarEmail>();

            // act
            var retorno = await casoDeUso.Executar(login, emailValido);

            // assert
            retorno.ShouldBeTrue();
        }
    }
}
