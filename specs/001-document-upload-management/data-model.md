# Data Model: Document Upload and Management

## Core Entities

### Document

Represents a stored document and its metadata.

| Field | Type | Notes |
|---|---|---|
| DocumentId | int | Primary key, matches app conventions |
| Title | string | Required |
| Description | string? | Optional |
| Category | string | Text value such as Project Documents or Personal Files |
| FileName | string | Original file name or normalized storage name |
| FilePath | string | Relative or app-managed storage path, not user-supplied |
| FileSize | long | Size in bytes |
| FileType | string | MIME type, up to 255 chars |
| UploadedByUserId | int | Foreign key to User |
| ProjectId | int? | Optional association to a project |
| TaskId | int? | Optional association to a task |
| UploadedAt | DateTime | Time of upload |
| UpdatedAt | DateTime? | Optional revision timestamp |
| IsDeleted | bool | Soft-delete support if needed |

### DocumentShare

Represents user-to-document access granted by a document owner or manager.

| Field | Type | Notes |
|---|---|---|
| DocumentShareId | int | Primary key |
| DocumentId | int | Foreign key to Document |
| UserId | int | Recipient user |
| SharedByUserId | int | Actor who shared the document |
| SharedAt | DateTime | Creation timestamp |
| IsActive | bool | Allows revocation without deletion |

### Notification

Used for in-app alerts when a document is shared or a project receives a new document.

| Field | Type | Notes |
|---|---|---|
| NotificationId | int | Primary key |
| UserId | int | Recipient |
| Message | string | Human-readable detail |
| Type | string | Example: DocumentShared, ProjectDocumentAdded |
| IsRead | bool | Read/unread state |
| CreatedAt | DateTime | Alert timestamp |

## Relationships

- One User can upload many Documents.
- One Project can contain many Documents.
- One Task can optionally contain many Documents.
- One Document can have many DocumentShare records.
- One User can receive many Notifications.

## Implementation Notes

- Category values remain strings instead of enums to remain simple and easy to edit in the training app.
- File path values should be generated using GUID-based names and stored relative to the storage root.
- The storage abstraction should separate the metadata model from the physical file location.
- Document access checks should use membership and role rules rather than trusting the UI alone.
