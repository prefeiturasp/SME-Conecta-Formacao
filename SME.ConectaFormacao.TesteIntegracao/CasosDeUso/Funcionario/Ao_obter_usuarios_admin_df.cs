using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Funcionario
{
    public class Ao_obter_usuarios_admin_df : TesteBase
    {
        public Ao_obter_usuarios_admin_df(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        protected override void RegistrarFakes(IServiceCollection services)
        {
            base.RegistrarFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuariosPorPerfisServicoEolQuery, IEnumerable<UsuarioPerfilServicoEol>>), typeof(ObterUsuariosAdminDfQueryFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuariosAdminDfQuery, IEnumerable<RetornoUsuarioLoginNomeDTO>>), typeof(ObterUsuariosAdminDfQueryFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Funcionário - Deve retornar os funcionários Admin DF")]
        public async Task Deve_retornar_os_usuarios_admin_df()
        {
            // arrange
            var usuariosPerfis = UsuarioPerfilServicoEolMock.GerarListaUsuariosPerfis();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterUsuariosAdminDf>();

            // act
            var usuarios = await casoDeUso.Executar();

            // assert
            usuarios.Any().ShouldBeTrue();
            usuarios.Count().ShouldBe(usuariosPerfis.Count(c => c.Perfil == Perfis.ADMIN_DF));
        }
    }
}