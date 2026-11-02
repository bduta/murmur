# M8 — Direct messages

- **Goal:** mutual followers can chat one-to-one in real time.
- **Ships:** 1:1 DMs between mutual followers, read receipts, conversation list with unread counts.
- **Main learning:** SignalR in depth, conversation modelling.
- **Related ADRs:** 0019, 0005, 0023
- **Milestone acceptance criteria (draft):**
  - Only mutual followers can start or continue a conversation; blocking stops messages (integration-tested).
  - Messages persisted first, then pushed; history via REST with keyset pagination.
  - Read receipts and unread counts; report message feeds the M7 moderation queue.
  - Messages rate-limited.
- **Out of scope:** group chats, media in messages, typing indicators, message requests, E2E encryption.

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
