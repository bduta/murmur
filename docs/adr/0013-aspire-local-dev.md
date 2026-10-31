# 0013. .NET Aspire for local dev and observability

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Local dev needs Postgres, a storage emulator, the API and the Vite dev server running together, plus observability. `azd up` could also deploy, but would hide the Docker/Azure/CI learning that is a project goal.

## Decision
- **.NET Aspire AppHost** orchestrates local dev: Postgres, Azurite (Blob + Queue), API, Vite app. Aspire dashboard for logs/traces/metrics.
- **ServiceDefaults** project wires OpenTelemetry, health checks and resilience; telemetry flows to Application Insights in Azure.
- **Deployment is owned manually**: hand-written Dockerfile, Terraform (ADR-0014), GitHub Actions (ADR-0015). Aspire is not used for deployment.

## Consequences
- One-command local start.
- Every Azure resource is understood because it was written by hand.
