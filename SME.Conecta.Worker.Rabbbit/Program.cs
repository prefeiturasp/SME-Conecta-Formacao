//namespace SME.Conecta.Worker;

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        CreateHostBuilder(args).Build().Run();
//    }

//    public static IHostBuilder CreateHostBuilder(string[] args) =>
//        Host.CreateDefaultBuilder(args)
//            .ConfigureAppConfiguration((hostingContext, config) =>
//            {
//                config.AddEnvironmentVariables();
//                config.AddUserSecrets<Program>();
//            })
//            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
//            .ConfigureServices(services =>
//            {
//                services.AddHostedService<WorkerRabbitMQ>();
//                services.AddHealthChecks();
//            });
//}

using Microsoft.AspNetCore.Mvc;
using SME.Conecta.Worker;
using SME.Conecta.Worker.Rabbbit.Contexto;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.IoC;
using SME.ConectaFormacao.IoC.Extensions;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var registradorDeDependencia = new RegistradorDeDependencia(builder.Services, builder.Configuration);
registradorDeDependencia.Registrar();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IContextoAplicacao, ContextoHttp>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddHostedService<WorkerRabbitMQ>();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton(registradorDeDependencia);

var app = builder.Build();
//RegistrarConfigsThreads.Registrar(builder.Configuration);

app.Run();
//app.Run(async (context) => { await context.Response.WriteAsync("WorkerRabbit!"); });