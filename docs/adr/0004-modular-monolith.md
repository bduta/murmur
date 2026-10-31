# 0004. Modular monolith

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Microservices for a solo greenfield project bring distributed transactions, multiple pipelines, service discovery, tracing across services and multiplied cloud cost — before there is a single user.

## Decision
One deployable ASP.NET Core application split into strictly bounded modules: Identity, Profiles, Posts, Graph, Feed, Media, Notifications, Moderation, Messaging, Search, Billing. Modules own their data and interact only through public contracts and in-process events (ADR-0020). Boundaries are enforced by architecture tests.

A module is extracted into its own service only with a concrete reason (e.g. real-time load on Messaging), recorded in a new ADR.

## Consequences
- Simple deployment, debugging, transactions and cost.
- Same design skills (bounded contexts, contracts, events) as microservices.
- Discipline required: without arch tests, a monolith rots into a big ball of mud.

## Alternatives considered
- Microservices — operational cost far exceeds benefit at this scale.
- Unstructured monolith — fast at first, impossible to extract or reason about later.
