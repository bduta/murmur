# M4 — Likes + comments

- **Goal:** people can react to and discuss posts.
- **Ships:** like/unlike with optimistic UI, comments on posts, like and comment counters.
- **Main learning:** counter concurrency, optimistic updates with TanStack Query.
- **Agentic asset:** `/migration` skill.
- **Related ADRs:** 0018, 0020
- **Milestone acceptance criteria (draft):**
  - Like is idempotent (unique `(post_id, user_id)`); unlike likewise.
  - `like_count` / `comment_count` updated in the same transaction as the like/comment; correct under concurrent likes (integration-tested).
  - Comments paginated by keyset; author can delete own comment; post author can delete comments on their post.
  - UI rolls back an optimistic like when the request fails.
- **Out of scope:** comment threads/replies, reactions beyond "like".

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
