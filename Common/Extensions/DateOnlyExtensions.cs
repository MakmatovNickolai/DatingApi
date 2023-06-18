namespace Common.Extensions;

public static class DateOnlyExtensions
{
    public static int GetAge(this DateOnly dateOnly)
    {
        return (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - dateOnly.DayNumber) / 365;
    }
}
