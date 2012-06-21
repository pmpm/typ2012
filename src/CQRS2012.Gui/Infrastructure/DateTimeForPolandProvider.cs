using System;

namespace CQRS2012.Gui.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }

    public class DateTimeForPolandProvider : IDateTimeProvider
    {
        private readonly TimeZoneInfo _timeZoneInfoPoland = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");

        public DateTime Now
        {
            get
            {
                return TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfoPoland);
            }
        }
    }
}