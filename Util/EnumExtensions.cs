using System;
using System.ComponentModel;
using System.Reflection;

namespace SUSDK.Util
{
  public static class EnumExtensions
  {
    public static string GetValue(this Enum value)
    {
      var field = value.GetType().GetField(value.ToString());
      var attribute = field.GetCustomAttribute<DescriptionAttribute>();
      return attribute == null ? value.ToString() : attribute.Description;
    }
  }
}