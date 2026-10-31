# 0010. ASP.NET Core Identity with cookie auth

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Need sign-up, login, email confirmation and password reset, secure in a browser SPA, with a likely later move to Entra External ID.

## Decision
- **ASP.NET Core Identity** in the Identity module, storing users in Postgres.
- Sessions via **HttpOnly, Secure, SameSite cookie**; SPA and API on the **same domain**; antiforgery (CSRF) protection.
- Email confirmation and password reset from M1.
- Google sign-in and optional 2FA in M12, alongside migration to **Entra External ID**.
- **All other modules reference users only by our own `UserId` (GUID)** — never Identity's internal IDs, emails or claims. On Entra migration, Identity maps Entra's subject ID to the existing `UserId`.

## Consequences
- No tokens in `localStorage`; XSS cannot steal sessions.
- No CORS configuration needed.
- Entra migration touches only the Identity module.

## Alternatives considered
- Entra External ID now — valuable learning, but heavy for M1; planned for M12.
- Auth0/Clerk — vendor lock-in and cost at scale.
- JWT in localStorage — vulnerable to XSS.
