# 0017. Media upload and processing pipeline

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Photos must be uploaded efficiently, made safe (no GPS leaks, real images only) and protected, since private accounts (M11) and paid content (M13) are planned.

## Decision
```
1. SPA  → POST /api/media/uploads   API validates auth, size, type, quota → returns short-lived write-only SAS for "incoming/{id}"
2. SPA  → PUT file directly to Blob Storage (API never handles the bytes)
3. SPA  → POST /api/posts {mediaId}  media marked "pending"
4. Worker (hosted service, Azure Storage Queue):
     validate magic bytes → strip EXIF → fix orientation → resize (e.g. 1080px + 320px thumb) → WebP
     → write to private "media" container → delete original → mark "ready"
5. API returns short-lived read SAS URLs when serving posts
```
- **All containers private** from day one.
- Image library: **SkiaSharp** (MIT).
- No CDN initially (Front Door later if needed).
- Step 4 is the hook for AI moderation and alt-text (M10).

## Consequences
- Small API container; uploads scale independently.
- Read SAS URLs complicate caching; acceptable until a CDN milestone.
- Worker can move to a Container Apps Job without code changes beyond hosting.

## Alternatives considered
- ImageSharp — split license requires a paid licence above a revenue threshold; conflicts with paid-content plans.
- Public containers — would break every URL when private/paid content arrives.
