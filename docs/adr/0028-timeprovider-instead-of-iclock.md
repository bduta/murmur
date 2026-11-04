# 0028. TimeProvider instead of a custom IClock

- **Status:** Accepted
- **Date:** 2026-11-02
- **Amends:** [ADR-0020](0020-module-structure-vertical-slices-outbox.md) (SharedKernel contents)

## Context
ADR-0020 lists an `IClock` abstraction in SharedKernel so that code reading "now" can be tested. Since .NET 8 the base class library has shipped `System.TimeProvider` for this exact purpose: `GetUtcNow()`, `GetTimestamp()`, timers. It also has a test double, `FakeTimeProvider` (`Microsoft.Extensions.TimeProvider.Testing`), that can be set to a fixed time or advanced manually. ASP.NET Core Identity, data protection, rate limiting, `Task.Delay` and `CancellationTokenSource` already accept a `TimeProvider`.

## Decision
- No `IClock`. Code that needs the current time injects **`TimeProvider`** and calls `GetUtcNow()` (a `DateTimeOffset`, always UTC).
- The API host registers **one** instance: `builder.Services.AddSingleton(TimeProvider.System)`.
- **Never** call `DateTime.Now`, `DateTime.UtcNow` or `DateTimeOffset.UtcNow` directly in module code. An architecture test will enforce this once modules contain code.
- Integration tests replace the registration with a `FakeTimeProvider` through `WebApplicationFactory.ConfigureTestServices`, so time-dependent rules (token expiry, edit windows, rate limits) are tested deterministically. The testing package is added when the first test needs it.

## Consequences
- One abstraction instead of two, and it is the one the framework and libraries already understand. Identity token lifetimes and our own handlers move together when a test advances the clock.
- `TimeProvider` is an abstract class, not an interface. That's fine: it's built to be overridden, and the test double already exists.
- The ban on `DateTime.UtcNow` needs enforcement (an architecture test or analyzer); otherwise it gets forgotten.

## Alternatives considered
- Custom `IClock` (ADR-0020 as written): duplicates a BCL type, and framework components ignore it, so tests could end up running against two different clocks.
- `DateTime.UtcNow` directly: untestable time-dependent logic.
