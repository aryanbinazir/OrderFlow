using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.Helper.Enum
{
    public static class EnumFormatting
    {
        public static string GetEnumDescription(this System.Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetFormattedDescription(this System.Enum value, params object[] args)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            var description = attribute?.Description ?? value.ToString();
            return string.Format(description, args);
        }
    }

}
