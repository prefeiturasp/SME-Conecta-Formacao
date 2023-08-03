using System.Configuration;
using Elastic.Apm.AspNetCore;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.SqlClient;
using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.IoC;
using SME.ConectaFormacao.Webapi.Configuracoes;
using SME.ConectaFormacao.Webapi.Contexto;
using SME.ConectaFormacao.Webapi.Controllers;
using SME.ConectaFormacao.Webapi.Filtros;
using SME.ConectaFormacao.Webapi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var registradorDeDependencia = new RegistradorDeDependencia(builder.Services, builder.Configuration);
registradorDeDependencia.Registrar();
RegistraDocumentacaoSwagger.Registrar(builder.Services);
RegistraAutenticacao.Registrar(builder.Services, builder.Configuration);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IContextoAplicacao, ContextoHttp>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = true;
    options.Filters.Add(new ValidaDtoAttribute());
});

builder.Services.AddSingleton(registradorDeDependencia);

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

app.UseCors(config => config
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials());

app.UseElasticApm(builder.Configuration,
    new SqlClientDiagnosticSubscriber(),
    new HttpDiagnosticsSubscriber());

app.UseTratamentoExcecoesGlobalMiddleware();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
