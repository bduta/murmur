# M13a/b — Paid subscriptions

- **Goal:** fans can pay creators for access to subscriber-only posts.
- **Ships:** creators set a price, fans subscribe, paywalled posts; planned as two milestones (M13a, M13b).
- **Main learning:** Stripe Connect, webhooks, idempotency, signed media access.
- **Related ADRs:** 0005, 0017, 0024, 0026
- **Milestone acceptance criteria (draft):**
  - Billing module; Stripe Connect onboarding for creators (KYC handled by Stripe).
  - Subscription lifecycle driven by idempotent webhook handling (created, renewed, failed, cancelled, refunded).
  - Paywalled posts and media inaccessible without an active subscription, including direct media URLs.
- **Gates before starting:** split dev/prod database servers (ADR-0024); trademark check on public name (ADR-0026); review Stripe Connect terms, tax/VAT handling and refund policy.
- **Decisions to make at start:** per-creator vs platform-wide subscription, pricing, platform fee, payouts.

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
