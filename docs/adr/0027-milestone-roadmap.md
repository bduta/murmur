# 0027. Milestone roadmap and sizing

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
~5–6 h/week. Motivation depends on shipping regularly.

## Decision
- Each milestone is **~15–20 h (~3–4 weeks)**, ends **deployed and demo-able**, spans backend + frontend + tests + infra, and adds one agentic asset.
- Order: M0a local skeleton → M0b deployed skeleton → M1 accounts → M2 text posts + prod → M3 follow + feed → M4 likes + comments → M5 photos → M6 notifications → M7 trust/safety/GDPR (**public beta**) → M8 DMs → M9 search + PWA → M10 AI features → M11 private accounts → M12 Entra + Google → M13a/b paid subscriptions.
- Details in [`../PLAN.md`](../PLAN.md#5-roadmap). Roadmap reviewed after every milestone.

## Rationale for ordering
- Deploy in M0b, not at the end.
- Notifications before messaging: learn SignalR on the easier problem.
- Trust & safety before DMs and before public launch.
- Photos after the core social loop: riskiest infra, lower priority than the loop.
- Paid content last: depends on stable identity (M12) and private-access patterns (M11).

## Consequences
- Public beta ≈ 8–10 months; paid content ≈ 16–18 months.
- If a milestone overruns 20 h, slice the next one smaller.
