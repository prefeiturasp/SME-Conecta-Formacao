using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.TesteIntegracao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace SME.ConectaFormacao.TesteIntegracao
{
    [Collection("TesteIntegradoConectaFormacao")]
    public class TesteBase : IClassFixture<TestFixture>
    {
        protected readonly CollectionFixture _collectionFixture;

        public ServiceProvider ServiceProvider => _collectionFixture.ServiceProvider;

        public TesteBase(CollectionFixture collectionFixture, bool limparBanco = true)
        {
            _collectionFixture = collectionFixture;

            if (limparBanco)
                _collectionFixture.Database.LimparBase();

            _collectionFixture.IniciarServicos();
            RegistrarFakes(_collectionFixture.Services);
            _collectionFixture.BuildServiceProvider();
        }

        protected virtual void RegistrarFakes(IServiceCollection services)
        {
            RegistrarCommandFakes(services);
            RegistrarQueryFakes(services);
        }

        protected virtual void RegistrarCommandFakes(IServiceCollection services)
        {
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<PublicarNaFilaRabbitCommand, bool>), typeof(PublicarNaFilaRabbitCommandFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<AlterarEmailServicoAcessosCommand, bool>), typeof(AlterarEmailServicoAcessosCommandHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<AlterarNomeServicoAcessosCommand, bool>), typeof(AlterarNomeServicoAcessosCommandHandlerFake), ServiceLifetime.Scoped));
        }

        protected virtual void RegistrarQueryFakes(IServiceCollection services)
        {
        }

        public Task InserirNaBase<T>(IEnumerable<T> objetos) where T : EntidadeBase, new()
        {
            _collectionFixture.Database.Inserir(objetos);
            return Task.CompletedTask;
        }

        public Task InserirNaBase<T>(T objeto) where T : EntidadeBase, new()
        {
            _collectionFixture.Database.Inserir(objeto);
            return Task.CompletedTask;
        }

        public Task AtualizarNaBase<T>(T objeto) where T : class, new()
        {
            _collectionFixture.Database.Atualizar(objeto);
            return Task.CompletedTask;
        }

        public Task InserirNaBase(string nomeTabela, params string[] campos)
        {
            _collectionFixture.Database.Inserir(nomeTabela, campos);
            return Task.CompletedTask;
        }

        public Task InserirNaBase(string nomeTabela, string[] campos, string[] valores)
        {
            _collectionFixture.Database.Inserir(nomeTabela, campos, valores);
            return Task.CompletedTask;
        }

        public List<T> ObterTodos<T>() where T : class, new()
        {
            return _collectionFixture.Database.ObterTodos<T>();
        }

        public T ObterPorId<T, K>(K id)
            where T : class, new()
            where K : struct
        {
            return _collectionFixture.Database.ObterPorId<T, K>(id);
        }

        public T ObterCasoDeUso<T>()
        {
            return this.ServiceProvider.GetService<T>() ?? throw new Exception($"Caso de Uso {typeof(T).Name} não registrado!");
        }
        
        // protected void CriarClaimUsuario(string perfil, string login, string nomeUsuario,
        //     string numeroPagina = "0", string numeroRegistros = "10", string ordenacao = "1")
        // {
        //     var contextoAplicacao = ServiceProvider.GetService<IContextoAplicacao>();
        //     
        //     contextoAplicacao.AdicionarVariaveis(ObterVariaveisPorPerfil(login, nomeUsuario, perfil,numeroPagina, numeroRegistros, ordenacao));
        // }
        //
        // private Dictionary<string, object> ObterVariaveisPorPerfil(string login = ConstantesTestes.LOGIN_123456789, 
        //     string nomeUsuario = ConstantesTestes.SISTEMA, string perfil = Dominio.Constantes.Constantes.PERFIL_ADMIN_GERAL_GUID,
        //     string numeroPagina = "0", string numeroRegistros = "10", string ordenacao = "1")
        // {
        //     return new Dictionary<string, object>
        //     {
        //         { ConstantesTestes.USUARIO_CHAVE,  nomeUsuario},
        //         { ConstantesTestes.USUARIO_LOGADO_CHAVE, login },
        //         { ConstantesTestes.PERFIL_USUARIO, perfil },
        //         { ConstantesTestes.NUMERO_PAGINA, numeroPagina },
        //         { ConstantesTestes.NUMERO_REGISTROS, numeroRegistros },
        //         { ConstantesTestes.ORDENACAO, ordenacao },
        //         {
        //             ConstantesTestes.USUARIO_CLAIMS_CHAVE,new Tuple<string, string>(login, ConstantesTestes.USUARIO_CLAIM_TIPO_RF)
        //         }
        //     };
        // }
    }
}
