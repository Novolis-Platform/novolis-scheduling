using Novolis.Scheduling.Cron;

namespace Novolis.Scheduling.Unit;

public sealed class CronExpressionAdvancedTests
{
    [Test]
    public async Task Last_day_minus_offset()
    {
        var cron = new CronExpression("0 0 0 L-1 * *");
        var from = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc);
        var next = cron.Next(from);
        await Assert.That(next.Month).IsEqualTo(3);
        await Assert.That(next.Day).IsEqualTo(30);
    }

    [Test]
    public async Task Last_weekday_of_month()
    {
        var cron = new CronExpression("0 0 0 LW * *");
        var from = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc);
        var next = cron.Next(from);
        await Assert.That(next.Month).IsEqualTo(3);
        await Assert.That(next.Day).IsEqualTo(29);
    }

    [Test]
    public async Task Nth_weekday_of_month()
    {
        var cron = new CronExpression("0 0 0 * * 1#1");
        var from = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc);
        var next = cron.Next(from);
        await Assert.That(next.DayOfWeek).IsEqualTo(DayOfWeek.Monday);
        await Assert.That(next.Day).IsEqualTo(1);
    }

    [Test]
    public async Task Step_on_day_field_advances_past_query()
    {
        var cron = new CronExpression("0 0 0 */2 * *");
        var from = new DateTime(2024, 3, 3, 12, 0, 0, DateTimeKind.Utc);
        var next = cron.Next(from);
        await Assert.That(next).IsGreaterThan(from);
        await Assert.That(next.Hour).IsEqualTo(0);
    }

    [Test]
    public async Task Day_of_week_range()
    {
        var cron = new CronExpression("0 0 0 * * 1-3");
        var from = new DateTime(2024, 3, 10, 12, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from).DayOfWeek).IsEqualTo(DayOfWeek.Monday);
    }

    [Test]
    public async Task Every_tuesday_constant()
    {
        var from = new DateTime(2024, 3, 11, 12, 0, 0, DateTimeKind.Utc);
        await Assert.That(CronHelper.GetNextOccurrence(PredefinedCronExpressions.EveryTuesday, from).DayOfWeek)
            .IsEqualTo(DayOfWeek.Tuesday);
    }

    [Test]
    public async Task GetNextOccurrence_without_from_uses_utc_now()
    {
        var next = CronHelper.GetNextOccurrence(PredefinedCronExpressions.EveryMinute);
        await Assert.That(next).IsGreaterThan(DateTime.UtcNow.AddSeconds(-5));
    }

    [Test]
    public async Task GetTimeUntilNextOccurrence_string_overload()
    {
        var remaining = CronHelper.GetTimeUntilNextOccurrence(PredefinedCronExpressions.EveryMinute);
        await Assert.That(remaining).IsGreaterThan(TimeSpan.Zero);
        await Assert.That(remaining).IsLessThanOrEqualTo(TimeSpan.FromMinutes(1));
    }
}
