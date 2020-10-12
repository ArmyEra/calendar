using System;
using Audio.Utils;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FlowChartStateAttribute: Attribute
    {
        public readonly FlowChartStates State;

        public FlowChartStateAttribute(FlowChartStates state)
        {
            State = state;
        }
    }
}