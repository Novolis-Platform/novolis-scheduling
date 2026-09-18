using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Novolis.Scheduling;

namespace Novolis.Scheduling.Unit;

public sealed class CronJobHostAdvancedTests
{
    [Test]
    public async Task Schedule_maintainer_start_restarts_stopped_job()
    {
        StartRestartJob.RunCount = 0;
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<StartRestartJob>("* * * * * *", running: true, TimeZoneInfo.Utc);

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            var maintainer = host.Services.GetRequiredService<IScheduleMaintainer>();
            maintainer.Stop<StartRestartJob>();
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            maintainer.Start<StartRestartJob>();
            await Task.Delay(TimeSpan.FromSeconds(2));
            await Assert.That(StartRestartJob.RunCount).IsGreaterThan(0);
        }
        finally
        {
            await host.StopAsync();
            host.Dispose();
        }
    }

    [Test]
    public async Task Schedule_maintainer_set_schedule_triggers_refresh_while_running()
    {
        RefreshCountingJob.RunCount = 0;
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<RefreshCountingJob>("0 * * * * *", running: true, TimeZoneInfo.Utc);

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            var maintainer = host.Services.GetRequiredService<IScheduleMaintainer>();
            maintainer.SetSchedule<RefreshCountingJob>("* * * * * *");
            await Task.Delay(TimeSpan.FromSeconds(2));
            await Assert.That(RefreshCountingJob.RunCount).IsGreaterThan(0);
        }
        finally
        {
            await host.StopAsync();
            host.Dispose();
        }
    }

    [Test]
    public async Task Schedule_maintainer_missing_job_throws_for_all_operations()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<MissingJobProbe>("* * * * * *", running: false, TimeZoneInfo.Utc);

        using var host = builder.Build();
        var maintainer = host.Services.GetRequiredService<IScheduleMaintainer>();
        await Assert.That(() => maintainer.SetSchedule<UnknownJob>("* * * * * *")).Throws<InvalidOperationException>();
        await Assert.That(() => maintainer.SetTimeZone<UnknownJob>(TimeZoneInfo.Utc)).Throws<InvalidOperationException>();
        await Assert.That(() => maintainer.Start<UnknownJob>()).Throws<InvalidOperationException>();
        host.Dispose();
    }

    [Test]
    public async Task Unreachable_cron_does_not_schedule_timer()
    {
        UnreachableCountingJob.RunCount = 0;
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<UnreachableCountingJob>("0 0 0 31 2,4,6 *", running: true, TimeZoneInfo.Utc);

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            await Assert.That(UnreachableCountingJob.RunCount).IsEqualTo(0);
        }
        finally
        {
            await host.StopAsync();
            host.Dispose();
        }
    }

    [Test]
    public async Task Stopped_job_skips_execution_when_timer_fires()
    {
        StoppedCountingJob.RunCount = 0;
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<StoppedCountingJob>("* * * * * *", running: true, TimeZoneInfo.Utc);

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            var maintainer = host.Services.GetRequiredService<IScheduleMaintainer>();
            maintainer.Stop<StoppedCountingJob>();
            var countAfterStop = StoppedCountingJob.RunCount;
            await Task.Delay(TimeSpan.FromSeconds(2));
            await Assert.That(StoppedCountingJob.RunCount).IsEqualTo(countAfterStop);
        }
        finally
        {
            await host.StopAsync();
            host.Dispose();
        }
    }

    [Test]
    public async Task Invalid_running_descriptor_is_skipped_at_startup()
    {
        var builder = Host.CreateApplicationBuilder();
        SchedulingTestRegistration.AddInvalidRunningDescriptor(builder.Services, typeof(InvalidStartupJob));

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }
        finally
        {
            await host.StopAsync();
            host.Dispose();
        }
    }

    [Test]
    public async Task Invalid_schedule_change_is_ignored_after_startup()
    {
        InvalidChangeJob.RunCount = 0;
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCronJob<InvalidChangeJob>("* * * * * *", running: true, TimeZoneInfo.Utc);

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            await Assert.That(InvalidChangeJob.RunCount).IsGreaterThan(0);
            var countBeforeInvalidChange = InvalidChangeJob.RunCount;
            var descriptor = SchedulingTestRegistration.GetDescriptor(host.Services, typeof(InvalidChangeJob));
            SchedulingTestRegistration.SetSchedule(descriptor, "not a cron");
            SchedulingTestRegistration.InvokeScheduleChanged(host.Services, descriptor);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await Assert.That(InvalidChangeJob.RunCount).IsGreaterThanOrEqualTo(countBeforeInvalidChange);
        }
        finally
        {
            await host.StopAsync();
            host.Dispose();
        }
    }

    [Test]
    public async Task Missing_keyed_job_is_swallowed_by_timer_callback()
    {
        var builder = Host.CreateApplicationBuilder();
        SchedulingTestRegistration.AddDescriptorWithoutKeyedJob(builder.Services, typeof(MissingKeyedJob), "* * * * * *");

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

    private sealed class StartRestartJob : ICronJob
    {
        public static int RunCount;
        public Task RunAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref RunCount);
            return Task.CompletedTask;
        }
    }

    private sealed class RefreshCountingJob : ICronJob
    {
        public static int RunCount;
        public Task RunAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref RunCount);
            return Task.CompletedTask;
        }
    }

    private sealed class StoppedCountingJob : ICronJob
    {
        public static int RunCount;
        public Task RunAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref RunCount);
            return Task.CompletedTask;
        }
    }

    private sealed class UnreachableCountingJob : ICronJob
    {
        public static int RunCount;
        public Task RunAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class InvalidChangeJob : ICronJob
    {
        public static int RunCount;
        public Task RunAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref RunCount);
            return Task.CompletedTask;
        }
    }

    private sealed class MissingJobProbe : ICronJob
    {
        public Task RunAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class InvalidStartupJob : ICronJob
    {
        public Task RunAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class MissingKeyedJob : ICronJob
    {
        public Task RunAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class UnknownJob : ICronJob
    {
        public Task RunAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
