# 0019. Messaging and real-time with SignalR

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Messaging is a product in itself. Real-time delivery is needed for messages and notifications.

## Decision
**Scope (M8):** 1:1 conversations only; text messages; read receipts; conversation list with unread counts; only **mutual followers** can message; blocking stops messages. Later: media in messages, typing indicators, group chats, message requests.

**No end-to-end encryption.** Messages are encrypted in transit (TLS) and at rest (Azure default); the privacy policy states this honestly.

**Technology:**
- **ASP.NET Core SignalR** hub in-process while prod runs one replica; `@microsoft/signalr` client in React.
- Switch to **Azure SignalR Service** (free tier for demo, Standard when needed) when scaling beyond one replica — via config, behind `IRealtimePublisher`.
- Messages are **persisted first** (Postgres `messaging` schema), then pushed. SignalR is a nudge, never the source of truth; history via REST with keyset pagination.
- One connection carries both notifications (M6) and messages (M8).

## Consequences
- Notifications in M6 introduce SignalR on an easier problem first.
- Messaging is the most likely module to be extracted later.

## Alternatives considered
- Node/TS WebSocket service — second backend, contradicts ADR-0003.
- E2E encryption — specialist field; a half-done version is worse than none.
