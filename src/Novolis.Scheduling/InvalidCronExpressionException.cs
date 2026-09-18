namespace Novolis.Scheduling;

/// <summary>Thrown when a cron expression cannot be parsed.</summary>
public class InvalidCronExpressionException(string cronExpression) : Exception($"Invalid cron expression: {cronExpression}")
{
    /// <summary>The invalid expression text.</summary>
    public string InvalidCronExpression { get; } = cronExpression;
}

