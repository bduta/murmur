# 0029. Module boundaries: shared code, cross-module reads, events

- **Status:** Accepted
- **Date:** 2026-11-02
- **Amends:** [ADR-0020](0020-module-structure-vertical-slices-outbox.md)

## Context
ADR-0020 sets the module layout and says modules talk only through `.Contracts` and integration events. While building the first modules (M0a issue 3), several questions came up that it doesn't answer: where framework-dependent shared code lives, how a module *reads* another module's data, what events carry, and which exceptions the "everything internal" rule needs. These rules were worked out in a design review ("grilling") session on 2026-11-02 and are written here so agents can apply them without re-deriving them.

## Decision

### 1. Shared projects and dependency direction
```
Murmur.Api ──► Murmur.<Name> ──► <Name>.Contracts, Other.Contracts ──► SharedKernel
     └──────────────┴──────────► BuildingBlocks ───────────────────► SharedKernel
SharedKernel ──► base .NET only
```

**`src/Shared/Murmur.SharedKernel`**: a type is admitted only if it is
1. needed by at least two modules' **contracts**;
2. stable;
3. limited in behaviour to its **own invariants** (construction, validation of its own shape, parsing/formatting, equality). No rule that belongs to a specific module's domain. Test: *if product requirements changed this rule, which module would own the change?* If one module would, the code goes there.
4. free of **runtime** dependencies beyond the .NET base framework. Compile-time-only packages (analyzers, source generators with `PrivateAssets="all"`) are allowed. Framework adapters for kernel types (EF converters, OpenAPI tweaks) live outside the kernel.

Today it holds `UserId`, `Result`/`Result<T>`/`Error`, and later the integration-event base type.

**`src/Shared/Murmur.BuildingBlocks`**: generic plumbing for module implementations and the host (`IModule`; later the `Error` → ProblemDetails mapping, the `UserId` EF converter and the outbox).
- No module references and no domain vocabulary.
- Admitted when used by **≥ 2 modules**, or when an accepted ADR requires it of **every** module (e.g. the outbox, ADR-0020 rule 5).
- Host-only code stays in `Murmur.Api`; single-module code stays in its module.
- Framework dependencies allowed. Never referenced by SharedKernel or any `.Contracts` project.

### 2. References and what crosses a boundary
- A module implementation `Murmur.<Name>` may be referenced only by `Murmur.Api` and test projects. Everything else references at most `Murmur.<Name>.Contracts`.
- `.Contracts` projects reference only SharedKernel.
- Allowed across a boundary: contract DTOs (`sealed record`, no behaviour), integration events, kernel types, primitives/BCL types, enums defined in the contract.
- Never across a boundary: entities, `DbContext`s, `IQueryable`, EF or ASP.NET Core types, internal enums or value objects.
- A module's HTTP request/response types are **separate** from its contract DTOs, even when they look alike.
- The owning module maps entities to contract DTOs inside its implementation.
- Renaming or removing a contract member is a breaking change for consumers. Adding is safe.
- `UserId` lives in SharedKernel. Other strongly typed IDs (`PostId`, ...) live in their owning module's `.Contracts`.

### 3. Reading another module's data
1. **Default: query interfaces in `.Contracts`** (e.g. `IProfileLookup`), implemented inside the owning module and called in-process.
   - Batch-shaped: lists of IDs in, dictionary out.
   - `async` with a `CancellationToken`.
   - Lookups return nullable DTOs or dictionaries (missing = absent). `Result<T>` only when the caller must handle an expected failure reason.
2. **When traces show a read is hot:** cache behind the same interface (e.g. HybridCache with a short expiry, cleared by the owner's events).
3. **Local copies kept up to date by events** only with a written reason in the PR: a measured problem caching didn't fix, or a module we plan to extract. The reason must cover backfill, versioning/ordering and GDPR deletion.
4. **Visibility, authorization and privacy reads** (blocks, private accounts, follow checks) always go to the owner, fresh, and are never cached past the request.
5. **Cross-schema SQL** only for the documented Feed read (ADR-0018), against a read-only view published by the owning module.

### 4. Integration events
- Default shape: **IDs plus the facts of the change**, e.g. `PostCreated(PostId, AuthorId, CreatedAt)`.
- No personal data, secrets or content bodies (post text, message bodies); refer to them by ID.
- Every event has an `EventId` (idempotent handlers) and an `OccurredAt` from `TimeProvider` (ADR-0028).
- Full-state payloads only to feed a justified local copy, and then they carry a `Version`.

### 5. No cycles
Synchronous dependencies between modules form a one-way graph. When the reverse direction needs data, it uses an event. Expected layering: Identity/Profiles/Graph → Posts/Media → Feed/Notifications/Search.

### 6. Visibility
In a module implementation, only the `IModule` class is `public`. The one exemption: EF Core `Migration` and `ModelSnapshot` subclasses, because the generator emits them as public. Any further exemption requires updating this ADR.

### 7. Enforcement
`tests/Murmur.ArchitectureTests` (ArchUnitNET, `TngTech.ArchUnitNET.xUnitV3`) plus a hand-written `.csproj` reference test check:
- SharedKernel references base .NET only;
- `.Contracts` reference neither BuildingBlocks nor framework types;
- BuildingBlocks references no module;
- implementations are referenced only by `Murmur.Api` and test projects;
- there are no cycles between modules;
- the visibility rule in §6 holds;
- modules don't call `DateTime.UtcNow`/`DateTimeOffset.UtcNow` (ADR-0028).

Integration tests live in `tests/Murmur.Api.IntegrationTests` (HTTP only, no `InternalsVisibleTo`). Module unit test projects (`tests/Modules/Murmur.<Name>.UnitTests`) are created on demand, and only they may receive `InternalsVisibleTo`.

## Consequences
- Contracts stay small, framework-free and impossible to leak entities through.
- Most rules are compile-time or test-enforced; the judgement calls (kernel admission, local copies) have written criteria that PR review checks.
- Reads cost one extra batched query per page compared with local copies. That's accepted until traces say otherwise.
- One more shared project to keep disciplined; its admission rule mirrors the kernel's.

## Alternatives considered
- `IModule` in SharedKernel, guarded by a test: leaves ASP.NET Core reachable from every contract.
- Separate `Modules.Abstractions`/`Persistence`/`Web` projects now: three projects before two have code. Split BuildingBlocks later if it grows.
- Local copies by default: duplicated data, backfill, ordering and GDPR deletion in every consumer, for a ~1 ms per page saving.
- Full-state events by default: wide, frequently changing contracts that carry personal data into the outbox and logs.
- `UserId` in `Identity.Contracts`: makes the authentication module a hub every module depends on.
- NetArchTest: works on one assembly at a time; the original package is barely maintained.
