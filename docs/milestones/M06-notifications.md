# M6 — Notifications

- **Goal:** users find out in real time when someone interacts with them.
- **Ships:** in-app notifications for likes, comments and new followers, pushed live.
- **Main learning:** SignalR, consuming integration events from other modules.
- **Agentic asset:** agent-assisted debugging using App Insights traces and logs.
- **Related ADRs:** 0019, 0020
- **Milestone acceptance criteria (draft):**
  - Notifications module consumes outbox events idempotently.
  - Notification list (keyset-paginated) with unread count and mark-as-read.
  - SignalR hub pushes new notifications to connected clients behind `IRealtimePublisher`; works when the user is offline (shows on next load).
- **Decision to make at start:** notification types, grouping ("X and 3 others liked…"), retention.
- **Out of scope:** email/web push notifications (web push with PWA in M9).

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
