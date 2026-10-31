# Current status

> Update this file at the end of every working session. A new agent session starts here.

**Last updated:** 2026-10-29
**Milestone:** M0a — Local walking skeleton (approved, in progress)

## Done
- Planning session complete: all decisions recorded in [`PLAN.md`](PLAN.md) and ADRs 0001–0027 in [`adr/`](adr/README.md). Treat them as settled.
- Codename chosen: `murmur`. Repo lives at `C:\dev\murmur` (deliberately **not** in OneDrive — sync breaks git/node_modules/bin).
- **Issue 1 — repo bootstrap:** done, **staged but not committed** (user was reviewing).
  - `global.json` pins SDK 10.0.403 (`latestFeature`; a .NET 11 RC is also installed and must not be used)
  - `Directory.Build.props` (net10.0, nullable, warnings as errors, code style in build, invariant globalization)
  - `Directory.Packages.props` (central package management)
  - `.gitignore` (+ murmur section per ADR-0016), `.gitattributes`, `.editorconfig`, `Murmur.slnx` (empty), `README.md`, `docs/journal.md`
  - Decision: generated TS API client (`web/src/api/generated/`) **is committed**, not ignored.

## Next steps
1. User finishes reviewing staged files → requested changes, then **initial commit on `main`** (Conventional Commit, e.g. `chore: bootstrap repository`).
2. User installs prerequisites (were missing on 2026-10-29):
   - Docker Desktop (`winget install Docker.DockerDesktop`) — needed from issue 2
   - GitHub CLI (`winget install GitHub.cli`) then `gh auth login` — needed to create the repo
   - Azure CLI + Terraform — not needed until M0b
3. With user's explicit OK: create **public** GitHub repo `murmur`, push, enable secret scanning push protection, Dependabot, CodeQL, branch protection on `main`.
4. Continue M0a issues 2–7 (below), one branch + PR per issue.

## M0a issues (approved breakdown, ~18 h)
| # | Issue | Who | ~h | State |
|---|---|---|---|---|
| 1 | Repo bootstrap | Agent, user reviews | 2 | Staged, awaiting commit |
| 2 | Aspire host: `Murmur.AppHost`, `Murmur.ServiceDefaults`, `Murmur.Api` with `/api/health` (version + DB check); Postgres + Azurite | Agent, user reviews | 3 | Todo |
| 3 | Module skeleton: SharedKernel (`UserId`, `Result`, `IClock`), `IModule` registration, empty `Identity` + `Posts` modules with `.Contracts` | Agent, user reviews | 3 | Todo |
| 4 | Tests: architecture tests + Testcontainers integration test for `/api/health` (tests first) | Agent, user reviews | 3 | Todo |
| 5 | Web: Vite + React + TS, Tailwind + shadcn/ui, React Router, TanStack Query, ESLint/Prettier, Vitest, added to Aspire. **User writes the health-status component + its test** (agent tutors) | Mixed | 4 | Todo |
| 6 | OpenAPI → generated TS client, `npm run gen:api`, health page uses it | Agent, user reviews | 2 | Todo |
| 7 | Fill `CLAUDE.md` commands, journal entry, tag `m0a-local-skeleton` | Together | 1 | Todo |

Out of scope for M0a: CI, Dockerfile, Terraform (M0b — user writes these first-hand), auth, real data.

## Working preferences
- User prefers questions **one at a time**, each with a recommendation.
- Challenge bad decisions; record any change of decision as a new ADR.
- Commit/push only with the user's OK.
