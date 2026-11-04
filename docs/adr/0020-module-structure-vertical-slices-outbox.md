# 0020. Module structure, vertical slices, outbox

- **Status:** Accepted (clock amended by [ADR-0028](0028-timeprovider-instead-of-iclock.md); shared projects, cross-module reads and events detailed by [ADR-0029](0029-module-boundaries.md))
- **Date:** 2026-10-29

## Context
The modular monolith (ADR-0004) needs a concrete, enforceable structure that agents can follow.

## Decision
**Solution layout**
```
src/
  Murmur.AppHost/              Aspire orchestration (local only)
  Murmur.ServiceDefaults/      OpenTelemetry, health checks, resilience
  Murmur.Api/                  host: Program.cs, auth, module registration, serves SPA
  Modules/<Name>/
    Murmur.<Name>/             internal implementation
    Murmur.<Name>.Contracts/   public interfaces, DTOs, integration events
  Shared/Murmur.SharedKernel/  tiny, base .NET only: Result, UserId, event base types (ADR-0029; time: TimeProvider, ADR-0028)
  Shared/Murmur.BuildingBlocks/ generic plumbing for modules + host: IModule, later outbox etc. (ADR-0029)
web/                           React SPA
infra/                         Terraform
tests/
```

**Rules**
1. Inside a module: **vertical slices** — one folder per feature (`CreatePost/`: endpoint, request, validator, handler).
2. Modules reference each other **only via `.Contracts`**; everything else `internal`.
3. **Architecture tests** (NetArchTest / ArchUnitNET) enforce rule 2 and forbid cross-module `DbContext` use.
4. **One `DbContext` + schema + migration set per module.**
5. Cross-module side effects via **integration events** written to a **transactional outbox** in the same transaction; a background dispatcher delivers them to in-process handlers. Handlers must be idempotent.
6. **Minimal APIs** grouped per module, **FluentValidation**, errors as **ProblemDetails**.
7. Handlers are plain classes injected into endpoints — **no mediator library**.
8. Mapping by hand or **Mapperly**.

**Do not add** (commercial licences): MediatR, AutoMapper, MassTransit, FluentAssertions. Wolverine (MIT) is acceptable only via a new ADR.

## Consequences
- Agents can scaffold slices consistently (`/new-slice` skill, M1).
- Outbox maps 1:1 onto a real broker if a module is extracted.
