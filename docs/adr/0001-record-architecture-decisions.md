# 0001. Record architecture decisions

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
The project is built by one developer working with AI coding agents over many months. Agents have no memory of past discussions and will "helpfully" reintroduce rejected choices unless decisions are written down where they read.

## Decision
Record every significant decision as a short ADR in `docs/adr/`. `CLAUDE.md` points agents to this folder. Changing a decision means a new ADR that supersedes the old one.

## Consequences
- Agents and future-me can see *why*, not just *what*.
- Doubles as interview material.
- Small overhead per decision (~10 minutes).
