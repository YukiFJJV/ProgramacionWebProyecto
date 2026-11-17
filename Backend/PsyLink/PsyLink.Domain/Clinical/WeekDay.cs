namespace PsyLink.Domain.Clinical;

public enum WeekDay
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

public static class WeekDayExtensions
{
    /// <summary>Convierte una fecha al día de la semana del dominio (Lunes a Domingo).</summary>
    public static WeekDay ToWeekDay(this DateOnly date) => date.DayOfWeek switch
    {
        DayOfWeek.Monday => WeekDay.Monday,
        DayOfWeek.Tuesday => WeekDay.Tuesday,
        DayOfWeek.Wednesday => WeekDay.Wednesday,
        DayOfWeek.Thursday => WeekDay.Thursday,
        DayOfWeek.Friday => WeekDay.Friday,
        DayOfWeek.Saturday => WeekDay.Saturday,
        _ => WeekDay.Sunday
    };
}
