using Novolis.Scheduling.Cron;

namespace Novolis.Scheduling.Unit;

public sealed class CronHelperTests
{
    [Test]
    public async Task TryParse_valid_expression_returns_instance()
    {
        var ok = CronHelper.TryParse(PredefinedCronExpressions.EveryMinute, out var cronExpression);

        await Assert.That(ok).IsTrue();
        await Assert.That(cronExpression).IsNotNull();
        await Assert.That(cronExpression!.IsValid).IsTrue();
    }

    [Test]
    public async Task TryParse_invalid_expression_returns_false()
    {
        var ok = CronHelper.TryParse("not a cron", out var cronExpression);

        await Assert.That(ok).IsFalse();
        await Assert.That(cronExpression).IsNull();
    }

    [Test]
    public async Task GetNextOccurrence_every_five_seconds()
    {
        var from = new DateTime(2024, 1, 1, 0, 0, 3, DateTimeKind.Utc);
        var next = CronHelper.GetNextOccurrence(PredefinedCronExpressions.EveryFiveSeconds, from);

        await Assert.That(next).IsEqualTo(new DateTime(2024, 1, 1, 0, 0, 5, DateTimeKind.Utc));
    }

    [Test]
    public async Task GetTimeUntilNextOccurrence_every_minute()
    {
        var from = new DateTime(2024, 6, 1, 12, 34, 45, DateTimeKind.Utc);
        var cron = CronHelper.Parse(PredefinedCronExpressions.EveryMinute);
        var remaining = CronHelper.GetTimeUntilNextOccurrence(cron, from);

        await Assert.That(remaining).IsEqualTo(TimeSpan.FromSeconds(15));
    }

    [Test]
    public async Task IsDue_is_false_between_minute_ticks()
    {
        var from = new DateTime(2024, 6, 1, 12, 34, 45, DateTimeKind.Utc);
        var cron = CronHelper.Parse(PredefinedCronExpressions.EveryMinute);

        await Assert.That(CronHelper.IsDue(cron, from)).IsFalse();
    }

    [Test]
    public async Task Predefined_constants_are_valid()
    {
        await Assert.That(CronHelper.IsValid(PredefinedCronExpressions.EverySecond)).IsTrue();
        await Assert.That(CronHelper.IsValid(PredefinedCronExpressions.EveryWeekday)).IsTrue();
        await Assert.That(CronHelper.IsValid(PredefinedCronExpressions.EveryYearOn.ChristmasDay)).IsTrue();
    }
}
