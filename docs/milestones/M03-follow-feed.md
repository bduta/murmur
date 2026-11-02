# M3 — Follow + feed

- **Goal:** the core social loop — follow people and see their posts.
- **Ships:** follow/unfollow, follower and following lists, home feed with infinite scroll.
- **Main learning:** feed query design and indexes, `EXPLAIN ANALYZE`.
- **Agentic asset:** Playwright MCP so the agent can verify UI changes in a browser.
- **Related ADRs:** 0005, 0018
- **Milestone acceptance criteria (draft):**
  - `follows(follower_id, followee_id, created_at)` with PK + reverse index; follow is idempotent; can't follow yourself.
  - Feed = posts by followees + own posts, newest first, keyset-paginated, behind `IFeedService`.
  - Query plan verified with `EXPLAIN ANALYZE` on seeded data.
  - First Playwright journey: user A posts → user B follows A → B sees the post.
- **Out of scope:** ranked feed, fan-out on write, follow requests (M11).

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
