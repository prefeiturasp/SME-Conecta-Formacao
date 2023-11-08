using SME.ConectaFormacao.IoC;
using SME.ConectaFormacao.IoC.Extensions;

namespace SME.Conecta.Worker;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection serices)
    {
        var registrarDependencias = new RegistradorDeDependencia(serices, Configuration);
        registrarDependencias.RegistarParaWorkers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        RegistrarConfigsThreads.Registrar(Configuration);
        app.Run(async (context) => { await context.Response.WriteAsync("WorkerRabbit!"); });
    }
}