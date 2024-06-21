
using System;

public static class DateHelper
{
  /// <summary>
  /// Time ranges that are supported for querying from the database using a "where" clause on the date.
  /// </summary>
  public enum TimeRange
  {
    [System.ComponentModel.Description("All Time")]
    AllTime,
    [System.ComponentModel.Description("Today")]
    Today,
    [System.ComponentModel.Description("Yesterday")]
    Yesterday,
    [System.ComponentModel.Description("Past 7 Days")]
    PastWeek,
    [System.ComponentModel.Description("Current Week, Sunday to Saturday")]
    WeekSundayToSaturday,
    [System.ComponentModel.Description("Current Week, Monday to Sunday")]
    WeekMondayToSunday,
  }
  /// <summary>
  /// A "dummy" date time, set to 20 years ago. This is used internally as both the start and end date to indicate all records.
  /// </summary>
  public static readonly DateTime DummyDateTime = DateTime.Now.AddYears(-20);

  /// <summary>
  /// Get a tuple with the start and end date for the specified <paramref name="timeRange"/>
  /// </summary>
  /// <param name="timeRange">TimeRange that you want the actual start and end date for</param>
  /// <returns>A tuple with two dates for the time range, where the earlier date is <code>Item1</code></returns>
  public static Tuple<DateTime, DateTime> StartEndDatesForTimeRange(TimeRange timeRange)
  {
    switch (timeRange)
    {
      case TimeRange.Today:
        DateTime today = DateTime.Today;
        return new Tuple<DateTime, DateTime>(today, today);

      case TimeRange.Yesterday:
        DateTime yesterday = DateTime.Today.AddDays(-1.0);
        return new Tuple<DateTime, DateTime>(yesterday, yesterday);

      case TimeRange.PastWeek:
        DateTime weekAgo = DateTime.Today.AddDays(-7.0);
        DateTime today1 = DateTime.Today;
        return new Tuple<DateTime, DateTime>(weekAgo, today1);

      case TimeRange.WeekSundayToSaturday:
        DateTime today2 = DateTime.Today;
        int dow = (int)today2.DayOfWeek;

        DateTime sunday = today2.AddDays(-dow);
        DateTime saturday = today2.AddDays(6 - dow);
        // If we are on sunday, show for the current week
        if (today2.DayOfWeek == System.DayOfWeek.Sunday)
        {
          sunday = today2;
          saturday = today2.AddDays(6);
        }

        return new Tuple<DateTime, DateTime>(sunday, saturday);

      case TimeRange.WeekMondayToSunday:
        DateTime today3 = DateTime.Today;
        int dow1 = (int)today3.DayOfWeek;

        DateTime monday = today3.AddDays(1 - dow1); // Monday - day of week = goes backward to previous monday until we are in Sunday
        DateTime sunday1 = today3.AddDays(7 - dow1); // (Next monday) - day of week = goes to next monday until we are in Sunday then shows next Sunday

        // If we are on sunday, fix to show "current" week still
        if (today3.DayOfWeek == System.DayOfWeek.Sunday)
        {
          monday = today3.AddDays(-6); // Sunday - 6 = previous monday
          sunday = today3; // Sunday is today
        }

        return new Tuple<DateTime, DateTime>(monday, sunday1);

      case TimeRange.AllTime:
        return new Tuple<DateTime, DateTime>(DummyDateTime, DummyDateTime);

      default:
        return new Tuple<DateTime, DateTime>(DummyDateTime, DummyDateTime);
    }
  }
}
