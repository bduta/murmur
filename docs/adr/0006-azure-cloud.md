# 0006. Azure as cloud provider

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Choice between AWS and Azure for a .NET backend.

## Decision
**Azure**, in an EU region (West Europe or Sweden Central). Core services: Container Apps, Container Registry, PostgreSQL Flexible Server, Blob Storage + Queue, Key Vault, Application Insights / Log Analytics, Communication Services Email; later SignalR Service, Front Door, Entra External ID.

## Consequences
- First-class .NET tooling, docs and samples; less glue code.
- AWS skills are not practiced here; an optional later exercise can map the architecture to AWS equivalents.
