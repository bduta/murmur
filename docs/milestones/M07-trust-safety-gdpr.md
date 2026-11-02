# M7 — Trust, safety, GDPR → 🚀 public beta

- **Goal:** the platform is safe and compliant enough to open sign-up to the public.
- **Ships:** block, report, admin moderation queue, rate limits, account deletion, data export, privacy policy + terms; invite-only gate removed at the end.
- **Main learning:** event-driven cross-module deletion, rate limiting, security headers.
- **Agentic asset:** security-review workflow on PRs.
- **Related ADRs:** 0023, 0016, 0020
- **Milestone acceptance criteria (draft):** the full checklist in ADR-0023, including:
  - `UserDeleted` event removes/anonymises data in every module, including blobs (integration-tested per module).
  - Data export (JSON + photos).
  - Block, report (post, comment, user), admin role + queue (remove content, suspend user).
  - Rate limits on sign-up, login, post, comment, follow; CSP/HSTS headers.
  - Privacy policy and terms published; 18+ confirmation at sign-up; essential cookies only.
- **Gate:** public sign-up opens only when every ADR-0023 item is done.
- **Out of scope:** AI moderation (M10).

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work. Likely to exceed 20 h — consider splitting into M7a/M7b when drafting.
