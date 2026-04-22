using System.Collections.Concurrent;
using System.Reflection;

namespace SME.ConectaFormacao.Infra.Dados.Templates
{
    public class TemplateService(Assembly? assembly = null) : ITemplateService
    {
        private readonly ConcurrentDictionary<string, string> _cacheTemplates = new();
        private readonly ConcurrentDictionary<string, byte[]> _cacheTemplatesBytes = new();
        private readonly ConcurrentDictionary<string, string> _cacheImagensBase64 = new();
        private readonly Assembly _assembly = assembly ?? Assembly.GetExecutingAssembly();

        public string ObterImagemBase64(string nomeArquivoImagem)
        {
            return _cacheImagensBase64.GetOrAdd(nomeArquivoImagem, CarregarImagemBase64);
        }

        public string ObterTemplate(string nomeArquivo)
        {
            return _cacheTemplates.GetOrAdd(nomeArquivo, CarregarTemplate);
        }
        public byte[] ObterTemplateBytes(string nomeArquivo)
        {
            return _cacheTemplatesBytes.GetOrAdd(nomeArquivo, CarregarTemplateBytes);
        }

        private string CarregarImagemBase64(string nomeArquivoImagem)
        {
            var resourcePath =
                _assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(nomeArquivoImagem, StringComparison.OrdinalIgnoreCase)) ??
                throw new FileNotFoundException("Imagem não encontrada", nomeArquivoImagem);
            using var stream = _assembly.GetManifestResourceStream(resourcePath);
            using var memoryStream = new MemoryStream();
            stream!.CopyTo(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private string CarregarTemplate(string nomeArquivo)
        {
            var resourcePath =
                _assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(nomeArquivo, StringComparison.OrdinalIgnoreCase)) ??
                throw new FileNotFoundException("Template não encontrado", nomeArquivo);
            using var stream = _assembly.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        }
        private byte[] CarregarTemplateBytes(string nomeArquivo)
        {
            var resourcePath =
                _assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(nomeArquivo, StringComparison.OrdinalIgnoreCase)) ??
                throw new FileNotFoundException("Template não encontrado", nomeArquivo);

            using var stream = _assembly.GetManifestResourceStream(resourcePath);
            using var memoryStream = new MemoryStream();
            stream!.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
