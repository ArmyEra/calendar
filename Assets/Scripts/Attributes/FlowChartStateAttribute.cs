using System;
using Audio.FlowChart.Utils;
using Audio.Utils;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FlowChartStateAttribute: Attribute
    {
        public readonly AudioFlowChartStates State;

        public FlowChartStateAttribute(AudioFlowChartStates state)
        {
            State = state;
        }
    }
}