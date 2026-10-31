# 0024. Cost budget and guardrails

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Hobby budget; cloud bills surprise people through forgotten resources and log ingestion.

## Decision
**Target: ~€25–40/month** (estimates; verify with the Azure pricing calculator).

| Resource | Choice | ~€/month |
|---|---|---|
| PostgreSQL Flexible Server | One Burstable B1ms; `murmur_dev` + `murmur_prod` databases, separate credentials | 13–16 (free 12 months on new account) |
| Container Apps prod | 1 replica, 0.25 vCPU / 0.5 GiB | 5–15 |
| Container Apps dev | scale to zero | 0–2 |
| Container Registry | Basic | ~5 |
| Blob + Queue, Key Vault | LRS / Standard | <2 |
| App Insights / Log Analytics | sampling + daily cap within free 5 GB | ~0 |
| Email | ACS Email or free-tier provider (pick in M1) | ~0–1 |
| Domain | — | ~1 |

**Guardrails (in Terraform):** Azure budget with alerts at 50/80/100%; Log Analytics daily cap; App Insights sampling; `env` and `project` tags on everything.

**Deferred until a milestone needs them (cost stated up front):** Front Door/CDN, Azure SignalR Standard, Redis, second Postgres server, AKS.

## Consequences
- Shared DB server: a dev load test could slow prod. Accepted for now; **split servers before paid content (M13)**.
