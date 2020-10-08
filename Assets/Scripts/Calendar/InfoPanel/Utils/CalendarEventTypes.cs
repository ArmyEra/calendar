using SpeechKitApi.Attributes;

namespace Calendar.InfoPanel.Utils
{
    public enum CalendarEventTypes
    {
        Null,
        
        [EnumValueString("Дни воиской славы и памятные даты")]
        MilitaryMemoryDay,
        
        [EnumValueString("Знаменательные научные события")]
        ScienceDay,
        
        [EnumValueString("Планировщик")]
        Notes
    }
}