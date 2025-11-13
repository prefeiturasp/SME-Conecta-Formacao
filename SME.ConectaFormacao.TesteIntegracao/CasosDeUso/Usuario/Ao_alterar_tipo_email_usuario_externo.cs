using AutoMapper;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario
{
    public class Ao_alterar_tipo_email_usuario_externo : TesteBase
    {
        public Ao_alterar_tipo_email_usuario_externo(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
            UsuarioAlterarNomeMock.Montar();
        }

        [Fact(DisplayName = "Usuário - Deve retornar exceção ao alterar com tipo e-mail inválido")]
        public async Task Deve_retornar_excecao_ao_alterar_com_nome_nao_preenchido()
        {
            // arrange
            var login = UsuarioAlterarNomeMock.Login;
            var nome = UsuarioAlterarNomeMock.NomeInvalido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarTipoEmail>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(login, 0));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.Contains("Tipo de e-mail inválido");
        }

        [Fact(DisplayName = "Usuário - Deve alterar o tipo email e re-gerar o e-mail educacional do usuário")]
        public async Task Deve_alterar_o_tipo_email_do_usuario()
        {
            // arrange
            var mapper = ObterCasoDeUso<IMapper>();
            var usuarioMock = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            await InserirNaBase(mapper.Map<Dominio.Entidades.Usuario>(usuarioMock));
                        
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarTipoEmail>();

            // act
            var retorno = await casoDeUso.Executar(usuarioMock.Login!, (int)TipoEmail.Estagiario);

            // assert
            retorno.ShouldBeTrue();

            var usuarioAlterado = ObterTodos<Dominio.Entidades.Usuario>();
            usuarioAlterado.ShouldNotBeNull();

            //var usuarioLogin = usuarioAlterado.FirstOrDefault(f => f.Login.Equals(usuarioMock.Login));
            //usuarioLogin!.Login.ShouldBe(usuarioMock.Login);
            //((int)usuarioLogin.TipoEmail!).ShouldBe((int)TipoEmail.Estagiario);
            //usuarioLogin!.EmailEducacional.ShouldBe($"{usuarioMock.Nome.Replace(" ", "").ToLower()}.e{usuarioMock.Cpf}@edu.sme.prefeitura.sp.gov.br"); 
            //usuarioLogin.Excluido.ShouldBeFalse();
        }
    }
}
