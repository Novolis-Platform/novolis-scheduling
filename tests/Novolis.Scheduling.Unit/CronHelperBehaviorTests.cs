using Novolis.Scheduling.Cron;

namespace Novolis.Scheduling.Unit;

public sealed class CronHelperBehaviorTests
{
    [Test]
    public async Task Parse_returns_valid_expression()
    {
        var cron = CronHelper.Parse(PredefinedCronExpressions.EveryMinute);
        await Assert.That(cron.IsValid).IsTrue();
    }

    [Test]
    public async Task IsValid_rejects_malformed_expression()
    {
        await Assert.That(CronHelper.IsValid("bad cron")).IsFalse();
    }

    [Test]
    public async Task GetNextOccurrence_string_fromUtc()
    {
        var from = new DateTime(2024, 1, 1, 0, 0, 3, DateTimeKind.Utc);
        var next = CronHelper.GetNextOccurrence(PredefinedCronExpressions.EveryFiveSeconds, from);
        await Assert.That(next).IsEqualTo(new DateTime(2024, 1, 1, 0, 0, 5, DateTimeKind.Utc));
    }

    [Test]
    public async Task GetNextOccurrence_cronExpression_fromUtc()
    {
        var cron = CronHelper.Parse(PredefinedCronExpressions.EveryMinute);
        var from = new DateTime(2024, 6, 1, 12, 34, 45, DateTimeKind.Utc);
        await Assert.That(CronHelper.GetNextOccurrence(cron, from)).IsEqualTo(new DateTime(2024, 6, 1, 12, 35, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task GetTimeUntilNextOccurrence_string_fromUtc()
    {
        var from = new DateTime(2024, 6, 1, 12, 34, 45, DateTimeKind.Utc);
        var remaining = CronHelper.GetTimeUntilNextOccurrence(PredefinedCronExpressions.EveryMinute, from);
        await Assert.That(remaining).IsEqualTo(TimeSpan.FromSeconds(15));
    }

    [Test]
    public async Task GetTimeUntilNextOccurrence_cronExpression_no_from()
    {
        var cron = CronHelper.Parse(PredefinedCronExpressions.EveryMinute);
        var remaining = CronHelper.GetTimeUntilNextOccurrence(cron);
        await Assert.That(remaining).IsGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [Test]
    public async Task IsDue_is_false_on_exact_minute_tick()
    {
        var cron = CronHelper.Parse(PredefinedCronExpressions.EveryMinute);
        var onTick = new DateTime(2024, 6, 1, 12, 35, 0, DateTimeKind.Utc);
        await Assert.That(CronHelper.IsDue(cron, onTick)).IsFalse();
    }

    [Test]
    public async Task IsDue_string_overload_does_not_throw()
    {
        await Assert.That(CronHelper.IsDue(PredefinedCronExpressions.EveryMinute)).IsFalse();
    }

    [Test]
    public async Task Predefined_singleton_exposes_constants()
    {
        var predefined = CronHelper.Predefined;
        await Assert.That(predefined).IsNotNull();
        await Assert.That(PredefinedCronExpressions.Instance).IsEqualTo(predefined);
        await Assert.That(CronHelper.IsValid(PredefinedCronExpressions.EveryFriday)).IsTrue();
        await Assert.That(CronHelper.IsValid(PredefinedCronExpressions.EveryYearOn.Halloween)).IsTrue();
        await Assert.That(CronHelper.IsValid(PredefinedCronExpressions.EveryYearOn.NewYearsDay)).IsTrue();
    }

    [Test]
    public async Task InvalidCronExpressionException_carries_expression()
    {
        var ex = new InvalidCronExpressionException("bad");
        await Assert.That(ex.InvalidCronExpression).IsEqualTo("bad");
        await Assert.That(ex.Message).Contains("bad");
    }
}
