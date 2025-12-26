using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace ProductHub.Application.Common.Extensions;

public static class EnumExtensions
{
    private static readonly
       ConcurrentDictionary<string, string> DisplayNameCache = new();

    public static string DisplayName(this Enum value)
    {
        var key = $"{value.GetType().FullName}.{value}";
        return DisplayNameCache.GetOrAdd(key, _ =>
        {
            var name = Enum.GetName(value.GetType(), value);
            if (name == null)
                return value.ToString();

            var member = value.GetType()
                              .GetMember(name)
                              .FirstOrDefault();
            if (member == null)
                return name;

            var attr = member.GetCustomAttribute<DescriptionAttribute>(inherit: false);
            return attr?.Description ?? name;
        });
    }
}
