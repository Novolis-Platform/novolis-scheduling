<!-- novolis-marketing:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-brand-transparent.svg" width="360" alt="Novolis"/>
  </a>
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/banners/novolis-scheduling.svg" width="100%" alt="novolis-scheduling"/>
</p>

<p align="center">
  <strong>Scheduling primitives</strong><br/>
  Scheduling helpers for Novolis runtimes.
</p>

<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-scheduling/actions"><img src="https://img.shields.io/github/actions/workflow/status/Novolis-Platform/novolis-scheduling/merge.yml?branch=main&label=merge&logo=github" alt="merge"/></a>
  <a href="https://github.com/orgs/Novolis-Platform/packages?repo_name=novolis-scheduling"><img src="https://img.shields.io/badge/packages-GitHub%20Packages-0a7ea3?logo=nuget" alt="packages"/></a>
  <a href="https://github.com/Novolis-Platform"><img src="https://img.shields.io/badge/org-Novolis--Platform-111827" alt="org"/></a>
</p>

<p align="center">
  <a href="https://nuget.pkg.github.com/Novolis-Platform/index.json"><code>https://nuget.pkg.github.com/Novolis-Platform/index.json</code></a>
  ·
  <a href="https://github.com/Novolis-Platform/.github/blob/main/profile/README.md">Org landing</a>
  ·
  <a href="https://github.com/Novolis-Platform/novolis-governance">Governance</a>
</p>

---
<!-- novolis-marketing:end -->
# novolis-scheduling

DI-hosted cron job scheduling for .NET generic host.

## Packages

| Package | Description |
|---------|-------------|
| [Novolis.Scheduling](src/Novolis.Scheduling/README.md) | `AddCronJob<T>()`, `ICronJob`, hosted scheduler |
| [Novolis.Scheduling.Cron](src/Novolis.Scheduling.Cron/README.md) | Six-field cron parse, validate, next occurrence |

## Build

```powershell
dotnet build Novolis.Scheduling.slnx
dotnet test Novolis.Scheduling.slnx
```

Packages publish to **GitHub Packages** (`2026.1.*`) and **nuget.org** on release. Restore from those feeds only — no local folder feeds.

For local cross-repo iteration, open **`Novolis.Platform.slnx`** at the monorepo root (ProjectReference mode).

## Documentation

- [Getting started](docs/getting-started.md)
- [Design](docs/design.md)
- [Release](docs/release.md)

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## Security

See [SECURITY.md](SECURITY.md).

