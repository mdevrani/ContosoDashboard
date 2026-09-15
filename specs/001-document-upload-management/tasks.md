# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-upload-management/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, quickstart.md

**Organization**: Tasks are grouped by user story to enable independent implementation and testing.

## Phase 1: Setup and Shared Infrastructure

**Purpose**: Add the document feature foundation without altering the existing app structure.

- [ ] T001 Review current application models, DbContext, and service patterns to place document code consistently
- [ ] T002 [P] Add document-related models and data context changes for Document and DocumentShare in `ContosoDashboard/Models/` and `ContosoDashboard/Data/ApplicationDbContext.cs`
- [ ] T003 [P] Create storage abstraction and local implementation in `ContosoDashboard/Services/IFileStorageService.cs` and `ContosoDashboard/Services/LocalFileStorageService.cs`
- [ ] T004 [P] Create base document service skeleton with validation rules in `ContosoDashboard/Services/IDocumentService.cs` and `ContosoDashboard/Services/DocumentService.cs`
- [ ] T005 Configure a local upload folder under `AppData/uploads` and ensure the application creates it on first use

**Checkpoint**: Storage abstraction and document baseline are ready for feature development.

---

## Phase 2: User Story 1 - Upload and secure a work document (Priority: P1) 🎯 MVP

**Goal**: Allow authenticated employees to upload valid files with metadata and protect them from unauthorized access.

**Independent Test**: A user can upload a valid PDF or Office document and see it appear in their own document list.

### Tests for User Story 1

- [ ] T006 [P] [US1] Add a service test for valid document upload success and path generation in `ContosoDashboard.Tests/` or equivalent project test location
- [ ] T007 [P] [US1] Add a validation test for unsupported file types and file size limits

### Implementation for User Story 1

- [ ] T008 [US1] Implement `Document` entity with required fields and relationships in `ContosoDashboard/Models/Document.cs`  
- [ ] T009 [US1] Implement `DocumentShare` entity in `ContosoDashboard/Models/DocumentShare.cs`
- [ ] T010 [US1] Add upload workflow to `DocumentService` including validation, uniq path generation, file save, database persistence, and notification hooks
- [ ] T011 [US1] Add authorization checks to document upload, listing, and retrieval actions based on the current user and project membership
- [ ] T012 [US1] Add a Blazor document upload page or modal to collect metadata and submit files in `ContosoDashboard/Pages/`
- [ ] T013 [US1] Add document list view for My Documents with sorting and filtering in `ContosoDashboard/Pages/Documents.razor`
- [ ] T014 [US1] Update navigation and page routing so users can access document management from the app shell

**Checkpoint**: User Story 1 is fully operational and independently testable.

---

## Phase 3: User Story 2 - Browse, filter, and find documents by project or category (Priority: P2)

**Goal**: Give users a simple view for finding documents by project, category, and search term.

**Independent Test**: A user can filter and search their document list without seeing unauthorized records.

### Tests for User Story 2

- [ ] T015 [P] [US2] Add a search and filter service test covering title, project, and category lookups
- [ ] T016 [P] [US2] Add a permission test proving unauthorized documents are excluded from results

### Implementation for User Story 2

- [ ] T017 [US2] Add database query support for filtering by category, project, date, and file size in `DocumentService`
- [ ] T018 [US2] Implement document search over title, description, tags, uploader, and project names
- [ ] T019 [US2] Add project documents view that shows all allowed project files in `ContosoDashboard/Pages/ProjectDetails.razor`
- [ ] T020 [US2] Add dashboard summary/document counts and recent documents widget in `ContosoDashboard/Pages/Index.razor`
- [ ] T021 [US2] Polish user experience for empty states, sort controls, and result messaging

**Checkpoint**: Users can browse and find documents reliably within authorized scopes.

---

## Phase 4: User Story 3 - Share documents and manage access (Priority: P2)

**Goal**: Allow owners and project managers to share documents and notify recipients.

**Independent Test**: A document owner shares a document and the recipient sees it in the shared list and receives an in-app notification.

### Tests for User Story 3

- [ ] T022 [P] [US3] Add a document sharing permission test ensuring only allowed users can share
- [ ] T023 [P] [US3] Add a notification test confirming recipients receive a share alert

### Implementation for User Story 3

- [ ] T024 [US3] Add share API/service methods for owner and project manager scenarios
- [ ] T025 [US3] Add recipient-specific list view for shared documents in `ContosoDashboard/Pages/`
- [ ] T026 [US3] Integrate notification creation for share events and new project uploads
- [ ] T027 [US3] Add delete and replace-file flows with confirmation, persistence cleanup, and authorization checks

**Checkpoint**: Document access management is in place and auditable.

---

## Phase 5: User Story 4 - View document activity in project and dashboard context (Priority: P3)

**Goal**: Surface document activity where employees already work.

**Independent Test**: A user can view relevant documents from a task, project, or dashboard page.

### Tests for User Story 4

- [ ] T028 [P] [US4] Add an integration test confirming project document lists render only allowed records
- [ ] T029 [P] [US4] Add a dashboard widget test verifying the recent documents list shows the latest five items

### Implementation for User Story 4

- [ ] T030 [US4] Attach document references to tasks and project pages in `ContosoDashboard/Pages/Tasks.razor` and `ContosoDashboard/Pages/ProjectDetails.razor`
- [ ] T031 [US4] Add recent documents summary cards and widget logic to the dashboard home page
- [ ] T032 [US4] Evaluate preview support for PDFs and images and add browser-friendly preview behavior when available

**Checkpoint**: Users can discover relevant documents in the places they already work.

---

## Phase 6: Polish and Cross-Cutting Concerns

**Purpose**: Validate security, quality, and consistency of the whole feature.

- [ ] T033 [P] Review and harden authorization paths for document download, preview, delete, and replace-file actions
- [ ] T034 [P] Add or update audit logging for uploads, downloads, deletions, and share actions
- [ ] T035 [P] Validate upload limits, file type handling, and storage cleanup for all edge cases
- [ ] T036 [P] Review the feature against the requirements in `spec.md` and ensure all user stories are covered by the implementation
- [ ] T037 [P] Run relevant automated checks and manual validation based on `quickstart.md`

---

## Dependencies & Execution Order

- Phase 1 must complete before any user story implementation begins.
- User stories can proceed in priority order once the foundation is ready.
- The P1 story must be fully functional before moving to P2/P3 implementations.
- The final polish phase runs only after all feature stories are complete.
