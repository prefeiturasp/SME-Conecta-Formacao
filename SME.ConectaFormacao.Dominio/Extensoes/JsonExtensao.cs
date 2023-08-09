using System.Text.Json;
using System.Text.Json.Serialization;

namespace SME.ConectaFormacao.Dominio.Extensoes
{
    public static class JsonExtensao
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string ObjetoParaJson(this object objeto) => JsonSerializer.Serialize(objeto, _jsonSerializerOptions);

        public static T JsonParaObjeto<T>(this string json) => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
    }
}
