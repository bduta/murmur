# Current status

> Update this file at the end of every working session. A new agent session starts here.

**Last updated:** 2026-10-31
**Milestone:** M0a — Local walking skeleton (approved, in progress)

## Done
- Planning session complete: all decisions recorded in [`PLAN.md`](PLAN.md) and ADRs 0001–0027 in [`adr/`](adr/README.md). Treat them as settled.
- Codename chosen: `murmur`. Repo lives at `C:\dev\murmur` (deliberately **not** in OneDrive — sync breaks git/node_modules/bin).
- **Issue 1 — repo bootstrap:** done, committed on `main` as `494bf60 chore: bootstrap repository` (2026-10-31). Not pushed yet.
  - `global.json` pins SDK 10.0.403 (`latestFeature`; a .NET 11 RC is also installed and must not be used)
  - `Directory.Build.props` (net10.0, nullable, warnings as errors, code style in build, invariant globalization)
  - `Directory.Packages.props` (central package management)
  - `.gitignore` (+ murmur section per ADR-0016), `.gitattributes`, `.editorconfig`, `Murmur.slnx` (empty), `README.md`, `docs/journal.md`
  - Decision: generated TS API client (`web/src/api/generated/`) **is committed**, not ignored.
- **GitHub repo:** public at https://github.com/bduta/murmur (created 2026-10-31, `main` pushed).
  - Secret scanning + push protection, Dependabot alerts + security updates: on.
  - CodeQL default setup: `configured`, no languages yet (no code).
  - `main` protection: PR required (0 approvals; solo), enforced for admins, linear history, no force-push/deletion. No required status checks until CI exists (M0b). **Direct pushes to `main` are blocked**: everything goes through a PR.
  - In this environment, `git push` works from PowerShell but not the Bash tool, which can't authenticate.
  - `gh repo create --public` is blocked by Claude Code's auto-mode classifier. The user runs public-surface actions themselves with `!`.

## Next steps
1. Issue 2: user squash-merges PR #1 (https://github.com/bduta/murmur/pull/1). Note: the health check keys are `self` and `PostgreSql` (names set by Aspire), not `postgres` as first planned.
   Then continue M0a issues 3–7 (below), one branch + PR per issue.
2. After issue 2 merges: check that CodeQL default setup has picked up C# (`gh api repos/bduta/murmur/code-scanning/default-setup`). It shows `languages: []` today because there's no code yet.
3. Before M0b: install Azure CLI + Terraform (still missing as of 2026-10-31). Docker Desktop and GitHub CLI are installed; `gh` is authenticated as `bduta`.

## M0a issues (approved breakdown, ~18 h)
| # | Issue | Who | ~h | State |
|---|---|---|---|---|
| 1 | Repo bootstrap | Agent, user reviews | 2 | Done (`494bf60`) |
| 2 | Aspire host: `Murmur.AppHost`, `Murmur.ServiceDefaults`, `Murmur.Api` with `/api/health` (version + DB check); Postgres + Azurite | Agent, user reviews | 3 | PR #1 open (`feat/m0a-aspire-host`); awaiting user merge |
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
