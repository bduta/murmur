# 0007. Agentic development first, AI product features later

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
"Agentic AI skills" can mean (a) building *with* AI coding agents, or (b) building AI features *into* the product.

## Decision
- **(a) from day one:** every milestone is built with AI coding agents using the workflow in ADR-0022, and adds at least one reusable agentic asset (skill, hook, MCP integration, review workflow).
- **(b) later:** dedicated milestone M10 — AI moderation of text and images into the moderation queue, and auto alt-text. Further AI features (feed summaries, recommendations, semantic search) are optional.

## Consequences
- Designs leave hooks for (b): the media worker step (ADR-0017) and moderation queue (ADR-0023).
- Product features using LLMs must include cost caps, timeouts, fallbacks and an eval harness.
