# M2 — Text posts + prod

- **Goal:** signed-in users can publish text posts, and the app has a production environment.
- **Ships:** create/delete text posts, profile page listing a user's posts, **first prod deploy** behind an approval gate.
- **Main learning:** transactional outbox, keyset pagination, TanStack Query, prod Terraform environment, tag-triggered releases.
- **Agentic asset:** Claude Code PR review GitHub Action.
- **Related ADRs:** 0018, 0020, 0024, 0025
- **Milestone acceptance criteria (draft):**
  - Posts module with its own schema and `DbContext`; `PostCreated`/`PostDeleted` written to the outbox.
  - Profile posts paginated with an opaque keyset cursor (no `OFFSET`).
  - Only the author can delete their post (integration-tested).
  - `prod` environment from the same Terraform modules; tag `vX.Y.Z` promotes the dev-tested image after manual approval; migrations run as a pipeline step.
- **Decision to make by the end:** public name + domain (ADR-0026).
- **Out of scope:** photos (M5), feed (M3).

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
