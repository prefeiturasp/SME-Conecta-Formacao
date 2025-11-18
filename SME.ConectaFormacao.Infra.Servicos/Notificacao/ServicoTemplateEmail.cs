using SME.ConectaFormacao.Dominio.Interfaces;
using System.Reflection;
using System.Text;

namespace SME.ConectaFormacao.Infra.Servicos.Notificacao
{
    public class ServicoTemplateEmail : IServicoTemplateEmail
    {
        private const string CAMINHO_RESOURCE_BASE = "SME.ConectaFormacao.Infra.Servicos.Notificacao.Templates";
        public async Task<string> ObterHtmlInscricaoConfirmadaAsync(string nomeCursista, string nomeFormacao) =>
            await ProcessarTemplatePadraoAsync("InscricaoConfirmada.html", new Dictionary<string, string>
            {
                { "{{NOME_CURSISTA}}", nomeCursista },
                { "{{NOME_FORMACAO}}", nomeFormacao }
            });

        public async Task<string> ObterHtmlInscricaoEmEsperaAsync(string nomeCursista, string nomeFormacao) =>
            await ProcessarTemplatePadraoAsync("InscricaoEmEspera.html", new Dictionary<string, string>
            {
                { "{{NOME_CURSISTA}}", nomeCursista },
                { "{{NOME_FORMACAO}}", nomeFormacao }
            });

        private static async Task<string> ProcessarTemplatePadraoAsync(string nomeArquivo, Dictionary<string, string> placeholders)
        {
            var template = await LerResourceIncorporadoAsync(nomeArquivo);
            foreach (var placeholder in placeholders)
            {
                template = template.Replace(placeholder.Key, placeholder.Value);
            }
            return template;
        }

        private static async Task<string> LerResourceIncorporadoAsync(string nomeArquivo)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var nomeCompletoRecurso = $"{CAMINHO_RESOURCE_BASE}.{nomeArquivo}";

            using var stream = assembly.GetManifestResourceStream(nomeCompletoRecurso);

            if (stream != null)
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            }

            throw new FileNotFoundException($"Template de e-mail não encontrado: {nomeCompletoRecurso}");
        }
    }
}
