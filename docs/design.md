# Design

DI-hosted cron job scheduling for .NET generic host, plus six-field cron parsing.

Published docs: [https://novolis-platform.github.io/.github/novolis-scheduling/](https://novolis-platform.github.io/.github/novolis-scheduling/)

## Layer placement

Follow [library-boundaries](https://github.com/Novolis-Platform/novolis-governance/blob/main/docs/library-boundaries.md). Scheduling is platform plumbing (hosting abstractions + cron). It must not take Avalonia, MAUI, Gaming, or Simulation package references.

## Goals

- Keep public APIs documented and packable as `Novolis.Scheduling` and `Novolis.Scheduling.Cron` on GitHub Packages.
- Keep cron parsing usable without a generic host (`Novolis.Scheduling.Cron`).
- Document restore and ProjectReference-mode builds without local NuGet folder feeds.

## Non-goals

- Local NuGet folder feeds or committed cross-repo `ProjectReference` into sibling checkouts.
- Distributed job clusters, persistence, or a Hangfire/Quartz replacement.
- Avalonia package references outside `Novolis.Avalonia.*`.

## Packages

- `Novolis.Scheduling`
- `Novolis.Scheduling.Cron`

## Topics

- `dotnet`
- `scheduling`
- `novolis`
