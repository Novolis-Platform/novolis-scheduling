using Novolis.Scheduling.Cron;

namespace Novolis.Scheduling.Unit;

public sealed class CronExpressionBehaviorTests
{
    [Test]
    public async Task Constructor_null_expression_throws()
    {
        await Assert.That(() => new CronExpression(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Every_second_advances_to_next_second()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EverySecond);
        var from = new DateTime(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(from.AddSeconds(1));
    }

    [Test]
    public async Task Every_five_minutes_rounds_up()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryFiveMinutes);
        var from = new DateTime(2024, 6, 1, 12, 7, 30, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 6, 1, 12, 10, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Every_hour_at_top_of_hour()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryHour);
        var from = new DateTime(2024, 6, 1, 12, 45, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 6, 1, 13, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Every_day_at_midnight()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryDay);
        var from = new DateTime(2024, 6, 1, 15, 30, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 6, 2, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Every_weekday_skips_weekend()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryWeekday);
        var friday = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(friday)).IsEqualTo(new DateTime(2024, 3, 18, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Every_last_day_of_month()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryLastDayOfMonth);
        var from = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Every_day_at_noon()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryDayAtNoon);
        var from = new DateTime(2024, 6, 1, 8, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Every_month_on_first()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryMonth);
        var from = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Every_year_on_january_first()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryYear);
        var from = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Range_and_step_expression()
    {
        var cron = new CronExpression("0 0 10-12 * * *");
        var from = new DateTime(2024, 6, 1, 10, 30, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 6, 1, 11, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task List_expression()
    {
        var cron = new CronExpression("0 0 0 1,15 * *");
        var from = new DateTime(2024, 6, 5, 0, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Nearest_weekday_expression()
    {
        var cron = new CronExpression("0 0 0 15W * *");
        var from = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc);
        var next = cron.Next(from);
        await Assert.That(next.Day).IsEqualTo(15);
        await Assert.That(next.Month).IsEqualTo(3);
    }

    [Test]
    public async Task Unreachable_day_in_month_returns_min_value()
    {
        var cron = new CronExpression("0 0 0 31 2,4,6 *");
        await Assert.That(cron.IsValid).IsTrue();
        await Assert.That(cron.Next(DateTime.UtcNow)).IsEqualTo(DateTime.MinValue);
    }

    [Test]
    public async Task Christmas_day_expression()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryYearOn.ChristmasDay);
        var from = new DateTime(2024, 12, 26, 0, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2025, 12, 25, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Every_sunday()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EverySunday);
        var from = new DateTime(2024, 3, 11, 12, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 3, 17, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task Every_weekend_day()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryWeekendDay);
        var from = new DateTime(2024, 3, 11, 12, 0, 0, DateTimeKind.Utc);
        await Assert.That(cron.Next(from)).IsEqualTo(new DateTime(2024, 3, 16, 0, 0, 0, DateTimeKind.Utc));
    }
}
