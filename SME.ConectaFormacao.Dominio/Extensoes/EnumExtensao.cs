using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SME.ConectaFormacao.Dominio.Extensoes
{
    public static class EnumExtensao
    {
        private static TAttribute ObterAtributos<TAttribute>(this Enum enumValue)
               where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static string Nome(this Enum enumValue)
         => enumValue.ObterAtributos<DisplayAttribute>().Name;

        public static string NomeCurto(this Enum enumValue)
            => enumValue.ObterAtributos<DisplayAttribute>().ShortName;


        public static string Descricao(this Enum enumValue)
           => enumValue.ObterAtributos<DisplayAttribute>().Description;

        public static string NomeGrupo(this Enum enumValue)
           => enumValue.ObterAtributos<DisplayAttribute>().GroupName;
    }
}
