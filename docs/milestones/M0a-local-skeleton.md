# M0a — Local walking skeleton

- **Goal:** F5 starts Postgres, Azurite, the API and the React app; the page shows "API healthy, v0.x". Tests and module boundaries are enforced by the build.
- **Related ADRs:** 0004, 0009, 0011, 0013, 0020, 0021
- **Agentic asset:** `CLAUDE.md` + ADRs in the repo
- **Out of scope:** CI, Dockerfile, Terraform (M0b — user writes these first-hand), auth, real data

## Breakdown
- **Status:** Approved (2026-10-29)
- **Total estimate:** ~18 h

| # | Issue | Who writes it | ~h | State |
|---|---|---|---|---|
| 1 | Repo bootstrap | Agent, user reviews | 2 | Done (`494bf60`, on `main`) |
| 2 | Aspire host | Agent, user reviews | 3 | PR #1 open (`feat/m0a-aspire-host`); awaiting user merge |
| 3 | Module skeleton | Agent, user reviews | 3 | Todo |
| 4 | Architecture + integration tests | Agent (tests first), user reviews | 3 | Todo |
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
- SharedKernel (`UserId`, `Result`, `IClock`), `IModule` registration pattern, empty `Identity` and `Posts` modules each with a `.Contracts` project — so architecture rules have something to check.

### Issue 4 — Architecture + integration tests
- Architecture tests: modules reference each other only via `.Contracts`; implementation types `internal` by default.
- Integration test calling `/api/health` against real Postgres via Testcontainers. Tests written first.

### Issue 5 — Web app
- Vite + React + TypeScript, Tailwind + shadcn/ui, React Router, TanStack Query, ESLint/Prettier, Vitest; Vite dev server added to Aspire.
- **User writes** the health-status component and its test; agent tutors (ADR-0022 "I write the first one").

### Issue 6 — Typed API client
- .NET OpenAPI document → generated TS client, one `npm run gen:api` script; the health page uses the generated client.

### Issue 7 — Wrap-up
- Fill the commands section of `CLAUDE.md`, write the M0a entry in `docs/journal.md`, tag `m0a-local-skeleton`.
