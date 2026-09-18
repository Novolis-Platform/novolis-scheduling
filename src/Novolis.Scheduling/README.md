<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-scheduling">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Scheduling

DI-hosted cron job scheduling for .NET generic host: register `ICronJob` implementations, validate expressions, and run via `IHostedService`.

## Install

```bash
dotnet add package Novolis.Scheduling
```

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) (`net10.0`), `Microsoft.Extensions.Hosting`.

## Quick start

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Novolis.Scheduling;

public sealed class DailyReportJob : ICronJob
{
    public Task RunAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCronJob<DailyReportJob>("0 0 * * *", "UTC");
await builder.Build().RunAsync();
```

Cron parsing and validation use `Novolis.Scheduling.Cron` (referenced transitively).

## Related packages

| Package | When to use |
|---------|-------------|
| `Novolis.Scheduling.Cron` | Cron expressions only (no hosting) |

## More documentation

- [Getting started](https://github.com/Novolis-Platform/novolis-scheduling/blob/main/docs/getting-started.md)

## Support

Pre-release (`2026.1.*` on GitHub Packages).

