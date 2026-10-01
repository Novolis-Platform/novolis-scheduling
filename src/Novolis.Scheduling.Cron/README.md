<!-- novolis-pkg-brand:start -->
[![Novolis](https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.png)](https://novolis-platform.github.io/.github/novolis-scheduling/)

[Novolis](https://github.com/Novolis-Platform) · [Docs](https://novolis-platform.github.io/.github/novolis-scheduling/) · [Source](https://github.com/Novolis-Platform/novolis-scheduling)
<!-- novolis-pkg-brand:end -->

# Novolis.Scheduling.Cron

Cron expression parsing, validation, and next-occurrence calculation (six-field syntax with seconds).

## Install

```bash
dotnet add package Novolis.Scheduling.Cron
```

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) (`net10.0`).

## Quick start

```csharp
using Novolis.Scheduling.Cron;

var expr = CronHelper.Parse("0 */5 * * * *");
var next = CronHelper.GetNextOccurrence(expr, DateTime.UtcNow);
Console.WriteLine(next);

if (CronHelper.IsValid("0 0 12 * * ?"))
    Console.WriteLine("Valid cron");
```

For hosted background jobs, use `Novolis.Scheduling` (`AddCronJob`).

## Related packages

| Package | When to use |
|---------|-------------|
| `Novolis.Scheduling` | Generic host integration and `ICronJob` scheduler |

## More documentation

- [Getting started](https://github.com/Novolis-Platform/novolis-scheduling/blob/main/docs/getting-started.md)

## Support

Pre-release (`2026.1.*` on GitHub Packages).

