# M5 — Photos

- **Goal:** posts can contain photos; users have avatars.
- **Ships:** photo posts, avatars.
- **Main learning:** SAS tokens, Storage Queue worker, SkiaSharp processing, EXIF stripping.
- **Agentic asset:** `/new-module` skill.
- **Related ADRs:** 0017, 0012, 0024
- **Milestone acceptance criteria (draft):**
  - Direct-to-Blob upload with short-lived write SAS; API never streams the file.
  - Worker validates magic bytes, strips EXIF (incl. GPS), fixes orientation, produces resized WebP + thumbnail; deletes the original.
  - All containers private; images served via short-lived read SAS.
  - Size, type and daily upload limits enforced; post visible only when media is `ready`.
- **Out of scope:** video, CDN/Front Door, AI moderation (M10).

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
