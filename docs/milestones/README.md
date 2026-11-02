# Milestones

One file per milestone. The roadmap summary lives in [`../PLAN.md`](../PLAN.md#5-roadmap); these files hold the detail.

## Lifecycle of a milestone file

| Breakdown status | Meaning | Who moves it on |
|---|---|---|
| **Outline** | Goal, scope and acceptance criteria only, taken from PLAN.md and the ADRs. No task list yet. | Agent drafts a breakdown when the milestone is about to start |
| **Draft — awaiting approval** | Issue list proposed (title, who writes it, estimate, acceptance criteria). **No work starts and no GitHub issues are created.** | **User** approves or requests changes |
| **Approved** | Breakdown is agreed. GitHub milestone + issues may be created; work proceeds issue by issue. | Agent updates issue states as work lands |
| **Done** | All issues merged, deployed, milestone tag created, retro written in `../journal.md`. | Agent + user at milestone wrap-up |

Rules:
- A breakdown is **never** acted on until the user has explicitly approved it. Record the approval date in the file.
- Changes to an approved breakdown (adding, dropping or re-scoping an issue) also need the user's approval.
- Each milestone stays ~15–20 h (ADR-0027). If a draft exceeds that, split the milestone.
- `../STATUS.md` says which milestone is current and where the last session stopped; the milestone file owns the issue list and per-issue state.

## Index

| Milestone | File | Breakdown status |
|---|---|---|
| M0a — Local walking skeleton | [M0a-local-skeleton.md](M0a-local-skeleton.md) | Approved (2026-10-29) |
| M0b — Deployed walking skeleton | [M0b-deployed-skeleton.md](M0b-deployed-skeleton.md) | Outline |
| M1 — Accounts | [M01-accounts.md](M01-accounts.md) | Outline |
| M2 — Text posts + prod | [M02-text-posts-prod.md](M02-text-posts-prod.md) | Outline |
| M3 — Follow + feed | [M03-follow-feed.md](M03-follow-feed.md) | Outline |
| M4 — Likes + comments | [M04-likes-comments.md](M04-likes-comments.md) | Outline |
| M5 — Photos | [M05-photos.md](M05-photos.md) | Outline |
| M6 — Notifications | [M06-notifications.md](M06-notifications.md) | Outline |
| M7 — Trust, safety, GDPR → public beta | [M07-trust-safety-gdpr.md](M07-trust-safety-gdpr.md) | Outline |
| M8 — Direct messages | [M08-direct-messages.md](M08-direct-messages.md) | Outline |
| M9 — Search + PWA | [M09-search-pwa.md](M09-search-pwa.md) | Outline |
| M10 — AI features | [M10-ai-features.md](M10-ai-features.md) | Outline |
| M11 — Private accounts | [M11-private-accounts.md](M11-private-accounts.md) | Outline |
| M12 — Entra External ID + Google | [M12-entra-google.md](M12-entra-google.md) | Outline |
| M13a/b — Paid subscriptions | [M13-paid-subscriptions.md](M13-paid-subscriptions.md) | Outline |

## Template for a breakdown

```markdown
## Breakdown
- **Status:** Draft — awaiting approval | Approved (YYYY-MM-DD)
- **Total estimate:** ~N h

| # | Issue | Who writes it | ~h | State |
|---|---|---|---|---|

### Issue N — Title
- **Acceptance criteria:** …
- **Out of scope:** …
- **Notes:** (decisions or surprises discovered while doing it)
```
