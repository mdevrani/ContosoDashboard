# ContosoDashboard Constitution

<!--
Sync Impact Report
- Version change: N/A → 1.0.0
- Modified principles: Initial project governance definition
- Added sections: Core Principles, Additional Standards, Development Workflow
- Removed sections: N/A
- Follow-up TODOs: none
-->

## Core Principles

### I. Security-First Delivery
All features, data access, and infrastructure changes must preserve the training application's security model: authenticated access, role-based authorization, and service-level protection against unauthorized object access. Security cannot be deferred to a later milestone; it is required at design time and verified before merge.

### II. Offline-First and Portable Architecture
The application must remain functional without cloud dependencies for training purposes. Local storage, local data files, and offline-compatible patterns are the default unless a feature explicitly requires external services. Any infrastructure dependency must be abstracted behind interfaces or configuration boundaries to permit future migration.

### III. Quality Through Verification
Every functional change must be validated with the smallest relevant proof: build, targeted tests, or direct execution checks. Features may not be considered done until the relevant verification output confirms behavior. This includes .NET build health, database startup, and user-facing flows.

### IV. Incremental, User-Value-First Delivery
Work is organized into small, independently testable increments that deliver value to users. The team must prefer the smallest implementation that satisfies the requirement, avoiding speculative features or large rewrites that do not improve current delivery.

### V. Clear, Maintainable Code
Code must be readable, explicit, and consistent with the existing Blazor Server and service-based architecture. Naming, structure, and responsibilities must reflect the current patterns in the project so that changes remain understandable for training purposes.

## Additional Standards

- The application must continue to use the existing mock authentication and authorization model unless a feature explicitly requires a production identity provider.
- Database and file storage changes must remain compatible with the current training environment and must not require installation of enterprise services such as LocalDB when unavailable.
- File uploads must validate size, type, storage location, and ownership before execution.
- Feature work must preserve compatibility with the current .NET SDK used by the repository and must be validated in the active development environment before sign-off.
- Any user-facing requirement must be documented clearly enough that it can be tested by a reviewer without additional interpretation.

## Development Workflow

1. Start by understanding the requirement and relevant project context before making changes.
2. Prefer the least disruptive implementation path that satisfies the requirement and matches the project architecture.
3. Verify the relevant behavior with a build or runtime check before declaring the work complete.
4. Keep changes focused and reviewable; avoid unrelated refactors in the same feature branch.
5. Update documentation or project notes when a requirement changes or a new operating constraint is introduced.

## Governance

This constitution governs the development and maintenance of ContosoDashboard. It supersedes informal conventions when they conflict. All feature work, code review, and technical decisions must align with these principles and standards.

Amendments require:
- a clear rationale and scope statement,
- a version bump using semantic versioning,
- documentation of the change in this constitution,
- validation that the amended rules remain consistent with the project’s training goals.

Compliance review expectations:
- Changes must be traceable to the project requirement or a documented issue.
- Security, authorization, and offline constraints are non-negotiable.
- Versioned changes and review notes must be retained in the project’s governance artifacts.

**Version**: 1.0.0 | **Ratified**: 2026-09-15 | **Last Amended**: 2026-09-15
