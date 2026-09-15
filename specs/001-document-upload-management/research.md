# Research: Document Upload and Management

## Design Decisions

### 1. Local filesystem storage with abstraction

The project requirement explicitly states that the training implementation must work offline without cloud services. The solution is to store uploaded files under a local folder such as `AppData/uploads` and expose the storage behavior through an interface named `IFileStorageService`.

Rationale:
- Keeps the feature self-contained and suitable for offline training.
- Enables future migration to Azure Blob Storage with no UI or business logic rewrite.
- Aligns with the existing architecture principles described in the project README.

### 2. Pre-generate unique paths before DB insertion

The file path must be generated before the database record is saved. This avoids duplicate-key issues when multiple files share the same name or when the save step fails mid-way.

Rationale:
- Prevents orphaned metadata rows.
- Prevents path collisions and helps avoid path traversal issues.
- Matches the recommended design in the stakeholder requirements.

### 3. Metadata-first upload workflow

The upload sequence should be:
1. Validate file type and size.
2. Authorize the user and project association.
3. Generate a GUID-based file name.
4. Save the file to the local storage directory.
5. Save the metadata to the database.
6. Trigger project or user notifications.

Rationale:
- Keeps the app resilient when storage writes fail.
- Makes it easier to enforce record consistency.
- Aligns with the business requirements for security and audit logging.

### 4. Access control enforcement in services

All document actions must be validated in the service layer rather than relying only on UI conditions.

Rationale:
- Protects against IDOR-style vulnerabilities.
- Matches the project’s existing service-layer security model.
- Keeps authorization consistent across pages and future endpoints.

### 5. Document metadata and search strategy

The application stores document metadata in the database and searches over title, description, tag fields, uploader names, and project names.

Rationale:
- Keeps search fast enough for the expected training workload.
- Avoids unnecessary complexity from a separate search engine.
- Supports a simple and maintainable implementation inside the current app.

## Risks and Mitigations

### Risk: Local storage path exposure
Mitigation: store files outside `wwwroot`, use GUID-based file names, and only serve through authorized endpoints or app-managed actions.

### Risk: Unsupported or malicious file uploads
Mitigation: validate extension, MIME type, and size before storage and reject invalid files early.

### Risk: Duplicate or inconsistent records
Mitigation: use a unique generated path and transaction-safe service logic around file save and DB save.

### Risk: Over-permission access
Mitigation: enforce authorization at service level and require project membership checks before upload or download.

## Open Questions

- Whether the feature should include direct file preview for PDFs and images only, or all recognized browser-safe files.
- Whether document tags should be stored as a comma-separated text field or a normalized tag table for future scale.
- Whether task-level document attachments require a separate join entity or can reuse the document metadata with a task association.

These are acceptable for the training scope and can be resolved during implementation with the simplest viable design.
