﻿using System;

namespace Audio.CashedSounds.Default.Utils
{
    public static class DayTimeSoundManager
    {
        private const int MORNING_HOUR = 4;
        private const int AFTERNOON_HOUR = 11;
        private const int EVENING_HOUR = 17;
        private const int NIGHT_HOUR = 22; 
        
        /// <summary>
        /// Возвращает тип привествия, в зависимости от времени 
        /// </summary>
        public static DefaultSoundType GetGreetingSoundType()
        {
            var currentHour = DateTime.Now.TimeOfDay.Hours;

            if (currentHour >= MORNING_HOUR && currentHour < AFTERNOON_HOUR)
                return DefaultSoundType.GoodMorning;

            if (currentHour >= AFTERNOON_HOUR && currentHour < EVENING_HOUR)
                return DefaultSoundType.GoodAfternoon;
            
            if (currentHour >= EVENING_HOUR && currentHour < NIGHT_HOUR)
                return DefaultSoundType.GoodEvening;

            return DefaultSoundType.GoodNight;
        }
    }
}