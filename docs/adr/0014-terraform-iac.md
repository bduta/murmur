# 0014. Terraform for infrastructure as code

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Bicep is simpler for Azure-only (no state), but Terraform has far greater job-market value and is multi-cloud. Chosen for career value.

## Decision
- **Terraform** with the `azurerm` provider; `azapi` only for features missing from `azurerm`.
- **Remote state** in an Azure Storage account (azurerm backend, blob lease locking).
- The state storage account is created once by a small **bootstrap** config (or `az` CLI script) — the only resource outside the main config.
- One state file per environment (`dev.tfstate`, `prod.tfstate`); shared modules, per-env `.tfvars`.
- `terraform plan` on every PR (posted as comment); `apply` only from CI after merge; prod requires approval.
- Budgets, alerts, log caps and tags are defined in Terraform (ADR-0024).

## Consequences
- State must be protected: never committed, never edited by hand or by agents.
- Infrastructure is reproducible; environments can be destroyed and recreated.

## Alternatives considered
- Bicep — faster for Azure, no state; less portable career skill.
