# murmur — Project Plan

> Internal codename: **murmur** (from *murmuration*). Public brand and domain to be decided by **M2**.
> Decisions are recorded as ADRs in [`docs/adr/`](adr/). This plan is reviewed after every milestone.

---

## 1. Vision & goals

A social platform where people post text and photos, follow each other, like and comment, get notified, and message each other — later with private accounts and paid creator subscriptions.

**Primary goal:** a portfolio-grade project built with production habits that grows my skills in React/TypeScript, Azure, Docker, CI/CD and **agentic AI development**, while keeping the option to become a real product ([ADR-0002](adr/0002-goal-portfolio-grade-room-to-grow.md)).

**Constraints**

| Constraint | Value |
|---|---|
| Time | ~5–6 h/week (~20–25 h/month) |
| Strengths | Senior C#/.NET, intermediate SQL |
| Learning focus | React/TS (beg–int), Azure, Docker, CI/CD (beginner), agentic workflows |
| Budget | ~€25–40/month Azure |

**Guiding principles**
1. Ship a deployed, demo-able vertical slice every milestone (~15–20 h, ~3–4 weeks).
2. Don't design for scale we don't have — keep boundaries clean so we *can* scale.
3. Boring, managed infrastructure. Every new tool must earn its place.
4. Tests are the safety net for agent-written code.
5. Never merge code I couldn't explain in an interview.

---

## 2. Architecture

**Modular monolith** — one deployable ASP.NET Core app with strictly bounded modules ([ADR-0004](adr/0004-modular-monolith.md)).

```
                    ┌──────────────── Azure Container Apps (one container) ────────────────┐
 Browser (React SPA)│  ASP.NET Core host  (serves SPA from wwwroot + /api/* + /hubs/*)     │
  same-domain ─────►│  ┌──────────┐ ┌─────────┐ ┌───────┐ ┌───────┐ ┌──────┐ ┌───────┐     │
  cookie auth       │  │ Identity │ │Profiles │ │ Posts │ │ Graph │ │ Feed │ │ Media │ ... │
                    │  └────┬─────┘ └────┬────┘ └───┬───┘ └───┬───┘ └──┬───┘ └───┬───┘     │
                    │       └── in-process calls via *.Contracts + outbox events ──┘         │
                    │  Background workers: outbox dispatcher, media processor               │
                    └──────┬──────────────────────┬───────────────────────┬────────────────┘
                           │                      │                       │
                 PostgreSQL Flexible       Blob Storage + Queue       Key Vault, App Insights,
                 (schema per module)       (private containers)       ACS Email
```

### Modules

| Module | Owns | Arrives |
|---|---|---|
| **Identity** | Users, credentials, sessions, email verification, external logins | M1 |
| **Profiles** | Display name, bio, avatar | M1 |
| **Posts** | Posts, comments, likes, counters | M2/M4 |
| **Graph** | Follows, blocks, (later) follow requests | M3 |
| **Feed** | Home timeline query (`IFeedService`) | M3 |
| **Media** | Upload SAS, processing, read URLs | M5 |
| **Notifications** | In-app notifications, real-time push | M6 |
| **Moderation** | Reports, admin queue, (later) AI flags | M7 |
| **Messaging** | Conversations, messages, read receipts | M8 |
| **Search** | Postgres full-text over users/posts/hashtags | M9 |
| **Billing** | Creator plans, subscriptions, Stripe Connect | M13 |

### Rules ([ADR-0020](adr/0020-module-structure-vertical-slices-outbox.md))
- One project per module + a public `*.Contracts` project; everything else `internal`.
- Vertical slices inside a module (`CreatePost/` = endpoint + request + validator + handler + test).
- One `DbContext` and Postgres **schema** per module; no cross-schema joins in code except the documented Feed read query.
- Cross-module side effects via **integration events** through a hand-rolled **transactional outbox**.
- Modules reference users only by our own **`UserId` (GUID)** ([ADR-0010](adr/0010-aspnet-identity-cookie-auth.md)).
- Architecture tests enforce all of the above in CI.

### Key designs
- **Feed:** fan-out on read, chronological, keyset cursor pagination, counter columns ([ADR-0018](adr/0018-feed-fan-out-on-read.md)).
- **Media:** direct-to-Blob upload with write SAS → queue → worker (validate, strip EXIF, resize, WebP via SkiaSharp) → private container → read SAS ([ADR-0017](adr/0017-media-pipeline.md)).
- **Real-time:** SignalR in-process (one replica) → Azure SignalR Service when scaling out; one connection for notifications and messages ([ADR-0019](adr/0019-messaging-signalr.md)).
- **Messaging:** 1:1 between mutual followers, persisted first then pushed, no E2E encryption.

---

## 3. Tech stack

| Area | Choice | ADR |
|---|---|---|
| Backend | .NET 10, ASP.NET Core Minimal APIs, FluentValidation, ProblemDetails | 0003, 0020 |
| Data | PostgreSQL (Azure Flexible Server), EF Core + Npgsql, schema per module | 0009 |
| Auth | ASP.NET Core Identity, HttpOnly cookie, antiforgery; later Google + Entra External ID | 0010 |
| Frontend | Vite + React + TypeScript, React Router, TanStack Query, Tailwind + shadcn/ui, React Hook Form + Zod, OpenAPI-generated client | 0011 |
| Real-time | SignalR (`@microsoft/signalr` on client) | 0019 |
| Images | SkiaSharp | 0017 |
| Local dev | .NET Aspire (Postgres, Azurite, API, Vite), Aspire dashboard | 0013 |
| Hosting | Single Docker image on Azure Container Apps, Azure Container Registry | 0012 |
| IaC | Terraform (azurerm, azapi as needed), remote state in Azure Storage | 0014 |
| CI/CD | GitHub Actions, OIDC federation to Azure, GitHub Environments | 0015 |
| Secrets | Key Vault + managed identity; user-secrets locally | 0016 |
| Observability | OpenTelemetry → Application Insights (sampling + daily cap) | 0013, 0024 |
| Email | Azure Communication Services Email (or free-tier provider) — final pick in M1 | 0024 |
| Testing | xUnit v3, Testcontainers, Respawn, Shouldly, NetArchTest, Vitest, RTL, MSW, Playwright | 0021 |
| Feature flags | Microsoft.FeatureManagement (config-based) | 0025 |
| Planning | GitHub Issues + Projects | 0015 |

**Avoided on purpose:** microservices, Kubernetes/AKS, Next.js, a second backend language, Cosmos DB/graph DBs, JWT in localStorage, MediatR/AutoMapper/MassTransit/FluentAssertions (commercial licenses), ImageSharp (split license), CDN/Redis/Front Door until needed.

---

## 4. Environments & cost

| | dev | prod |
|---|---|---|
| Trigger | merge to `main` | tag `vX.Y.Z` + manual approval |
| Container Apps | scale to zero | 1 always-on replica |
| Database | `murmur_dev` DB | `murmur_prod` DB (same server, separate credentials) |
| Region | EU (West Europe or Sweden Central) | same |

**Target: ~€25–40/month** ([ADR-0024](adr/0024-cost-budget.md)). Azure budget alerts at 50/80/100%, Log Analytics daily cap, App Insights sampling, consistent `env`/`project` tags. Expensive services (Front Door, SignalR Standard, Redis, second DB server) are added only when a milestone needs them, with cost stated up front.

Naming: `rg-murmur-{env}`, `murmur{env}st`, `murmuracr`, `ca-murmur-{env}`, `psql-murmur`, `kv-murmur-{env}`.

---

## 5. Roadmap

Each milestone: ~15–20 h, ends **deployed**, includes backend + frontend + tests + infra, and adds **one agentic asset**.

| # | Milestone | Ships | Main learning | Agentic asset |
|---|---|---|---|---|
| **M0a** | Local walking skeleton | React app calls `/api/health` | Aspire, module layout, arch tests, Vite + TS | `CLAUDE.md`, ADRs in repo |
| **M0b** | Deployed walking skeleton | Live on `dev` URL | **Hand-written** Dockerfile, Terraform bootstrap + dev, GitHub Actions + OIDC | Hook blocking edits to secrets/state |
| **M1** | Accounts | Sign up, verify email, login/logout, reset password, basic profile, invite-only | Identity + cookies, email, Key Vault, managed identity | `/new-slice` skill |
| **M2** | Text posts + prod | Create/delete text posts, profile posts, **first prod deploy** | Outbox, keyset pagination, TanStack Query, prod env | Claude Code PR review action |
| **M3** | Follow + feed | Follow/unfollow, follower lists, infinite-scroll feed | Feed query & indexes, `EXPLAIN ANALYZE` | Playwright MCP |
| **M4** | Likes + comments | Optimistic likes, comments, counters | Counter concurrency, optimistic UI | `/migration` skill |
| **M5** | Photos | Photo posts, avatars | SAS, queue worker, SkiaSharp, EXIF stripping | `/new-module` skill |
| **M6** | Notifications | Real-time in-app notifications | SignalR, event consumers | Agent debugging with App Insights |
| **M7** | Trust, safety, GDPR → **🚀 public beta** | Block, report, admin queue, rate limits, delete account, export data, policies | Event-driven deletion, rate limiting, security headers | Security-review PR workflow |
| **M8** | Direct messages | 1:1 DMs (mutuals), read receipts, unread counts | SignalR in depth, conversation modelling | — |
| **M9** | Search + PWA | Search users/posts/hashtags; installable | Postgres FTS, service workers | — |
| **M10** | AI features | AI moderation (text + images) into queue, auto alt-text | LLM APIs, cost/latency limits, evals | Prompt eval harness |
| **M11** | Private accounts | Follow requests, approve/reject | Authorization across feed/media/DMs | — |
| **M12** | Entra External ID + Google | Social login, user migration | OIDC, identity migration | — |
| **M13a/b** | Paid subscriptions | Creator pricing, fan subscriptions, paywalled posts | Stripe Connect, webhooks, idempotency, signed media | — |

**Optional tracks:** ranked "For you" feed · fan-out on write with Redis · Front Door/CDN · extract Messaging service · group chats · React Native app · port of IaC concepts to AWS.

**Indicative timeline:** public beta (M7) ≈ 8–10 months; paid content ≈ 16–18 months.

**Gates**
- Before M7 ends: invite-only. M7 checklist must be complete before public sign-up opens ([ADR-0023](adr/0023-trust-safety-before-public.md)).
- Before M13: split dev/prod database servers, trademark check on public name, Stripe Connect terms reviewed.

---

## 6. Ways of working

### Per-feature loop ([ADR-0022](adr/0022-agent-workflow.md))
1. **Issue** — user story, acceptance criteria (I own them), out-of-scope.
2. **Plan** — agent in plan mode proposes approach; I approve/correct.
3. **Tests first** — agent writes integration tests from acceptance criteria; I review the tests.
4. **Implement** until tests, arch tests and lint are green.
5. **PR** linked to issue, small (<~400 lines).
6. **CI** — build, tests, CodeQL, `terraform plan` comment.
7. **AI review** on the PR.
8. **I merge** → auto-deploy to dev → Playwright smoke.

### "I write the first one" zones
First Dockerfile, first Terraform module, first GitHub Actions workflow, first React components — agent as tutor, not author.

### Git & releases ([ADR-0025](adr/0025-trunk-based-development.md))
Trunk-based, short-lived branches, squash merge, Conventional Commits, branch protection on `main`, tag-triggered prod releases, milestone tags (`m3-follow-feed`), config feature flags, backward-compatible migrations run by the pipeline (expand → migrate → contract).

### Testing ([ADR-0021](adr/0021-integration-first-testing.md))
Integration-first against real Postgres (Testcontainers); thin unit layer; arch tests; Vitest + MSW; 5–10 Playwright journeys doubling as smoke tests. No coverage target — every acceptance criterion has a test.

### Learning journal
`docs/journal.md` — per milestone: what the agent did well, where it went wrong, what I changed (prompt, skill, guardrail). ~15 min/milestone.

### Milestone retro (end of each milestone)
- Is it deployed and demo-able?
- Did it fit 15–20 h? If not, slice smaller next time.
- Update this plan, ADRs and `CLAUDE.md`.

---

## 7. Decisions deferred to their milestone
- Email provider final pick (M1)
- Public name + domain (by M2)
- Notification types and grouping (M6)
- Search ranking and hashtag model (M9)
- LLM provider/model and moderation thresholds (M10)
- Subscription model: per-creator vs platform-wide, pricing, payouts (M13)
