using System.Globalization;

namespace GymTracker.Utils
{
  public static class Utilities
  {
    // ===============================
    // Zona horaria República Dominicana
    // ===============================
    private static readonly TimeZoneInfo RdTimeZone =
      TimeZoneInfo.FindSystemTimeZoneById("America/Santo_Domingo");

    private static readonly CultureInfo SpanishCulture =
      CultureInfo.GetCultureInfo("es-ES");

    // ===============================
    // Fecha y hora actuales (RD)
    // ===============================
    public static DateTime NowRD()
    {
      // Siempre partimos de UTC (seguro en prod)
      return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, RdTimeZone);
    }

    public static DateTime TodayRD()
    {
      var now = NowRD();
      return new DateTime(
        now.Year,
        now.Month,
        now.Day,
        0, 0, 0,
        DateTimeKind.Unspecified);
    }

    public static DayOfWeek DayOfWeekRD()
    {
      return NowRD().DayOfWeek;
    }

    // ===============================
    // Nombres de días
    // ===============================
    public static string GetDayNameInSpanish(DayOfWeek day)
    {
      return SpanishCulture.DateTimeFormat.GetDayName(day);
    }

    public static string GetTodayNameInSpanish()
    {
      return GetDayNameInSpanish(DayOfWeekRD());
    }

    public static string GetDayNameInSpanishCapitalized(DayOfWeek day)
    {
      var dayName = GetDayNameInSpanish(day);
      return SpanishCulture.TextInfo.ToTitleCase(dayName);
    }

    public static string GetTodayNameInSpanishCapitalized()
    {
      return GetDayNameInSpanishCapitalized(DayOfWeekRD());
    }

    // ===============================
    // Formatos de fecha
    // ===============================
    public static string GetLocalDateFormat(DateTime date)
    {
      return date.ToString("dd/MM/yyyy", SpanishCulture);
    }

    public static string GetLocalDateFormat(DateOnly date)
    {
      return date.ToString("dd/MM/yyyy", SpanishCulture);
    }

    public static string GetTodayLocalDateFormat()
    {
      return GetLocalDateFormat(NowRD());
    }
  }
}
