using Attributes;
using Audio.Utils;
using SpeechKitApi.Attributes;

namespace Audio.CashedSounds.Default.Utils
{
    public enum DefaultSoundType
    {
        [EnumValueString("Доброе утро!"), FlowChartState(FlowChartStates.Greeting)]
        GoodMorning,
        
        [EnumValueString("Добрый день!"), FlowChartState(FlowChartStates.Greeting)]
        GoodAfternoon,
        
        [EnumValueString("Добрый вечер!"), FlowChartState(FlowChartStates.Greeting)]
        GoodEvening,
        
        [EnumValueString("Доброй ночи!"), FlowChartState(FlowChartStates.Greeting)]
        GoodNight,
        
        [EnumValueString("Сегодня"), FlowChartState(FlowChartStates.DayNotification)]
        Today,
        
        [EnumValueString("В этот день"), FlowChartState(FlowChartStates.DayNotification)]
        ThisDay,
        
        [EnumValueString("Празднуется"), FlowChartState(FlowChartStates.HolidayPreview)]
        HolidayProcess,
        
        [EnumValueString("У вас запланировано событие"), FlowChartState(FlowChartStates.NoteNotification)]
        ScheduledEvent,
        
        [EnumValueString("Событие добавлено"), FlowChartState(FlowChartStates.Other)]
        EventAdded
    }
}