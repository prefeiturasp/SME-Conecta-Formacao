using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
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
        
        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<CadastrarUsuarioServicoAcessoCommand, bool>), typeof(CadastrarUsuarioServicoAcessoCommandHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<UsuarioExisteNoCoreSsoQuery, bool>), typeof(UsuarioExisteNoCoreSsoQueryFake), ServiceLifetime.Scoped));
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
        
        [Fact(DisplayName = "Usuário - Deve retornar exceção ao alterar email @edu com email inválido")]
        public async Task Deve_retornar_excecao_ao_alterar_email_edu_com_email_invalido()
        {
            // arrange
            var login = UsuarioAlterarEmailMock.Login;
            var emailInvalido = UsuarioAlterarEmailMock.EmailInvalido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarEmailEducacional>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(login, emailInvalido));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.Contains(MensagemNegocio.EMAIL_EDU_INVALIDO);
        }
        
        [Fact(DisplayName = "Usuário - Deve alterar o email @edu do usuário")]
        public async Task Deve_alterar_o_email_edu_do_usuario()
        {
            // arrange
            var parametro = ParametroSistemaMock.GerarParametroSistema(Dominio.Enumerados.TipoParametroSistema.ConfirmarEmailUsuarioExterno, "false");
            await InserirNaBase(parametro);
            var emailValido = UsuarioAlterarEmailMock.EmailEducacional;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarEmailEducacional>();
            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            var casoDeUsoInserir = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            await casoDeUsoInserir.Executar(usuarioExterno);
            var retorno = await casoDeUso.Executar(usuarioExterno.Cpf, emailValido);

            // assert
            retorno.ShouldBeTrue();
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
