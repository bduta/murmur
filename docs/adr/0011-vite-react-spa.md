# 0011. Vite + React + TypeScript SPA

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Next.js would add a Node server (effectively a second backend, contradicting ADR-0003) and complicate same-domain cookie auth (ADR-0010). Most content is behind login, so SSR/SEO matters little.

## Decision
A **Vite + React + TypeScript** single-page app in `web/`, built to static files served by ASP.NET Core. Toolkit:
- React Router — routing
- TanStack Query — server state, infinite scroll, optimistic updates
- Tailwind CSS + shadcn/ui — styling and components
- React Hook Form + Zod — forms and validation
- OpenAPI-generated TS client (Orval or openapi-typescript) from the .NET OpenAPI document
- Vitest + React Testing Library + MSW; Playwright for E2E

## Consequences
- Frontend types never drift from backend DTOs.
- Link previews (Open Graph) for shared posts can be served by .NET later if needed.
