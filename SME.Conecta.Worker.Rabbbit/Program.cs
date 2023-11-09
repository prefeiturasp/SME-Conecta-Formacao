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

using Elastic.Apm.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SME.Conecta.Worker;
using SME.Conecta.Worker.Rabbbit.Contexto;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Servicos.Options;
using SME.ConectaFormacao.IoC;
using SME.ConectaFormacao.IoC.Extensions;
using System;
using System.Text;
using System.Web;


var builder = WebApplication.CreateBuilder(args);
var registradorDeDependencia = new RegistradorDeDependencia(builder.Services, builder.Configuration);
registradorDeDependencia.Registrar();

var serviceProvider = builder.Services.BuildServiceProvider();
var options = serviceProvider.GetService<IOptions<ConfiguracaoRabbitOptions>>().Value;

builder.Services.AddSingleton<IConnectionFactory>(serviceProvider => 
{
    var factory = new ConnectionFactory
    {
        HostName = options.HostName,
        UserName = options.UserName,
        Password = options.Password,
        VirtualHost = options.VirtualHost,
        RequestedHeartbeat = System.TimeSpan.FromSeconds(options.TempoHeartBeat),
    };

    return factory;

});
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
RegistrarConfigsThreads.Registrar(builder.Configuration);

app.Run(async (context) => { await context.Response.WriteAsync("WorkerRabbit Conecta"); });
app.Run();

