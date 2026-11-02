# M11 — Private accounts

- **Goal:** users can choose who sees their posts.
- **Ships:** private-account setting, follow requests with approve/reject.
- **Main learning:** authorization rules that span feed, media, search and DMs.
- **Related ADRs:** 0005, 0017, 0018
- **Milestone acceptance criteria (draft):**
  - `follows` gains a status (`pending` / `accepted`); following a private account creates a request.
  - Private posts are hidden from non-approved users everywhere: profile, feed, search, media read URLs (integration-tested per surface).
  - Switching public → private and back behaves predictably for existing followers.
- **Out of scope:** close-friends lists, per-post audience.

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
