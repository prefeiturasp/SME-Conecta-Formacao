using Microsoft.AspNetCore.Mvc;
using SME.Conecta.Worker;
using SME.ConectaFormacao.IoC;

var builder = WebApplication.CreateBuilder(args);
var registradorDeDependencia = new RegistradorDeDependencia(builder.Services, builder.Configuration);
registradorDeDependencia.Registrar();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddHostedService<WorkerRabbitMQ>();
builder.Services.AddSingleton(registradorDeDependencia);

var app = builder.Build();


app.Run();