namespace Novolis.Scheduling;

/// <summary>Scheduled background work invoked by the cron host.</summary>
public interface ICronJob
{
    /// <summary>Runs one invocation of the job.</summary>
    Task RunAsync(CancellationToken cancellationToken);
}


