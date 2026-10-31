# 0022. Agent-driven feature workflow

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Learning agentic development is a core goal. The process must be repeatable, keep me in control of design, and still teach me the weaker areas.

## Decision
**Per-feature loop**
1. **Issue**: user story, acceptance criteria (owned by me), out-of-scope.
2. **Plan**: agent in plan mode proposes approach and files; I approve/correct.
3. **Tests**: agent writes integration tests from acceptance criteria; I review them.
4. **Implement**: until tests, arch tests and lint are green.
5. **PR**: small (< ~400 changed lines), linked to the issue.
6. **CI**: build, tests, CodeQL, `terraform plan`.
7. **AI review** (Claude Code GitHub Action).
8. **Human merge** → auto-deploy to dev → Playwright smoke.

**Assets grown over time:** `CLAUDE.md` (root + per area, short and accurate), ADRs, custom skills/commands (`/new-slice`, `/new-module`, `/migration`, `/adr`), hooks (format on edit; block edits to state/secrets), MCP servers (GitHub, Playwright; later read-only Azure/Postgres).

**Guardrails:** agents never hold prod credentials; prod `terraform apply` only in CI with approval; branch protection on `main`.

**"I write the first one":** first Dockerfile, Terraform module, GitHub Actions workflow and React components are written by me with the agent as tutor.

**Rule:** never merge code I couldn't explain in an interview.

**Learning journal:** `docs/journal.md`, ~15 minutes per milestone.

## Consequences
- Slower first instance of each skill; faster and better-understood afterwards.
