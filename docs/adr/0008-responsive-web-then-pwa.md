# 0008. Responsive web first, PWA later

- **Status:** Accepted
- **Date:** 2026-10-29

## Context
A native or cross-platform mobile app roughly doubles frontend work and adds store review, push setup and separate releases. A URL is the best portfolio artifact.

## Decision
Mobile-first **responsive React web app**. Make it a **PWA** in M9 (installable, web push). React Native (Expo) is an optional final track.

## Consequences
- Design every screen mobile-first.
- The REST API stays client-agnostic so a mobile client can reuse it later.
