# M9 — Search + PWA

- **Goal:** people can find users and content, and install the app.
- **Ships:** search for users, posts and hashtags; installable PWA with web push.
- **Main learning:** Postgres full-text search, service workers.
- **Related ADRs:** 0008, 0009
- **Milestone acceptance criteria (draft):**
  - Search module with Postgres FTS (`tsvector` + GIN index); respects blocks and (later) private accounts.
  - Hashtags extracted from posts and searchable.
  - Web app manifest + service worker; installable on Android/desktop; web push for notifications (opt-in).
- **Decision to make at start:** search ranking and hashtag model.
- **Out of scope:** semantic/vector search (optional AI track).

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
