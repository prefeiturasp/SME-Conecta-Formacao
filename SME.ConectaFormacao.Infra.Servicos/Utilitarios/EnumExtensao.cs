using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SME.ConectaFormacao.Infra
{
    public static class EnumExtensao
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static string Name(this Enum enumValue)
        {
            return Enum.IsDefined(enumValue.GetType(), enumValue) ? enumValue.GetAttribute<DisplayAttribute>().Name : enumValue.ToString();
        }
    }
}
