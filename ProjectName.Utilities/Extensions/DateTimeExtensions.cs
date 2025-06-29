namespace ProjectName.Utilities.Extensions;
public static class DateTimeExtensions
{
    // Check if a DateTime is a weekend
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    // Get the start of the day (midnight)
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }

    // Get the end of the day (last millisecond)
    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }
}

