using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SME.ConectaFormacao.Dominio.Extensoes
{
    public static class JsonExtensao
    {
        private static readonly JsonSerializerOptions _opcoesJson = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            // Garante que caracteres especiais (acentos) não sejam escapados para Unicode (ex: \u00E3)
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static string ObjetoParaJson(this object? objeto)
        => objeto is null ? string.Empty : JsonSerializer.Serialize(objeto, _opcoesJson);

        public static T? JsonParaObjeto<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(json, _opcoesJson);
            }
            catch (JsonException)
            {
                return (typeof(T) == typeof(string)) ? (T)(object)json : default;
            }
        }
    }
}
