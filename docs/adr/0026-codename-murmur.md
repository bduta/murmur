# 0026. Internal codename `murmur`

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
Names propagate into namespaces, the repo and Azure resources — some of which (Storage accounts, Container Registries) cannot be renamed.

## Decision
- Permanent **internal codename: `murmur`** (from *murmuration*).
- Used for: repo name, `Murmur.*` namespaces, Azure resources (`rg-murmur-{env}`, `murmur{env}st`, `murmuracr`, `ca-murmur-{env}`, `psql-murmur`, `kv-murmur-{env}`).
- **Public brand and domain** are separate, decided **by M2**; they affect only domain, UI title and email sender.
- Trademark check (EUIPO / TMview) on the public name before paid content (M13).
