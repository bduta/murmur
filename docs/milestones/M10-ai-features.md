# M10 — AI features

- **Goal:** AI makes the platform safer and more accessible.
- **Ships:** AI moderation of text and images feeding the moderation queue; automatic alt-text for photos.
- **Main learning:** LLM APIs in production — cost caps, timeouts, fallbacks, evaluation.
- **Agentic asset:** prompt eval harness.
- **Related ADRs:** 0007, 0017, 0023
- **Milestone acceptance criteria (draft):**
  - Media worker step calls moderation + alt-text; failures or timeouts never block posting (fallback to human queue).
  - Text moderation on posts and comments flags into the existing queue (no auto-delete without review, initially).
  - Per-day cost cap and metrics in App Insights.
  - Eval set of labelled examples with a repeatable score.
- **Decision to make at start:** LLM provider/model, moderation thresholds.
- **Out of scope:** feed summaries, recommendations (optional tracks).

## Breakdown
- **Status:** Outline — not broken down yet. Needs the user's approval before any work.
