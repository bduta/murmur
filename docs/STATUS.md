# Current status

> Update this file at the end of every working session. A new agent session starts here.

**Last updated:** 2026-11-02
**Milestone:** M0a — Local walking skeleton (approved, in progress) → [`milestones/M0a-local-skeleton.md`](milestones/M0a-local-skeleton.md)
**Current branch:** `feat/m0a-module-skeleton` (PR #2 open)
**Where we stopped:** issue 3 (module skeleton) is in PR #2 (https://github.com/bduta/murmur/pull/2), waiting for the user to review and squash-merge. Per-milestone plans live in [`milestones/`](milestones/README.md); every milestone after M0a is an outline only.

## Done
- Planning session complete: all decisions recorded in [`PLAN.md`](PLAN.md) and ADRs 0001–0028 in [`adr/`](adr/README.md). Treat them as settled.
- Codename chosen: `murmur`. Repo lives at `C:\dev\murmur` (deliberately **not** in OneDrive — sync breaks git/node_modules/bin).
- **Issue 1 — repo bootstrap:** done, `494bf60` on `main` (2026-10-31).
  - `global.json` pins SDK 10.0.403 (`latestFeature`; a .NET 11 RC is also installed and must not be used)
  - `Directory.Build.props` (net10.0, nullable, warnings as errors, code style in build, invariant globalization)
  - `Directory.Packages.props` (central package management)
  - Decision: generated TS API client (`web/src/api/generated/`) **is committed**, not ignored.
- **Issue 2 — Aspire host:** done, squash-merged as `7b29525` (PR #1, 2026-11-02). Health check keys are `self` and `PostgreSql`.
- **GitHub repo:** public at https://github.com/bduta/murmur.
  - Secret scanning + push protection, Dependabot alerts + security updates: on.
  - CodeQL default setup: `configured`, languages `csharp` (confirmed 2026-11-02).
  - `main` protection: PR required (0 approvals; solo), enforced for admins, linear history, no force-push/deletion. No required status checks until CI exists (M0b). **Direct pushes to `main` are blocked**: everything goes through a PR. Merged branches are auto-deleted on the remote.
  - In this environment, `git push` works from PowerShell but not the Bash tool, which can't authenticate.
  - `gh repo create --public` is blocked by Claude Code's auto-mode classifier. The user runs public-surface actions themselves with `!`.
- **Milestone files:** `docs/milestones/`, one file per milestone. M0a holds the approved breakdown. M0b–M13 are outlines with **no breakdown yet**.
- **Issue 3 — module skeleton (in progress, 2026-11-02):** `src/Shared/Murmur.SharedKernel` (`UserId`, `Result`/`Result<T>`/`Error`, `IModule`), empty `Identity` and `Posts` modules plus `.Contracts` projects, explicit module registration in `Program.cs`, `TimeProvider.System` in DI. **ADR-0028** replaces the planned `IClock` with `TimeProvider`.

## Next steps
1. Issue 3: user reviews and squash-merges PR #2. Then switch to `main`, pull, and delete the local branch.
2. Issue 4: architecture tests + `/api/health` integration test (tests first). Include: module→module only via `.Contracts`; module implementation types `internal` except the `IModule` class; no `DateTime.UtcNow`/`DateTimeOffset.UtcNow` in modules (ADR-0028).
3. Then M0a issues 5–7 (see the M0a milestone file), one branch + PR per issue.
4. Before M0b: install Azure CLI + Terraform (still missing as of 2026-10-31). Docker Desktop and GitHub CLI are installed; `gh` is authenticated as `bduta`.
5. When M0a is done: draft the M0b breakdown in `milestones/M0b-deployed-skeleton.md` and **get the user's approval before starting any work or creating GitHub issues**.

## Current milestone at a glance
M0a issue states (the details and the source of truth are in [`milestones/M0a-local-skeleton.md`](milestones/M0a-local-skeleton.md)):
1 Done · 2 Done · **3 PR #2 awaiting merge** · 4 Todo · 5 Todo · 6 Todo · 7 Todo

## Working preferences
- User prefers questions **one at a time**, each with a recommendation.
- Challenge bad decisions; record any change of decision as a new ADR.
- Commit/push only with the user's OK.
- **Milestone breakdowns need the user's explicit approval** before any work starts or GitHub issues are created. Changes to an approved breakdown need approval too (see [`milestones/README.md`](milestones/README.md)).
- Recommend the grill-me (grilling) skill when a plan or design would benefit from stress-testing.
- At the end of every session, update this file so the next session can resume exactly where this one stopped.
