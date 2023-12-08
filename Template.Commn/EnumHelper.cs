using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Template.Commn
{
    public class EnumHelper
    {
        public static string GetDescription(Type type, object obj)
        {
            //如果不是枚举类型,则返回空
            if (type.BaseType.FullName != "System.Enum")
            {
                return string.Empty;
            }
            //如果值为空则返回空
            if (obj == null || obj == DBNull.Value)
            {
                return string.Empty;
            }
            string strObj = obj.ToString();
            if (strObj.Length == 0)
            {
                return string.Empty;
            }
            int intObj = 0;
            if (!int.TryParse(strObj, out intObj))
            {
                return strObj;
            }
            string name = Enum.ToObject(type, intObj).ToString();
            FieldInfo field = type.GetField(name);
            if (field == null) return null; //add by kord
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute == null)
            {
                return name;
            }
            return attribute == null ? null : attribute.Description;
        }
    }
}