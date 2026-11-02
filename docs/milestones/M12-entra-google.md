# M12 — Entra External ID + Google sign-in

- **Goal:** move identity to a managed provider and add social login without disrupting existing users.
- **Ships:** Google sign-in, migration of existing users to Entra External ID, optional 2FA.
- **Main learning:** OIDC/OAuth2, identity migration.
- **Related ADRs:** 0010
- **Milestone acceptance criteria (draft):**
  - Only the Identity module changes; Entra subject ID maps to the existing `UserId`.
  - Existing users can sign in after migration (tested migration path).
  - Cookie session model for the SPA unchanged.
- **Out of scope:** other social providers.

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
