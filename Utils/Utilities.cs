using System.Globalization;

namespace GymTracker.Utils
{
  public static class Utilities
  {
    public static string GetDayNameInSpanish(DayOfWeek day)
    {
      return CultureInfo
        .GetCultureInfo("es-ES")
        .DateTimeFormat
        .GetDayName(day);
    }

    public static string GetDayNameInSpanishCapitalized(DayOfWeek day)
    {
      var culture = CultureInfo.GetCultureInfo("es-ES");
      var dayName = GetDayNameInSpanish(day);

      return culture.TextInfo.ToTitleCase(dayName);
    }

    public static string GetLocalDateFormat(DateTime date)
    {
      return date.ToString("dd/MM/yyyy");
    }
  }

}
