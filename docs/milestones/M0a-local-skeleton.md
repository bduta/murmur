# M0a — Local walking skeleton

- **Goal:** F5 starts Postgres, Azurite, the API and the React app; the page shows "API healthy, v0.x". Tests and module boundaries are enforced by the build.
- **Related ADRs:** 0004, 0009, 0011, 0013, 0020, 0021
- **Agentic asset:** `CLAUDE.md` + ADRs in the repo
- **Out of scope:** CI, Dockerfile, Terraform (M0b — user writes these first-hand), auth, real data

## Breakdown
- **Status:** Approved (2026-10-29); issues 3 and 4 changed with approval on 2026-11-02 after the module-boundary design review (ADR-0029)
- **Total estimate:** ~18 h

| # | Issue | Who writes it | ~h | State |
|---|---|---|---|---|
| 1 | Repo bootstrap | Agent, user reviews | 2 | Done (`494bf60`, on `main`) |
| 2 | Aspire host | Agent, user reviews | 3 | Done (`7b29525`, PR #1) |
| 3 | Module skeleton | Agent, user reviews | 3 | Done (`58575ad`, PR #2) |
| 4 | Architecture + integration tests | Agent (tests first), user reviews | 3 | In progress: PR A (architecture tests) open, PR B (integration tests) todo |
| 5 | Web app | Mixed — **user writes the health-status component + its test** | 4 | Todo |
| 6 | Typed API client | Agent, user reviews | 2 | Todo |
| 7 | Wrap-up | Together | 1 | Todo |

### Issue 1 — Repo bootstrap
- `git init`, `.gitignore` (+ murmur section per ADR-0016), `.gitattributes`, `.editorconfig`, `global.json` pinned to .NET 10 SDK 10.0.403 (`latestFeature`), `Directory.Build.props` (net10.0, nullable, warnings as errors, code style in build, invariant globalization), `Directory.Packages.props` (central package management), `Murmur.slnx`, `README.md`, docs.
- **Notes:** the generated TS API client (`web/src/api/generated/`) **is committed**, not ignored, so contract changes show in PR diffs and CI needs no running API.

### Issue 2 — Aspire host
- `Murmur.AppHost`, `Murmur.ServiceDefaults`, `Murmur.Api` with `/api/health` returning version + database check; Postgres and Azurite wired via Aspire.
- **Notes:** health check keys are `self` and `PostgreSql` (names set by Aspire), not `postgres` as first planned. Solution version set to `0.1.0` in `Directory.Build.props`.

### Issue 3 — Module skeleton
- SharedKernel (`UserId`, `Result`; base .NET only), BuildingBlocks (`IModule`), explicit module registration in `Program.cs`, `TimeProvider.System` registered in DI (ADR-0028 replaces the planned `IClock`), empty `Identity` and `Posts` modules each with a `.Contracts` project — so architecture rules have something to check.
- ADR-0029 records the module-boundary rules from the 2026-11-02 design review. The PR includes a manual Aspire run of `/api/health` as evidence.
- **Notes:** integration-event base types are deferred to the outbox (M2). Modules map endpoints onto an `/api` route group.

### Issue 4 — Architecture + integration tests
- `tests/Murmur.ArchitectureTests` (ArchUnitNET, `TngTech.ArchUnitNET.xUnitV3`; check licence first) plus a hand-written `.csproj` reference test, enforcing ADR-0029 §7:
  1. SharedKernel references base .NET only;
  2. `.Contracts` reference neither BuildingBlocks nor framework types;
  3. BuildingBlocks references no module;
  4. module implementations are referenced only by `Murmur.Api` and test projects;
  5. no cycles between modules;
  6. only the `IModule` class is public in implementations (exempt: EF `Migration`/`ModelSnapshot`);
  7. no `DateTime.UtcNow`/`DateTimeOffset.UtcNow` in modules (ADR-0028).
- `tests/Murmur.Api.IntegrationTests`: `/api/health` against real Postgres via Testcontainers. Tests written first.
- **Notes:** may run slightly over the 3 h estimate because of the larger rule set.
  - Design review (grilling, 2026-11-04) decisions:
    - Rules 1–5 are checked only on the `.csproj` files (pure rule functions plus permanent in-memory "must fail" tests); rules 6–7 only with ArchUnitNET. Rules 6–7 were broken by hand once and the failures recorded in PR A.
    - The clock ban covers all of `src/` except ServiceDefaults and AppHost, and also bans `Now`/`Today` (ADR-0028 amendment).
    - Modules are found by scanning `src/Modules/*/`; misplaced module projects and references added in `Directory.Build.*`/`Directory.Packages.props` fail a test.
    - PR B adds a test that the modules registered in `Program.cs` match the module projects on disk (needs a one-line DI seam), and pins "database down → 503 Unhealthy" as intended.
    - Shipped as two PRs under this one issue: A = architecture tests (`feat/m0a-arch-tests`), B = integration tests (`feat/m0a-integration-tests`).
  - xunit.v3 4.x runs on Microsoft.Testing.Platform: `global.json` opts `dotnet test` into it, and `Microsoft.NET.Test.Sdk`/`xunit.runner.visualstudio` aren't needed.

### Issue 5 — Web app
- Vite + React + TypeScript, Tailwind + shadcn/ui, React Router, TanStack Query, ESLint/Prettier, Vitest; Vite dev server added to Aspire.
- **User writes** the health-status component and its test; agent tutors (ADR-0022 "I write the first one").

### Issue 6 — Typed API client
- .NET OpenAPI document → generated TS client, one `npm run gen:api` script; the health page uses the generated client.

### Issue 7 — Wrap-up
- Fill the commands section of `CLAUDE.md`, write the M0a entry in `docs/journal.md`, tag `m0a-local-skeleton`.
