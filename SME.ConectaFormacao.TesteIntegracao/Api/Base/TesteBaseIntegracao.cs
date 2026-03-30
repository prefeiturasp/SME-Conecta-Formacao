using Microsoft.AspNetCore.Authentication.JwtBearer;
using Npgsql;
using Respawn;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using StackExchange.Redis;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.Api.Base
{
    [Collection("WebApi Conecta Teste Integracao")]
    public abstract class TesteBaseIntegracao(ConectaWebApplicationFactory factory) : IAsyncLifetime
    {
        protected readonly ConectaWebApplicationFactory Factory = factory;
        private Respawner? _respawner;
        protected HttpClient Client { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            await using var connection = new NpgsqlConnection(Factory.StringDeConexaoPostgres);
            await connection.OpenAsync();

            _respawner ??= await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
                TablesToIgnore = [
                        "flyway_schema_history",
                        "cargo_funcao",
                        "criterio_validacao_inscricao",
                        "roteiro_proposta_formativa",
                        "palavra_chave",
                        "criterio_certificacao",
                        "parametro_sistema",
                        "ano_turma",
                        "componente_curricular",
                        "cargo_funcao_depara_eol"
                    ]
            });

            await _respawner.ResetAsync(connection);
            //await AlimentarDadosPadraoAsync(connection);

            await LimparCacheRedisAsync();

            Client = Factory.CreateClient();
        }
        protected void Deslogar()
        {
            Client.DefaultRequestHeaders.Remove("x-test-Login");
            Client.DefaultRequestHeaders.Remove("x-test-Nome");
            Client.DefaultRequestHeaders.Remove("x-test-Sistema");
            Client.DefaultRequestHeaders.Remove("x-test-Perfil");
            Client.DefaultRequestHeaders.Remove("x-test-Perfis");
            Client.DefaultRequestHeaders.Remove("x-test-Dres");
            Client.DefaultRequestHeaders.Remove("x-test-Roles");
            Client.DefaultRequestHeaders.Authorization = null;
        }
        protected void Autenticar(string login, string nome, string sistema, string perfil, string[] perfis, string[] dres, Permissao[] permissoes)
        {
            Deslogar();

            var roles = permissoes.Select(p => (int)p).Distinct();
            var rolesString = string.Join(",", roles);

            Client.DefaultRequestHeaders.Add("x-test-Login", login);
            Client.DefaultRequestHeaders.Add("x-test-Nome", nome);
            Client.DefaultRequestHeaders.Add("x-test-Sistema", sistema);
            Client.DefaultRequestHeaders.Add("x-test-Perfil", perfil);
            Client.DefaultRequestHeaders.Add("x-test-Perfis", string.Join(",", perfis));
            Client.DefaultRequestHeaders.Add("x-test-Dres", string.Join(",", dres));
            Client.DefaultRequestHeaders.Add("x-test-Roles", rolesString);
            Client.DefaultRequestHeaders.Authorization = new(JwtBearerDefaults.AuthenticationScheme);
        }
        protected void AutenticarComoAdmin()
        {
            var todasPermissoes = Enum.GetValues<Permissao>()
                                         .Select(p => p)
                                         .ToArray();
            Autenticar(
                login: "admin",
                nome: "Administrador de Teste",
                sistema: "1007",
                perfil: "7eda4540-a16c-4fe5-8322-9f75b3414e27", // Admin DF
                perfis: ["7eda4540-a16c-4fe5-8322-9f75b3414e27"],
                dres: [],
                permissoes: todasPermissoes
            );
        }
        protected void AutenticarComoCursista(string? dreId = null)
        {
            Autenticar(
                login: "cursista",
                nome: "Cursista do sistema",
                sistema: "1007",
                perfil: "651914B6-C4B6-4463-B773-B0960F4A148B", // Cursista
                perfis: ["651914B6-C4B6-4463-B773-B0960F4A148B"],
                dres: dreId is null ? [] : [dreId],
                permissoes: []
            );
        }
        protected void AutenticarComoAreaPromotora(string? dreId = null)
        {
            Autenticar(
                login: "emforpef",
                nome: "Area promotora do sistema",
                sistema: "1007",
                perfil: "2358698A-D07B-471C-A76B-0AC8324C2FEE", // EMFORPEF (Area Promotora)
                perfis: ["2358698A-D07B-471C-A76B-0AC8324C2FEE"],
                dres: dreId is null ? [] : [dreId],
                permissoes: [Permissao.Proposta_I, Permissao.Proposta_C, Permissao.Proposta_E, Permissao.Proposta_A,
                             Permissao.Inscricao_A, Permissao.Inscricao_C, Permissao.Inscricao_I, Permissao.Inscricao_E]
            );
        }
        private static async Task AlimentarDadosPadraoAsync(NpgsqlConnection connection)
        {
            // Usamos Raw String Literals (C# 11+) para escrever blocos SQL limpos.
            // Aqui você cria os seus "Coringas". Todo teste seu saberá que a "DRE 1" e a "UE 1" sempre existem.
            var sqlSincronizacao = """
                -- Inserindo DRE Padrão
                INSERT INTO public.dre (id, codigo_dre, abreviacao, nome) 
                VALUES (1, '108800', 'DRE CS', 'Diretoria Regional de Educação Capela do Socorro')
                ON CONFLICT DO NOTHING; -- Evita erros se o teste não sujou a DRE

                -- Inserindo UE (Escola) Padrão vinculada à DRE
                INSERT INTO public.ue (id, dre_id, codigo_ue, tipo_escola, nome) 
                VALUES (1, 1, '123456', 1, 'EMEF TESTE DE INTEGRACAO')
                ON CONFLICT DO NOTHING;

                -- Inserindo Cargos e Atribuições EOL
                INSERT INTO public.cargos_eol (id, codigo_cargo, descricao) 
                VALUES (1, '4321', 'PROFESSOR DE ENSINO FUNDAMENTAL II E MEDIO')
                ON CONFLICT DO NOTHING;

                -- Você pode continuar adicionando dados essenciais de negócio aqui...
                """;

            using var cmd = new NpgsqlCommand(sqlSincronizacao, connection);
            await cmd.ExecuteNonQueryAsync();
        }
        private async Task LimparCacheRedisAsync()
        {
            var connectionStringComAdmin = $"{Factory.StringDeConexaoRedis},allowAdmin=true";
            var redisConnection = await ConnectionMultiplexer.ConnectAsync(connectionStringComAdmin);
            var endpoints = redisConnection.GetEndPoints();

            if (endpoints.Length > 0)
            {
                var server = redisConnection.GetServer(endpoints[0]);
                await server.FlushAllDatabasesAsync();
            }

            await redisConnection.DisposeAsync();
        }
        public Task DisposeAsync() => Task.CompletedTask;
    }
}
