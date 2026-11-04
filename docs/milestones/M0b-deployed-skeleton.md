# M0b — Deployed walking skeleton

- **Goal:** the M0a skeleton runs live on a `dev` URL in Azure, deployed automatically on merge to `main`.
- **Ships:** health page reachable at the dev Container Apps URL.
- **Main learning:** **user writes first-hand**: multi-stage Dockerfile, Terraform bootstrap (state storage) + dev environment, GitHub Actions with OIDC to Azure.
- **Agentic asset:** hook blocking agent edits to secrets and Terraform state.
- **Related ADRs:** 0012, 0014, 0015, 0016, 0022, 0024
- **Milestone acceptance criteria (draft):**
  - `docker build` produces one image serving SPA + API; runs locally with `docker run`.
  - Terraform remote state in Azure Storage; `dev` environment (resource group, Container Apps env + app, ACR, Postgres server with `murmur_dev` DB, Key Vault, App Insights, budget alerts, log cap) created from code.
  - PR workflow: build + tests + `terraform plan` comment. Main workflow: build, push to ACR, apply dev, deploy, smoke check.
  - No secrets in GitHub; Azure auth via OIDC federation. Required status checks added to `main` protection.
- **Prerequisites:** Azure subscription, Azure CLI, Terraform installed.
- **Out of scope:** prod environment (M2), custom domain.
- **Carried over from M0a:**
  - Decide the liveness/readiness split when configuring Container Apps probes. Today `/api/health` includes the Postgres check and returns 503 when the database is down (pinned by an M0a integration test), so as a readiness probe a database outage takes every replica out of rotation. `/alive` exists but ServiceDefaults maps it in Development only.

## Breakdown
- **Status:** Outline — not broken down yet. Draft a breakdown when M0a is done; it needs the user's approval before any work.
