using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario
{
    public class Ao_alterar_unidade_eol_do_usuario : TesteBase
    {
        public Ao_alterar_unidade_eol_do_usuario(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
            UsuarioAlterarUnidadeEolMock.Montar();
        }

        [Fact(DisplayName = "Usuário - Deve retornar exceção ao alterar unidade eol não preenchido")]
        public async Task Deve_retornar_excecao_ao_alterar_com_unidade_eol_nao_preenchido()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var unidadeEol = string.Empty;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarUnidadeEol>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(usuario.Login, unidadeEol));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.Contains("Informe o código da unidade do usuário para realizar a alteração da unidade eol");
        }

        [Fact(DisplayName = "Usuário - Deve retornar exceção ao alterar usuario não encontrado")]
        public async Task Deve_retornar_excecao_ao_alterar_unidade_eol_usuario_nao_encontrado()
        {
            // arrange
            var login = UsuarioAlterarUnidadeEolMock.Login;
            var unidadeEol = UsuarioAlterarUnidadeEolMock.UnidadeEol;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarUnidadeEol>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(login, unidadeEol));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.Contains(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
        }

        [Fact(DisplayName = "Usuário - Deve retornar exceção ao alterar unidade eol não preenchido")]
        public async Task Deve_alterar_unidade_eol_valida()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var unidadeEol = UsuarioAlterarUnidadeEolMock.UnidadeEol;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarUnidadeEol>();

            // act
            var retorno = await casoDeUso.Executar(usuario.Login, unidadeEol);

            // assert
            retorno.ShouldBeTrue();
        }
    }
}
