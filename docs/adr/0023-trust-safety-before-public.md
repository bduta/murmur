# 0023. Trust, safety and privacy before public launch

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Once strangers can sign up, the platform carries real responsibilities. GDPR applies to EU users even for a hobby project. Retrofitting deletion and moderation across modules is painful.

## Decision
Milestones before M7 are **invite-only**. Public sign-up opens only when all of the following ship (M7):

**Privacy**
- Account deletion: `UserDeleted` integration event; every module deletes/anonymises its data, including blobs.
- Data export (JSON + photos), contributed by each module.
- Privacy policy and terms (what is stored, no E2E encryption, EU hosting).
- Essential cookies only, no third-party trackers → no cookie banner.
- Minimum age **18+**.

**Safety**
- Block and report (post, comment, user, message).
- Admin role + moderation queue (remove content, suspend user).
- Rate limiting (ASP.NET Core rate limiter) on sign-up, login, post, comment, follow, DM.
- Email verification before posting; upload size and daily limits.

**Security**
- CSP, HSTS and other security headers; antiforgery.
- Server-side authorization on every resource, each rule covered by an integration test.
- Dependabot, CodeQL, vulnerable-package check in CI.

## Consequences
- AI moderation (M10) feeds the same queue.
