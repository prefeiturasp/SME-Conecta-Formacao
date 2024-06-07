using System.Text.Json;
using System.Text.Json.Serialization;

namespace SME.ConectaFormacao.Dominio.Extensoes
{
    public static class JsonExtensao
    {
        private static JsonSerializerOptions ObterConfigSerializer()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };            
        }

        public static string ObjetoParaJson(this object objeto) => JsonSerializer.Serialize(objeto, ObterConfigSerializer());

        public static T JsonParaObjeto<T>(this string json)
        {
            var obj = JsonSerializer.Deserialize<T>(json, ObterConfigSerializer());
            if (obj == null)
                throw new InvalidOperationException("Não foi possível converter o Json.");

            return obj;
        }
    }
}
