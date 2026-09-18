using Novolis.Scheduling.Cron;

namespace Novolis.Scheduling.Unit;

public class CronExpressionTests
{
    [Test]
    public async Task Every_minute_expression_is_valid()
    {
        var cron = new CronExpression("* * * * * *");
        await Assert.That(cron.IsValid).IsTrue();
    }

    [Test]
    public async Task Invalid_expression_is_not_valid()
    {
        var cron = new CronExpression("not a cron");
        await Assert.That(cron.IsValid).IsFalse();
    }

    [Test]
    public async Task Next_every_minute_rounds_up_to_next_minute()
    {
        var cron = new CronExpression(PredefinedCronExpressions.EveryMinute);
        var from = new DateTime(2024, 3, 15, 10, 12, 34, DateTimeKind.Utc);
        var next = cron.Next(from);

        await Assert.That(next).IsEqualTo(new DateTime(2024, 3, 15, 10, 13, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task ToString_returns_original_expression()
    {
        const string expression = "0 0 12 * * *";
        var cron = new CronExpression(expression);

        await Assert.That(cron.ToString()).IsEqualTo(expression);
    }

    [Test]
    public async Task Next_invalid_expression_returns_min_value()
    {
        var cron = new CronExpression("bad");
        await Assert.That(cron.Next(DateTime.UtcNow)).IsEqualTo(DateTime.MinValue);
    }
}
