using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.ConectaFormacao.Aplicacao.Mapeamentos;
using SME.ConectaFormacao.Aplicacao.Servicos;
using SME.ConectaFormacao.Aplicacao.Servicos.Interface;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.IoC;
using SME.ConectaFormacao.TesteIntegracao.ServicosFakes;
using SME.ConectaFormacao.Webapi.Contexto;

namespace SME.ConectaFormacao.TesteIntegracao.Setup
{
    public class RegistradorDependencias : RegistradorDeDependencia
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IConfiguration _configuration;

        public RegistradorDependencias(IServiceCollection serviceCollection, IConfiguration configuration) : base(serviceCollection, configuration)
        {
            _serviceCollection = serviceCollection;
            _configuration = configuration;
        }

        public override void Registrar()
        {
            RegistrarTelemetria();
            RegistrarConexao();
            RegistrarRepositorios();
            RegistrarLogs();
            RegistrarPolly();
            RegistrarMapeamentos();
            RegistrarServicos();
            RegistrarProfiles();
            RegistrarContextos();
            RegistrarHttpClients();
        }

        protected void RegistrarContextos()
        {
            _serviceCollection.TryAddScoped<IHttpContextAccessor, HttpContextAccessorFake>();
            _serviceCollection.TryAddScoped<IContextoAplicacao, ContextoHttp>();
        }
        
        protected override void RegistrarProfiles()
        {
            _serviceCollection.AddAutoMapper(typeof(DominioParaDTOProfile));
        }
        
        protected override void RegistrarConexao()
        {
            _serviceCollection.AddScoped<IConectaFormacaoConexao, ConectaFormacaoConexao>();
            _serviceCollection.AddScoped<ITransacao, Transacao>();
        }
        protected override void RegistrarServicos()
        {
            _serviceCollection.TryAddScoped<IServicoAcessos, ServicoAcessosFake>();
            _serviceCollection.TryAddScoped<IServicoPerfilUsuario, ServicoPerfilUsuario>();
        }
        protected override void RegistrarHttpClients()
        {}

        protected virtual void RegistrarLogs()
        { }
    }
}
