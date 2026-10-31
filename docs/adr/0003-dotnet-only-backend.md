# 0003. .NET-only backend, TypeScript on the frontend

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Original idea was .NET **and** TypeScript on the backend. For a solo developer that means two toolchains, two ORMs, two test stacks, duplicated auth/validation and drifting contracts.

## Decision
The whole backend is ASP.NET Core on **.NET 10**. TypeScript is used in the React frontend. A second backend language is only introduced if a later milestone gives a concrete reason, via a new ADR.

## Consequences
- One build, one test stack, one deployable.
- TypeScript practice comes from the frontend and generated API client.
- Shared types are achieved via an OpenAPI-generated TS client (ADR-0011), not a shared language.

## Alternatives considered
- Node/TS service for real-time messaging — SignalR covers it first-class in .NET (ADR-0019).
