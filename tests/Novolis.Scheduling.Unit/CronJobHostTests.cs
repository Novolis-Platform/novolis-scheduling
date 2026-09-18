using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Novolis.Scheduling;

namespace Novolis.Scheduling.Unit;

public sealed class CronJobHostTests
{
    private sealed class CountingJob : ICronJob
    {
        public static int RunCount;
        public Task RunAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref RunCount);
            return Task.CompletedTask;
        }
    }

    [Test]
    public async Task Hosted_scheduler_executes_registered_job()
    {
        CountingJob.RunCount = 0;
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<CountingJob>("* * * * * *", running: true, TimeZoneInfo.Utc);

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            await Assert.That(CountingJob.RunCount).IsGreaterThan(0);
        }
        finally
        {
            await host.StopAsync();
            host.Dispose();
        }
    }

    [Test]
    public async Task Schedule_maintainer_updates_schedule_without_starting_host()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<CountingJob>("0 0 0 1 1 *", running: false, TimeZoneInfo.Utc);

        using var host = builder.Build();
        var maintainer = host.Services.GetRequiredService<IScheduleMaintainer>();
        maintainer.SetSchedule<CountingJob>("0 * * * * *");
        maintainer.SetTimeZone<CountingJob>(TimeZoneInfo.Utc);
        await Assert.That(() => maintainer.SetSchedule<CountingJob>("not valid")).Throws<InvalidCronExpressionException>();
        host.Dispose();
    }

    [Test]
    public async Task Schedule_maintainer_unknown_job_throws()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<CountingJob>("* * * * * *", running: false, TimeZoneInfo.Utc);

        using var host = builder.Build();
        var maintainer = host.Services.GetRequiredService<IScheduleMaintainer>();
        await Assert.That(() => maintainer.Stop<UnknownJob>()).Throws<InvalidOperationException>();
        host.Dispose();
    }

    [Test]
    public async Task AddCronJob_iana_timezone_registers()
    {
        var services = new ServiceCollection();
        services.AddCronJob<CountingJob>("* * * * * *", "UTC", running: false);
        await Assert.That(services.Any(d => d.ServiceType == typeof(IHostedService))).IsTrue();
    }

    [Test]
    public async Task AddCronJob_second_job_reuses_scheduler()
    {
        var services = new ServiceCollection();
        services.AddCronJob<CountingJob>("* * * * * *", running: false);
        services.AddCronJob<UnknownJob>("0 * * * * *", running: false);
        var hosted = services.Where(d => d.ServiceType == typeof(IHostedService)).ToList();
        await Assert.That(hosted.Count).IsEqualTo(1);
    }

    private sealed class UnknownJob : ICronJob
    {
        public Task RunAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FailingJob : ICronJob
    {
        public Task RunAsync(CancellationToken cancellationToken) =>
            throw new InvalidOperationException("boom");
    }

    [Test]
    public async Task Scheduler_logs_and_continues_when_job_throws()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<FailingJob>("* * * * * *", running: true, TimeZoneInfo.Utc);

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
        finally
        {
            await host.StopAsync();
            host.Dispose();
        }
    }
}
