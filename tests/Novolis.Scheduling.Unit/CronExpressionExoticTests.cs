using Novolis.Scheduling.Cron;

namespace Novolis.Scheduling.Unit;

public sealed class CronExpressionExoticTests
{
    [Test]
    public async Task Day_31_rolls_to_next_month_with_31_days()
    {
        var cron = new CronExpression("0 0 0 31 * *");
        var from = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 3, 31, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Last_friday_of_month()
    {
        var cron = new CronExpression("0 0 0 * * 5L");
        var from = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 3, 29, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Weekday_near_month_end_in_june()
    {
        var cron = new CronExpression("0 0 0 30W 6 *");
        var from = new DateTime(2024, 5, 15, 0, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 6, 28, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task First_weekday_when_first_is_saturday()
    {
        var cron = new CronExpression("0 0 0 1W 8 *");
        var from = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Last_weekday_of_june()
    {
        var cron = new CronExpression("0 0 0 LW 6 *");
        var from = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 6, 28, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Day_of_week_next_skips_to_following_week()
    {
        var cron = new CronExpression("0 0 0 * * 1");
        var from = new DateTime(2024, 3, 10, 12, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 3, 11, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Descending_day_of_week_range()
    {
        var cron = new CronExpression("0 0 0 * * 5-3");
        var from = new DateTime(2024, 3, 10, 12, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 3, 13, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Unreachable_weekday_with_weekend_only_day_of_week()
    {
        var cron = new CronExpression("0 0 0 15W * 0,6");
        await Assert.That(cron.IsValid).IsTrue();
        await Assert.That(cron.Next(DateTime.UtcNow)).IsEqualTo(DateTime.MinValue);
    }

    [Test]
    public async Task Year_rollover_when_no_more_matches_in_calendar_year()
    {
        var cron = new CronExpression("0 0 0 1 1 *");
        var from = new DateTime(2024, 12, 31, 23, 59, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Day_list_skips_to_next_month_when_day_exceeds_month_length()
    {
        var cron = new CronExpression("0 0 0 30,31 * *");
        var from = new DateTime(2024, 2, 29, 0, 0, 0, DateTimeKind.Utc);
        var next = cron.Next(from);
        await Assert.That(next.Month).IsEqualTo(3);
        await Assert.That(next.Day).IsEqualTo(30);
    }

    [Test]
    public async Task Saturday_not_on_first_uses_previous_friday()
    {
        var cron = new CronExpression("0 0 0 8W 8 *");
        var from = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc);
        var next = cron.Next(from);
        await Assert.That(next.DayOfWeek).IsEqualTo(DayOfWeek.Friday);
        await Assert.That(next.Day).IsEqualTo(7);
    }

    [Test]
    public async Task Sunday_on_month_last_day_uses_previous_friday()
    {
        var cron = new CronExpression("0 0 0 30W 6 *");
        var from = new DateTime(2024, 6, 30, 12, 0, 0, DateTimeKind.Utc);
        var next = cron.Next(from);
        await Assert.That(next).IsGreaterThan(from);
        await Assert.That(next.Month).IsEqualTo(6);
    }
}
