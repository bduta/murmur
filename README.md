# murmur

A social platform — posts, photos, follows, likes, comments, notifications and direct messages — built as a portfolio-grade project with production habits and AI-agent-driven development.

> Status: **M0a — local walking skeleton** (in progress). See the [roadmap](docs/PLAN.md#5-roadmap).

## Architecture at a glance
- **Modular monolith** on .NET 10 / ASP.NET Core, PostgreSQL via EF Core (schema per module)
- **React + TypeScript** SPA (Vite) served by the API on the same domain
- **Azure Container Apps**, provisioned with **Terraform**, deployed by **GitHub Actions** (OIDC)
- Local development orchestrated with **.NET Aspire**

Details: [`docs/PLAN.md`](docs/PLAN.md) · Decisions: [`docs/adr/`](docs/adr/README.md) · Learning log: [`docs/journal.md`](docs/journal.md)

## Prerequisites
- .NET SDK 10 (pinned in `global.json`)
- Node.js 24+
- Docker Desktop (Aspire and Testcontainers run Postgres and Azurite in containers)

## Getting started
_Filled in as M0a lands._
