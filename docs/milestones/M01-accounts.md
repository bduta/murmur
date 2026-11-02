# M1 — Accounts

- **Goal:** people can create an account and sign in; the platform is invite-only.
- **Ships:** sign up, email verification, login/logout, password reset, basic profile (display name, bio), invite-only gate.
- **Main learning:** ASP.NET Core Identity with cookie auth, transactional email, Key Vault + managed identity.
- **Agentic asset:** `/new-slice` skill (scaffold endpoint + handler + validator + test in a module).
- **Related ADRs:** 0010, 0016, 0020, 0021, 0023
- **Milestone acceptance criteria (draft):**
  - HttpOnly, Secure, SameSite cookie session; antiforgery on state-changing requests; no tokens in `localStorage`.
  - Other modules see only `UserId` (GUID), never Identity's internals.
  - Email confirmation required before posting; password reset works end to end.
  - Sign-up requires an invite code (or allow-list) until M7.
  - Rate limiting on sign-up and login.
- **Decision to make at start:** email provider (Azure Communication Services Email vs a free-tier provider).
- **Out of scope:** Google sign-in, 2FA, Entra (M12).

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
