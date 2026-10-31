# 0009. PostgreSQL + EF Core, schema per module

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Social data (users, posts, follows, likes, comments) is relational. Options were PostgreSQL, Azure SQL, Cosmos DB, or a graph DB.

## Decision
**PostgreSQL** on Azure Database for PostgreSQL **Flexible Server**, accessed via **EF Core + Npgsql**. Each module has its **own schema** (`identity`, `profiles`, `posts`, `graph`, `messaging`, ...) and its own `DbContext` and migrations. Locally Postgres runs in a container via Aspire.

## Consequences
- JSONB, built-in full-text search (M9) and **pgvector** (AI features) available without new services.
- Portable across clouds; strong job-market value.
- Some SQL Server habits need relearning (case sensitivity, `ILIKE`, identity vs sequences).

## Alternatives considered
- Azure SQL — familiar, but less portable and fewer extras.
- Cosmos DB — awkward for joins/counts; RU pricing easy to get wrong.
- Graph DB for follows — unnecessary; an indexed `follows` table handles 1-hop queries.
