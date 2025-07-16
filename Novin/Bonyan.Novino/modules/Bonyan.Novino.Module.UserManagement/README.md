# Bonyan.Novino.Module.UserManagement

This module provides user management functionality for the Bonyan Novino application.

## Structure

```
Bonyan.Novino.Module.UserManagement/
├── Controllers/
│   └── UserManagementController.cs
├── Models/
│   └── UserManagementViewModels.cs
├── Views/
│   ├── _ViewImports.cshtml
│   ├── _ViewStart.cshtml
│   └── UserManagement/
│       └── Index.cshtml
├── BonyanNovinoUserManagementModule.cs
├── Bonyan.Novino.Module.UserManagement.csproj
└── README.md
```

## Features

- User listing with search and pagination
- User sorting by different fields
- Bulk operations support
- Permission-based access control
- Responsive UI with Bootstrap 5

## Usage

To use this module, add it to your application's module dependencies and ensure the views are properly configured to load from the module.

## Dependencies

- Bonyan.Novino.Domain
- Bonyan.Novino.Infrastructure
- Bonyan.Ui.Novino.Core
- Bonyan.Ui.Novino.Web

## Routes

- `GET /UserManagement/Index` - User list page with filtering and pagination 