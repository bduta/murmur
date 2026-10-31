# 0005. Follow-only social graph

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Both "follow" (one-way) and "connect" (mutual with approval) were considered. Supporting both doubles graph complexity and confuses users about visibility.

## Decision
1. **Follow-only** (one-way, instant) in the early milestones.
2. **Private accounts with follow requests** later (M11): same `follows` table plus a status column.
3. **Paid subscriptions** to creators' content later (M13), as a separate Billing concern — not a graph relationship type.

No separate "connection" model unless a real product reason appears.

## Consequences
- Simple `follows(follower_id, followee_id, created_at)` table.
- "Mutual follow" is derived, and used as the DM permission rule (ADR-0019).
- Paid subscriptions make the platform a marketplace (Stripe Connect, KYC, payouts, tax, refunds) — deliberately last.
