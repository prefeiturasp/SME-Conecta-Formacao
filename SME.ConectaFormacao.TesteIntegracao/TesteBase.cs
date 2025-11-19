using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Extensoes;
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
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterTotalRegistroFilaQuery, uint>), typeof(ObterTotalRegistroFilaQueryHandlerFaker), ServiceLifetime.Scoped));
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

        protected void CriarClaimUsuario(string perfil, string login = "1", string nomeUsuario = "Sistema", string numeroPagina = "0", string numeroRegistros = "10")
        {
            var contextoAplicacao = ServiceProvider.GetService<IContextoAplicacao>();

            contextoAplicacao.AdicionarVariaveis(ObterVariaveisPorPerfil(login, nomeUsuario, perfil, numeroPagina, numeroRegistros));
        }

        private Dictionary<string, object> ObterVariaveisPorPerfil(string login, string nomeUsuario, string perfil, string numeroPagina = "0", string numeroRegistros = "10")
        {
            return new Dictionary<string, object>
            {
                { "RF",  login},
                { "NomeUsuario",  nomeUsuario},
                { "UsuarioLogado", login },
                { "login", login },
                { "PerfilUsuario", perfil },
                { "NumeroPagina", numeroPagina },
                { "NumeroRegistros", numeroRegistros },
                {
                    "Claims",new Tuple<string, string>(login, "RF")
                }
            };
        }

        protected async Task InserirUsuario(string login = "1", string nome = "Sistema", string email = "")
        {
            await InserirNaBase(new Dominio.Entidades.Usuario()
            {
                Login = login,
                Nome = nome,
                Email = email,
                CriadoPor = nome,
                CriadoEm = DateTimeExtension.HorarioBrasilia(),
                CriadoLogin = login
            });
        }
    }
}
