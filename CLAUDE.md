# murmur

Social platform (posts, follows, likes, comments, notifications, DMs). Solo developer + AI agents. Portfolio-grade, built with production habits.

**Start of every session:** read `docs/STATUS.md` (where we left off, next steps) and the current milestone file in `docs/milestones/`. Update both before ending.

**Milestone breakdowns:** draft them in the milestone's file in `docs/milestones/` and **wait for the user's explicit approval** before starting work or creating GitHub issues. Changes to an approved breakdown need approval too.

**Read first:** `docs/PLAN.md` (architecture, stack, roadmap) and the ADR relevant to your change in `docs/adr/`. Do not contradict an accepted ADR — if a change requires it, stop and propose a new ADR instead.

## Stack
- Backend: .NET 10, ASP.NET Core Minimal APIs, EF Core + Npgsql (PostgreSQL), FluentValidation, ASP.NET Core Identity (cookie auth), SignalR
- Frontend: `web/` — Vite + React + TypeScript, React Router, TanStack Query, Tailwind + shadcn/ui, React Hook Form + Zod, OpenAPI-generated client
- Local: .NET Aspire (`src/Murmur.AppHost`)
- Infra: Terraform in `infra/`, Azure Container Apps, GitHub Actions

## Commands
<!-- Fill in during M0a once the solution exists. -->
- Run everything locally: `dotnet run --project src/Murmur.AppHost`
- Backend tests: `dotnet test`
- Frontend: `cd web && npm run dev | npm test | npm run lint`
- Regenerate API client: _TBD in M0a_

## Architecture rules (enforced by architecture tests)
- Modular monolith. Modules live in `src/Modules/<Name>/` as `Murmur.<Name>` (internal) + `Murmur.<Name>.Contracts` (public).
- A module may reference another module **only** through its `.Contracts` project. Never another module's `DbContext`, entities or internals.
- Shared code: `Shared/Murmur.SharedKernel` (base .NET only, no module-owned rules) and `Shared/Murmur.BuildingBlocks` (generic plumbing, never referenced by contracts). Cross-module reads, event shape, no cycles: **ADR-0029**.
- One `DbContext`, Postgres schema and migration set per module.
- Cross-module side effects go through integration events via the **transactional outbox**. Event handlers must be idempotent.
- Reference users only by `UserId` (GUID from SharedKernel) — never Identity's internal IDs or email.
- Inside a module, use **vertical slices**: one folder per feature containing endpoint, request, validator, handler.
- Handlers are plain classes injected into endpoints. Errors return `ProblemDetails`.

## Do not add
- MediatR, AutoMapper, MassTransit, FluentAssertions, ImageSharp (licensing — ADR-0020, ADR-0017)
- EF Core InMemory or SQLite for tests (ADR-0021)
- Next.js, a Node backend, microservices, Kubernetes (ADR-0003/0004/0011/0012)
- Tokens in `localStorage`; public blob containers
- New NuGet/npm dependencies without saying why in the PR

## Testing
- Write integration tests **first**, from the issue's acceptance criteria (xUnit v3 + WebApplicationFactory + Testcontainers Postgres).
- Every authorization rule has an integration test.
- Unit tests only for pure logic. No coverage targets.
- Feed/history pagination is keyset-based; never `OFFSET`.

## Workflow
- Branch from `main` (`feat/<milestone>-<slice>`), small PRs (< ~400 lines), Conventional Commit PR titles.
- Plan before implementing; wait for approval on the plan.
- Migrations must be backward compatible (expand → migrate → contract); never run them at app startup.

## Never
- Commit secrets, `.env*`, real `*.tfvars` or `*.tfstate*`.
- Edit Terraform state or run `terraform apply` against prod.
- Use or request production credentials.
