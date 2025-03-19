using System;
using System.ComponentModel;

public enum CharacterEnum
{
    [Description("")]
    None,
    Lan,
    Mateo,
    Ravi
}

public static class EnumExtensions
{
    public static string GetParsedName(this Enum value)
    {
        var attribute = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
    }
}