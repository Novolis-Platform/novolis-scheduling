using Microsoft.Extensions.DependencyInjection;
using Novolis.Scheduling;

namespace Novolis.Scheduling.Unit;

public sealed class SchedulingDiTests
{
    private sealed class Job : ICronJob
    {
        public Task RunAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    [Test]
    public async Task AddCronJob_InvalidExpression_Throws()
    {
        var services = new ServiceCollection();
        await Assert.That(() => services.AddCronJob<Job>("not a cron", running: true))
            .Throws<InvalidCronExpressionException>();
    }

    [Test]
    public async Task AddCronJob_ValidExpression_RegistersDescriptor()
    {
        var services = new ServiceCollection();
        services.AddCronJob<Job>("* * * * * *", running: true, TimeZoneInfo.Utc);
        await Assert.That(services.Any(d => d.ServiceType.Name.Contains("ICronJobDescriptor", StringComparison.Ordinal))).IsTrue();
    }
}
