using SpeechKitApi.Attributes;

namespace Audio.Utils
{
    public enum DefaultSoundType
    {
        Null,
        
        [EnumValueString("Доброе утро!")]
        GoodMorning,
        
        [EnumValueString("Добрый день!")]
        GoodAfternoon,
        
        [EnumValueString("Добрый вечер!")]
        GoodEvening,
        
        [EnumValueString("Доброй ночи!")]
        GoodNight,
        
        [EnumValueString("У вас запланировано событие")]
        ScheduledEvent,
        
        [EnumValueString("Событие добавлено")]
        EventAdded
    }
}