# 0015. GitHub + GitHub Actions with OIDC

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Need source hosting, CI/CD and planning that maximise portfolio visibility and AI-agent integration.

## Decision
- Code on **GitHub**; planning in **GitHub Issues + Projects** (one milestone = one GitHub milestone).
- **GitHub Actions** pipelines:
  1. **PR checks** — build, unit + integration tests (Testcontainers), frontend lint/tests, CodeQL, `terraform plan` comment.
  2. **main → dev** — build image, push to ACR, `terraform apply` dev, run migrations, deploy, Playwright smoke.
  3. **tag → prod** — GitHub Environment approval, promote the **same image** (never rebuild), migrations, deploy, smoke.
- Azure auth via **OIDC workload identity federation** — no Azure secrets stored in GitHub.

## Consequences
- Best-supported platform for Claude Code (GitHub Action, `gh`, GitHub MCP).
- Azure DevOps skills not practiced; concepts transfer.
