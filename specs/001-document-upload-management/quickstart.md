# Quickstart: Document Upload and Management

## Local Setup

1. Open the solution in Visual Studio or VS Code.
2. Ensure the .NET 8 SDK is installed.
3. Navigate to the project folder and run:

```powershell
dotnet restore
dotnet run
```

4. Open the app in a browser and log in using one of the mock users in the application.

## Basic Validation

### Upload a document

1. Open the Documents or Project Documents view.
2. Select a valid PDF, Word, Excel, PowerPoint, text, or image file below 25 MB.
3. Enter a title and category.
4. Choose an optional project and tags.
5. Submit the upload.
6. Confirm the document appears in the list with metadata and success feedback.

### Validation failure checks

- Upload a file over 25 MB and confirm the file is rejected with a clear message.
- Upload a file with an unsupported extension and confirm the rejection.
- Attempt to upload to a project without membership and confirm the restriction.

### Search and filter

1. Open My Documents or Project Documents.
2. Search by title or tag.
3. Apply a category or project filter.
4. Confirm only permitted documents appear in the result set.

### Sharing and notification

1. Select a document that you own.
2. Share it with another user.
3. Log in as that user and verify the shared document appears in the shared list.
4. Confirm an in-app notification is generated.

## Success Signals

- Success message appears after upload.
- File is stored outside the web root.
- Database record is created with metadata.
- Search results respect authorization.
- Notifications appear for recipients of shared documents.
