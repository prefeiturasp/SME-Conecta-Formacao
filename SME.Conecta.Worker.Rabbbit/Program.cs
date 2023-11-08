namespace SME.Conecta.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddEnvironmentVariables();
                config.AddUserSecrets<Program>();
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .ConfigureServices(services =>
            {
                services.AddHostedService<WorkerRabbitMQ>();
                services.AddHealthChecks();
            });
}

//using Microsoft.AspNetCore.Mvc;
//using SME.Conecta.Worker;
//using SME.ConectaFormacao.IoC;

//var builder = WebApplication.CreateBuilder(args);
//var registradorDeDependencia = new RegistradorDeDependencia(builder.Services, builder.Configuration);
//registradorDeDependencia.RegistarParaWorkers();

//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//});
//builder.Services.AddHostedService<WorkerRabbitMQ>();
//builder.Services.AddSingleton(registradorDeDependencia);

//var app = builder.Build();


//app.Run();