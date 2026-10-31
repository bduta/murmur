# 0002. Portfolio-grade project with room to grow

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
The project serves career growth (React/TS, Azure, Docker, CI/CD, agentic AI) and might become a real product. Time budget is ~5–6 h/week.

## Decision
Build a serious portfolio project with production habits — CI/CD, tests, observability, IaC, a real deployed URL — but do **not** design for scale we don't have. Keep module boundaries clean so scaling paths stay open.

## Consequences
- No sharding, queues-for-everything, caches or microservices up front.
- Every "scale" feature (Redis, CDN, fan-out on write, service extraction) is an optional later milestone justified by need.

## Alternatives considered
- Pure learning project — would skip production habits that matter for career value.
- Real product designed for scale from day one — months of infrastructure, no features, likely abandonment.
