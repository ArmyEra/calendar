using Attributes;
using Audio.FlowChart.Utils;
using Audio.Utils;
using SpeechKitApi.Attributes;

namespace Audio.CashedSounds.Default.Utils
{
    public enum DefaultSoundType
    {
        [EnumValueString("Доброе утро!"), FlowChartState(AudioFlowChartStates.Greeting)]
        GoodMorning,
        
        [EnumValueString("Добрый день!"), FlowChartState(AudioFlowChartStates.Greeting)]
        GoodAfternoon,
        
        [EnumValueString("Добрый вечер!"), FlowChartState(AudioFlowChartStates.Greeting)]
        GoodEvening,
        
        [EnumValueString("Доброй ночи!"), FlowChartState(AudioFlowChartStates.Greeting)]
        GoodNight,
        
        [EnumValueString("Сегодня"), FlowChartState(AudioFlowChartStates.DayNotification)]
        Today,
        
        [EnumValueString("В этот день"), FlowChartState(AudioFlowChartStates.DayNotification)]
        ThisDay,
        
        [EnumValueString("Празднуется"), FlowChartState(AudioFlowChartStates.HolidayPreview)]
        HolidayProcess,
        
        [EnumValueString("У вас запланировано событие"), FlowChartState(AudioFlowChartStates.NoteNotification)]
        ScheduledEvent,
        
        [EnumValueString("Событие добавлено"), FlowChartState(AudioFlowChartStates.Other)]
        EventAdded
    }
}