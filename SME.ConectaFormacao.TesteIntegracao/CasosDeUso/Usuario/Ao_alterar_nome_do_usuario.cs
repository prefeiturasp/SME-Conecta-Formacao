using AutoMapper;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario
{
    public class Ao_alterar_nome_do_usuario : TesteBase
    {
        public Ao_alterar_nome_do_usuario(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
            UsuarioAlterarNomeMock.Montar();
        }

        [Fact(DisplayName = "Usuário - Deve retornar exceção ao alterar com nome não preenchido")]
        public async Task Deve_retornar_excecao_ao_alterar_com_nome_nao_preenchido()
        {
            // arrange
            var login = UsuarioAlterarNomeMock.Login;
            var nome = UsuarioAlterarNomeMock.NomeInvalido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarNome>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(login, nome));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.Contains("É necessário informar o novo nome para ser alterado do usuário");
        }

        [Fact(DisplayName = "Usuário - Deve alterar o nome do usuário")]
        public async Task Deve_alterar_o_nome_do_usuario()
        {
            // arrange
            var mapper = ObterCasoDeUso<IMapper>();
            var usuario = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            await InserirNaBase(mapper.Map<Dominio.Entidades.Usuario>(usuario));

            var login = usuario.Login;
            var nome = UsuarioAlterarNomeMock.NomeValido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarNome>();

            // act
            var retorno = await casoDeUso.Executar(login, nome);

            // assert
            retorno.ShouldBeTrue();

            var usuarioAlterado = ObterTodos<Dominio.Entidades.Usuario>();
            usuarioAlterado.ShouldNotBeNull();
            
            var usuarioLogin = usuarioAlterado.FirstOrDefault(f => f.Login.Equals(login));
            usuarioLogin.Login.ShouldBe(login);
            usuarioLogin.Nome.ShouldBe(nome.ToUpper());
            usuarioLogin.Excluido.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Usuário - Deve alterar o email edu com base no primeiro e último nome contendo espaços no final - bug 121342")]
        public async Task Deve_alterar_o_email_edu_com_base_no_primeiro_e_ultimo_nome_contendo_espacos_no_final_bug_121342()
        {
            // arrange
            var mapper = ObterCasoDeUso<IMapper>();
            var usuario = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuario.Cpf = "48682233720";
            await InserirNaBase(mapper.Map<Dominio.Entidades.Usuario>(usuario));

            var login = usuario.Login;
            var nome = "José Antônio Miguel ";

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarNome>();

            // act
            var retorno = await casoDeUso.Executar(login, nome);

            // assert
            retorno.ShouldBeTrue();

            var usuarioAlterado = ObterTodos<Dominio.Entidades.Usuario>();
            usuarioAlterado.ShouldNotBeNull();
            
            var usuarioLogin = usuarioAlterado.FirstOrDefault(f=> f.Login.Equals(login));
            usuarioLogin.Login.ShouldBe(login);
            usuarioLogin.Nome.ShouldBe(nome.Trim().ToUpper());
            usuarioLogin.Excluido.ShouldBeFalse();
            usuarioLogin.EmailEducacional.ShouldBe("josemiguel.48682233720@edu.sme.prefeitura.sp.gov.br");
        }
        
        [Fact(DisplayName = "Usuário - Deve alterar o email edu com base no primeiro ponto cpf contendo espaços no final - bug 121342")]
        public async Task Deve_alterar_o_email_edu_com_base_no_primeiro_nome__ponto_cpf_contendo_espacos_no_final_bug_121342()
        {
            // arrange
            var mapper = ObterCasoDeUso<IMapper>();
            var usuario = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuario.Cpf = "48682233720";
            await InserirNaBase(mapper.Map<Dominio.Entidades.Usuario>(usuario));

            var login = usuario.Login;
            var nome = "José  ";

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarNome>();

            // act
            var retorno = await casoDeUso.Executar(login, nome);

            // assert
            retorno.ShouldBeTrue();

            var usuarioAlterado = ObterTodos<Dominio.Entidades.Usuario>();
            usuarioAlterado.ShouldNotBeNull();
            
            var usuarioLogin = usuarioAlterado.FirstOrDefault(f=> f.Login.Equals(login));
            usuarioLogin.Login.ShouldBe(login);
            usuarioLogin.Nome.ShouldBe(nome.Trim().ToUpper());
            usuarioLogin.Excluido.ShouldBeFalse();
            usuarioLogin.EmailEducacional.ShouldBe("jose.48682233720@edu.sme.prefeitura.sp.gov.br");
        }
    }
}
