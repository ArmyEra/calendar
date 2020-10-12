using System;
using System.Reflection;

namespace Extensions.AttributeExtensions
{
    /// <summary>
    /// Общие расширения для аттрибутов
    /// </summary>
    public static class AttributeExtensions
    {
        public static TypeInfo CheckNullValue(this object value, out Type objType)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));
            
            objType = value.GetType();
            return objType.GetTypeInfo();
        }
    }
}