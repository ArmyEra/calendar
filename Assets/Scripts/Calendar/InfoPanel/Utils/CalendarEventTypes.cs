using SpeechKitApi.Attributes;

namespace Calendar.InfoPanel.Utils
{
    public enum CalendarEventTypes
    {
        Null,
        
        [EnumValueString("Дни Воинской Славы")]
        MilitaryMemoryDay,
        
        [EnumValueString("Этот день в науке и технике")]
        ScienceDay,
        
        [EnumValueString("Планировщик")]
        Notes
    }
}