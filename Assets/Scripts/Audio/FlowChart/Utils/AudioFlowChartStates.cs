namespace Audio.FlowChart.Utils
{
    /// <summary>
    /// Вершины графа менеджера воспроизведения звука
    /// </summary>
    public enum AudioFlowChartStates
    {
        Null,
        
        Greeting,
        
        DayNotification,
        
        HolidayPreview,
        HolidayNotification,
        
        NoteNotification,
        
        Other
    }
}