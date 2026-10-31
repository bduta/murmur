# 0012. Single container on Azure Container Apps

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Options: App Service, Azure Container Apps, AKS. Docker experience is a learning goal; Kubernetes ops would consume whole milestones.

## Decision
- **One Docker image**: multi-stage build — Node builds the SPA, .NET publishes the API, SPA `dist/` copied into `wwwroot`, slim ASP.NET runtime image.
- Run on **Azure Container Apps** (consumption plan), images in **Azure Container Registry**.
- `dev`: scale to zero. `prod`: 1 always-on replica (no cold start during demos).
- Background workers (outbox dispatcher, media processor) run as hosted services in the same container; can move to Container Apps Jobs later.

## Consequences
- Same domain for SPA and API by construction.
- Extracting a module later = another container app in the same environment.
- Front Door/CDN or Static Web Apps can be added later without code changes.

## Alternatives considered
- App Service — simpler, but no scale-to-zero and less Docker learning.
- AKS — high ops burden and cost.
