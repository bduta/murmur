# 0016. Public repo, no license, secrets policy

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
A public repo shows how I work and unlocks free security tooling, but must never leak secrets. Commercial options (paid content) should stay open.

## Decision
- Repository is **public from day one**.
- **No license file** (all rights reserved): viewable, not legally reusable.
- Enabled: secret scanning with **push protection**, **CodeQL**, **Dependabot**.
- Secrets policy:
  - No secrets in the repo, ever. A leaked secret is **rotated**, not just deleted.
  - CI → Azure via OIDC; runtime secrets in **Key Vault** accessed via **managed identity**.
  - Local: `dotnet user-secrets` / Aspire parameters.
  - `.gitignore` covers `.env*`, real `*.tfvars`, `*.tfstate*`, `.terraform/`.

## Consequences
- Commit history and PRs are public portfolio material — write them well.
