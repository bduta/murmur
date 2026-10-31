# 0018. Feed: fan-out on read

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Fan-out on write (precomputed per-user timelines) is needed only at very large scale and adds workers, backfills and the celebrity problem.

## Decision
- **Fan-out on read**: home feed = posts by people I follow, plus my own, newest first, in one SQL query.
- **Chronological** ordering; ranked feed is an optional later track.
- **Keyset (cursor) pagination** on `(created_at, id)`; never `OFFSET`. Cursor is opaque (base64) to the client.
- Indexes: `posts(author_id, created_at DESC, id DESC)`; `follows` PK `(follower_id, followee_id)` plus index on `(followee_id)`.
- **Counter columns** (`like_count`, `comment_count`) updated in the same transaction as the like/comment.
- Implemented behind `IFeedService` in the Feed module. This is the one documented cross-schema read query.

## Consequences
- Switching to fan-out on write (Redis + worker) later affects only the Feed module.
- Verify query plans with `EXPLAIN ANALYZE` in M3.
