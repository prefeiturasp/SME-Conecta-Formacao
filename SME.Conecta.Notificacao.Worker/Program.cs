using Elastic.Apm.AspNetCore;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.SqlClient;
using RabbitMQ.Client;
using SME.Conecta.Notificacao.Worker;
using SME.Conecta.Notificacao.Worker.Hub;
using SME.Conecta.Notificacao.Worker.Notificacao;
using SME.ConectaFormacao.Infra.Servicos.Options;
using SME.ConectaFormacao.Infra.Servicos.Telemetria.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigurarTelemetria(builder.Configuration);

var configuracaoRabbitOptions = new ConfiguracaoRabbitOptions();
builder.Configuration.GetSection(ConfiguracaoRabbitOptions.Secao).Bind(configuracaoRabbitOptions, c => c.BindNonPublicProperties = true);

builder.Services.AddSingleton<IConnectionFactory>(serviceProvider =>
{
    var factory = new ConnectionFactory
    {
        HostName = configuracaoRabbitOptions.HostName,
        UserName = configuracaoRabbitOptions.UserName,
        Password = configuracaoRabbitOptions.Password,
        VirtualHost = configuracaoRabbitOptions.VirtualHost,
        RequestedHeartbeat = TimeSpan.FromSeconds(configuracaoRabbitOptions.TempoHeartBeat),
    };

    return factory;
});
builder.Services.AddSingleton<INotificacao, Notificacao>();

builder.Services.AddHostedService<WorkerRabbitMQNotificacao>();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
        }));

builder.Services.AddSignalR();

var app = builder.Build();

app.UseElasticApm(builder.Configuration,
    new SqlClientDiagnosticSubscriber(),
    new HttpDiagnosticsSubscriber());


app.UseCors("CorsPolicy");

app.UseRouting();

app.MapHub<NotificacaoHub>("/notificacaoHub");

app.MapGet("/", () => "Worker de notificação online!");

app.Run();
