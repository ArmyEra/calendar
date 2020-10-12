using System;
using Attributes;
using Audio.Utils;

namespace Extensions.AttributeExtensions
{
    /// <summary>
    /// Расширение аттрибута FlowChartState 
    /// </summary>
    public static class FlowChartStateExtensions
    {
        public static FlowChartStates GetState(this object value)
        {
            var objTypeInfo = value.CheckNullValue(out var objType);
            if (!(objTypeInfo.IsEnum || objTypeInfo.IsClass))
                throw new ArgumentNullException(nameof(objType));

            var fieldInfo = objType.GetField(value.ToString());
            var stateAttribute = (FlowChartStateAttribute) Attribute.GetCustomAttribute(fieldInfo, typeof(FlowChartStateAttribute));

            return stateAttribute?.State ?? FlowChartStates.Null;
        }
    }
}