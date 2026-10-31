# 0025. Trunk-based development and releases

- **Status:** Accepted
- **Date:** 2026-10-29

## Decision
- `main` is always deployable. Short-lived branches (`feat/m3-follow-endpoint`), PR, **squash merge**. No GitFlow/`develop`.
- **Branch protection** on `main`: PR required, CI green, no force-push — for humans and agents.
- **Small PRs**: one slice, < ~400 changed lines.
- **Conventional Commits** (`feat:`, `fix:`, `chore:`, `docs:`, `test:`, `refactor:`); PR title becomes squash commit.
- Merge to `main` → auto-deploy **dev**. Tag `vX.Y.Z` → **prod** via GitHub Environment approval, promoting the image already tested in dev. At least one release per milestone; milestone tags like `m3-follow-feed`.
- **Feature flags** via Microsoft.FeatureManagement (config-based) so unfinished work can merge hidden.
- **Migrations**: EF Core migration bundles run as a pipeline step before the new image goes live — never at app startup. Must be **backward compatible** (expand → migrate → contract) so the previous image still runs on rollback.

## Consequences
- Rollback = redeploy previous image; database stays compatible.
