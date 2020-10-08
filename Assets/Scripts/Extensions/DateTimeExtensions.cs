using System;
using Calendar.DatePanel.Utils;

namespace Extensions
{
    public static class DateTimeExtensions
    {
        public const string ConvertDateTimeFormat = "dd/MM/yyyy hh:mm:ss";
        public const string ClockDateFormat = "dd.MM.yyyy";

        /// <summary>
        /// Явялется ли дата "условно" пустой 
        /// </summary>
        public static bool IsNull(this DateTime date)
            => date == DateTime.MinValue;
        
        /// <summary>
        /// Получает индексы в таблице, с какой ячейки начинается месяц, а в какой заканичвается 
        /// </summary>
        public static void GetBoundIndexes(this DateTime date, out GridCell startIndex, out GridCell finalIndex)
        {
            var startDate = date.FirstMonthDate();
            startIndex = new GridCell(0, DateTimeExtensions.DayOfWeek(in startDate));

            var lastDate = date.LastMonthDate();
            var finalTotalIndex = startIndex.Column + (lastDate.Day - startDate.Day);
            finalIndex = GridCell.FromTotalIndex(finalTotalIndex, Params.GridSize); 
        }
        
        /// <summary>
        /// Воззвращает первую дату в месяце 
        /// </summary>
        public static DateTime FirstMonthDate(this DateTime date)
            => new DateTime(date.Year, date.Month, 1);

        /// <summary>
        /// Возвращает последнюю дату в месяце 
        /// </summary>
        public static DateTime LastMonthDate(this DateTime date)
        {
            var lastMonth = date.Month == 12;
            var yearInc = lastMonth ? 1 : 0;
            var monthInc = lastMonth ? 0 : 1;
            
            return new DateTime(date.Year + yearInc, date.Month + monthInc, 1).AddDays(-1);
        }
        
        /// <summary>
        /// Возвращает порядковый номер дня недели, начиная с 1 
        /// </summary>
        private static int DayOfWeek(in DateTime date)
            => date.DayOfWeek == System.DayOfWeek.Sunday ? 6 : ((int) date.DayOfWeek) - 1;

        /// <summary>
        /// Возвращает дату из строки
        /// </summary>
        public static DateTime GetDateTime(this string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                return DateTime.MinValue;
            
            var array = value.Trim().Split('.', '/', ' ', ':');
            
            var date = new DateTime(int.Parse(array[2]), int.Parse(array[1]), int.Parse(array[0]));
            if (array.Length == 6)
            {
                date = date
                    .AddHours(int.Parse(array[3]))
                    .AddMinutes(int.Parse(array[4]))
                    .AddSeconds(int.Parse(array[5]));
            }
            else
            {
                date = date.AddHours(12);
            }
            
            return date;
            //return DateTime.ParseExact(value, ConvertDateTimeFormat, CultureInfo.CurrentCulture);
        }
    }
}