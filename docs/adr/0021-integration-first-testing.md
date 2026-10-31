# 0021. Integration-first testing

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
AI agents write much of the code; tests are the main way to verify it without reading every line. Mocked databases miss the bugs that matter (indexes, transactions, keyset comparisons, outbox).

## Decision
| Layer | Tools | Scope |
|---|---|---|
| Integration (most tests) | xUnit v3, `WebApplicationFactory`, **Testcontainers** (Postgres, Azurite), Respawn | HTTP → endpoint → DB → response, incl. every permission rule |
| Unit | xUnit, Shouldly | Pure logic only |
| Architecture | NetArchTest / ArchUnitNET | Module boundaries |
| Frontend | Vitest, React Testing Library, MSW | Components/hooks with behaviour |
| E2E | Playwright | 5–10 critical journeys, also post-deploy smoke tests |

- **Test-first per slice**: tests are written from the issue's acceptance criteria and reviewed by me before implementation.
- **No coverage percentage target.** Every acceptance criterion has a test.
- No EF Core InMemory or SQLite substitutes.

## Consequences
- Docker required locally and in CI (available on GitHub-hosted runners).
