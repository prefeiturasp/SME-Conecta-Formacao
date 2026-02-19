using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

namespace SME.ConectaFormacao.IoC.Features
{
    public static class ComumExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AdicionarModuloComum() =>
                services
                .AddScoped<IRepositorioUsuario, RepositorioUsuario>()
                .AddScoped<IRepositorioUsuarioAcessibilidade, RepositorioUsuarioAcessibilidade>()
                .AddScoped<IUsuarioAcessibilidadeService, UsuarioAcessibilidadeService>();
        }
    }
}
