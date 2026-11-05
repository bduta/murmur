# Current status

> Update this file at the end of every working session. A new agent session starts here.

**Last updated:** 2026-11-05
**Milestone:** M0a — Local walking skeleton (approved, in progress) → [`milestones/M0a-local-skeleton.md`](milestones/M0a-local-skeleton.md)
**Current branch:** `feat/m0a-integration-tests` (issue 4, PR B)
**Where we stopped:** issue 4 shipped as two PRs. PR A (architecture tests) was squash-merged as `69c53bf` (PR #3, 2026-11-05). PR B (integration tests) is written and green on `feat/m0a-integration-tests` (34/34 tests pass), waiting for the user's OK to commit, push and open the PR. Per-milestone plans live in [`milestones/`](milestones/README.md); every milestone after M0a is an outline only.

## Done
- Planning session complete: all decisions recorded in [`PLAN.md`](PLAN.md) and ADRs 0001–0029 in [`adr/`](adr/README.md). Treat them as settled.
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
- **Local run gotchas:**
  - Run the AppHost with the default (https) profile: `dotnet run --project src/Murmur.AppHost`. The `http` profile fails at startup unless `ASPIRE_ALLOW_UNSECURED_TRANSPORT=true`. The API is at `https://localhost:7080/api/health`.
  - Killing the AppHost process (instead of Ctrl+C) leaves the `postgres-*`/`storage-*` containers running; `docker stop` them. Data volumes persist.
  - In Windows PowerShell 5.1, `git commit -F -` with a here-string doesn't work; write the message to a file in the scratchpad and use `git commit -F <file>`.
- **Milestone files:** `docs/milestones/`, one file per milestone. M0a holds the approved breakdown. M0b–M13 are outlines with **no breakdown yet**.
- **Issue 3 — module skeleton:** done, squash-merged as `58575ad` (PR #2, 2026-11-04). `src/Shared/Murmur.SharedKernel` (`UserId`, `Result`/`Result<T>`/`Error`; base .NET only), `src/Shared/Murmur.BuildingBlocks` (`IModule`), empty `Identity` and `Posts` modules plus `.Contracts` projects, explicit module registration in `Program.cs`, `TimeProvider.System` in DI. **ADR-0028** replaces the planned `IClock` with `TimeProvider`.
- **Module-boundary design review (grilling session, 2026-11-02):** 12 decisions recorded in **ADR-0029** (shared-code admission rules, cross-module reads, event shape, no cycles, visibility, enforcement). The user approved the resulting changes to issues 3 and 4 in the M0a breakdown.

## Next steps
1. Issue 4, PR B: with the user's OK, commit, push (from PowerShell) and open the PR. The PR body must say why each new package is added (`Testcontainers.PostgreSql`, MIT: real Postgres per ADR-0021; `Microsoft.AspNetCore.Mvc.Testing`, MIT: in-memory `WebApplicationFactory`). It should also mention the sabotage check: leaking the exception text and adding an unregistered `src/Modules/Fake` folder each failed the matching test. After the squash-merge: `git switch main`, `git pull --ff-only`, `git branch -D feat/m0a-integration-tests`, and mark issue 4 Done in the milestone file.
   - Integration tests need Docker running. The health tests passed on the first run because issue 2 had already built that behaviour; only the module-registration test was red before the DI seam was added.
2. Then M0a issues 5–7 (see the M0a milestone file). Each issue gets its own branch and PR unless it's too big for one; issue 4 shipped as two PRs.
3. Before M0b: install Azure CLI + Terraform (still missing as of 2026-10-31). Docker Desktop and GitHub CLI are installed; `gh` is authenticated as `bduta`.
4. When M0a is done: draft the M0b breakdown in `milestones/M0b-deployed-skeleton.md` and **get the user's approval before starting any work or creating GitHub issues**.

## Current milestone at a glance
M0a issue states (the details and the source of truth are in [`milestones/M0a-local-skeleton.md`](milestones/M0a-local-skeleton.md)):
1 Done · 2 Done · 3 Done · **4 In progress (PR A merged as #3, PR B ready for PR)** · 5 Todo · 6 Todo · 7 Todo

## Working preferences
- User prefers questions **one at a time**, each with a recommendation. This also applies inside the grilling skill, which otherwise batches questions.
- Challenge bad decisions; record any change of decision as a new ADR.
- Commit/push only with the user's OK.
- **Milestone breakdowns need the user's explicit approval** before any work starts or GitHub issues are created. Changes to an approved breakdown need approval too (see [`milestones/README.md`](milestones/README.md)).
- Recommend the grill-me (grilling) skill when a plan or design would benefit from stress-testing.
- At the end of every session, update this file so the next session can resume exactly where this one stopped.
