using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Usuario.ServicosFakes;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario
{
    public class Ao_obter_dados_do_usuario : TesteBase
    {
        public Ao_obter_dados_do_usuario(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            UsuarioMeusDadosMock.Montar();
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>), typeof(ObterMeusDadosServicoAcessosPorLoginQueryHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Usuário - Deve retornar os dados do usuário existente")]
        public async Task Deve_retornar_dados_usuario_existente()
        {
            // arrange
            var login = UsuarioMeusDadosMock.Login;
            var casoDeUsoUsuarioMeusDados = ObterCasoDeUso<ICasoDeUsoUsuarioMeusDados>();

            // act
            var retorno = await casoDeUsoUsuarioMeusDados.Executar(login);

            // assert
            retorno.ShouldNotBeNull();
            retorno.Login.ShouldBe(login);
        }

        [Fact(DisplayName = "Usuário - Deve retornar os dados nulo para usuário não encontrado")]
        public async Task Deve_retornar_dados_nulo_para_usuario_nao_encontrado()
        {
            // arrange
            var login = UsuarioMeusDadosMock.LoginNaoEncontrado;
            var casoDeUsoUsuarioMeusDados = ObterCasoDeUso<ICasoDeUsoUsuarioMeusDados>();

            // act
            var retorno = await casoDeUsoUsuarioMeusDados.Executar(login);

            // assert
            retorno.ShouldNotBeNull();
            retorno.Login.ShouldBeNull();
        }
    }
}
