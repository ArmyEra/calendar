using SpeechKitApi.Attributes;

namespace Audio.Utils
{
    public enum DefaultSoundType
    {
        [EnumValueString("У вас запланировано событие")]
        ScheduledEvent,
        
        [EnumValueString("Событие добавлено")]
        EventAdded
    }
}