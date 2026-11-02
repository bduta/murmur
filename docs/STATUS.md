# Current status

> Update this file at the end of every working session. A new agent session starts here.

**Last updated:** 2026-10-31
**Milestone:** M0a — Local walking skeleton (approved, in progress) → [`milestones/M0a-local-skeleton.md`](milestones/M0a-local-skeleton.md)
**Current branch:** `feat/m0a-aspire-host` (PR #1 open)
**Where we stopped:** issue 2 is waiting for the user to merge PR #1. Per-milestone plans now live in [`milestones/`](milestones/README.md); every milestone after M0a is an outline only.

## Done
- Planning session complete: all decisions recorded in [`PLAN.md`](PLAN.md) and ADRs 0001–0027 in [`adr/`](adr/README.md). Treat them as settled.
- Codename chosen: `murmur`. Repo lives at `C:\dev\murmur` (deliberately **not** in OneDrive — sync breaks git/node_modules/bin).
- **Issue 1 — repo bootstrap:** done, committed on `main` as `494bf60 chore: bootstrap repository` (2026-10-31) and pushed.
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
- **Milestone files** (2026-10-31): `docs/milestones/` with one file per milestone. M0a holds the approved breakdown (moved here from this file). M0b–M13 are outlines (goal, scope, draft acceptance criteria, ADRs) with **no breakdown yet**. Committed on `feat/m0a-aspire-host`, so they ship with PR #1.

## Next steps
1. Issue 2: user squash-merges PR #1 (https://github.com/bduta/murmur/pull/1). Note: the health check keys are `self` and `PostgreSql` (names set by Aspire), not `postgres` as first planned.
   Then continue M0a issues 3–7 (see the M0a milestone file), one branch + PR per issue.
2. After issue 2 merges: check that CodeQL default setup has picked up C# (`gh api repos/bduta/murmur/code-scanning/default-setup`). It shows `languages: []` today because there's no code yet.
3. Before M0b: install Azure CLI + Terraform (still missing as of 2026-10-31). Docker Desktop and GitHub CLI are installed; `gh` is authenticated as `bduta`.
4. When M0a is done: draft the M0b breakdown in `milestones/M0b-deployed-skeleton.md` and **get the user's approval before starting any work or creating GitHub issues**.

## Current milestone at a glance
M0a issue states (the details and the source of truth are in [`milestones/M0a-local-skeleton.md`](milestones/M0a-local-skeleton.md)):
1 Done · **2 PR #1 awaiting merge** · 3 Todo · 4 Todo · 5 Todo · 6 Todo · 7 Todo

## Working preferences
- User prefers questions **one at a time**, each with a recommendation.
- Challenge bad decisions; record any change of decision as a new ADR.
- Commit/push only with the user's OK.
- **Milestone breakdowns need the user's explicit approval** before any work starts or GitHub issues are created. Changes to an approved breakdown need approval too (see [`milestones/README.md`](milestones/README.md)).
- At the end of every session, update this file so the next session can resume exactly where this one stopped.
