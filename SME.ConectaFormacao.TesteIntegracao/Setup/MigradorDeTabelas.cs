using DbUp;
using DbUp.Engine;
using StackExchange.Redis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.TesteIntegracao.Setup
{
    public static partial class MigradorDeTabelas
    {
        public static void Migrar(string connectionString)
        {
            var scriptsOrdenados = ObterScriptsOrdenados();

            var upgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScripts(scriptsOrdenados)
                .LogToConsole()
                .WithVariablesDisabled()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
                throw new Exception($"Erro ao executar os scripts de migração: {result.Error}");
        }

        private static IEnumerable<SqlScript> ObterScriptsOrdenados()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var recursos = assembly.GetManifestResourceNames()
                                   .Where(r => r.EndsWith(".sql"));

            var scripts = new List<SqlScript>();

            foreach (var recurso in recursos)
            {
                using var stream = assembly.GetManifestResourceStream(recurso);
                using var reader = new StreamReader(stream!);
                var nomeFormatado = FormatarNomeArquivo(recurso);

                scripts.Add(new SqlScript(nomeFormatado, reader.ReadToEnd()));
            }
            return scripts;
        }

        [GeneratedRegex(@"V(\d+)__")]
        private static partial Regex VersaoFlywayRegex();

        private static string FormatarNomeArquivo(string nomeOriginal)
        {
            var match = VersaoFlywayRegex().Match(nomeOriginal);

            if (!match.Success)
                return nomeOriginal;

            var numeroString = match.Groups[1].Value;
            var versao = int.Parse(numeroString);
            return nomeOriginal.Replace($"V{numeroString}__", $"V{versao:D5}__");
        }
    }
}
