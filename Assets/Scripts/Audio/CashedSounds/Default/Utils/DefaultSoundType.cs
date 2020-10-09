using SpeechKitApi.Attributes;

namespace Audio.CashedSounds.Default.Utils
{
    public enum DefaultSoundType
    {
        [EnumValueString("Доброе утро!")]
        GoodMorning,
        
        [EnumValueString("Добрый день!")]
        GoodAfternoon,
        
        [EnumValueString("Добрый вечер!")]
        GoodEvening,
        
        [EnumValueString("Доброй ночи!")]
        GoodNight,
        
        [EnumValueString("Сегодня")]
        Today,
        
        [EnumValueString("В этот день")]
        ThisDay,
        
        [EnumValueString("У вас запланировано событие")]
        ScheduledEvent,
        
        [EnumValueString("Событие добавлено")]
        EventAdded,
        
        [EnumValueString("Празднуется")]
        HolidayProcess
    }
}