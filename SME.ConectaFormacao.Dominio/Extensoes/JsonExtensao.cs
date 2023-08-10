using System.Text.Json;
using System.Text.Json.Serialization;

namespace SME.ConectaFormacao.Dominio.Extensoes
{
    public static class JsonExtensao
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptionsPadrao = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string ObjetoParaJson(this object objeto) => JsonSerializer.Serialize(objeto, _jsonSerializerOptionsPadrao);

        public static T JsonParaObjeto<T>(this string json) => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptionsPadrao);
    }
}
