using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Usuario.ServicosFakes;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario
{
    public class Ao_alterar_email_do_usuario : TesteBase
    {
        public Ao_alterar_email_do_usuario(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            UsuarioAlterarEmailMock.Montar();
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<AlterarEmailServicoAcessosCommand, bool>), typeof(AlterarEmailServicoAcessosCommandHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Usuário - Deve retornar excessão ao alterar com email inválido")]
        public async Task Deve_retornar_excessao_ao_alterar_com_email_invalido()
        {
            // arrange
            var login = UsuarioAlterarEmailMock.Login;
            var emailInvalido = UsuarioAlterarEmailMock.EmailInvalido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarEmail>();

            // act
            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(login, emailInvalido));

            // assert
            excessao.ShouldNotBeNull();
            excessao.StatusCode.ShouldBe(400);
            excessao.Mensagens.Contains("Email inválido");
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
