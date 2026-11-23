## HelpDesk System

A role-based IT HelpDesk and Support Ticket Management System built with Blazor, ASP.NET Core Identity, Entity Framework Core, and SQL Server.

The application helps organizations manage IT support requests through a centralized ticketing platform, allowing employees to report issues, technicians to manage support requests, and administrators to oversee the system.

## Overview

HelpDesk System is a three-layer .NET application designed with separation of concerns, role-based access control, and maintainable architecture.

### Key Features

- Ticket Management — Create, view, and manage IT support tickets.
- Automatic Ticket Numbering — Sequential ticket numbers in the format HD-000001.
- Role-Based Access Control — Separate permissions for Employees, Technicians, and Administrators.
- Ticket Assignment — Assign and manage support requests.
- Ticket Status Tracking — Track tickets through Open, In Progress, and Resolved statuses.
- Ticket Conversations — Support communication through ticket comments.
- Ticket History — Maintain a record of ticket activities and status changes.
- Category Management — Administrators can manage ticket categories.
- Account Management — User authentication and identity management.
- Input Validation and Authorization — Protect application operations and data access.

## User Roles

| Role       | Responsibilities                                                                         |
|------------|------------------------------------------------------------------------------------------|
| Employee   | Create support tickets and follow their progress.                                        |
| Technician | Manage assigned support tickets, communicate with employees, and update ticket statuses. |
| Admin      | Manage categories and oversee administrative operations.                                 |

## Technology Stack

| Area           | Technology                  |
|----------------|-----------------------------|
| Framework      | ASP.NET Core / .NET 10      |
| Web UI         | Blazor Web App              |
| Interactivity  | Interactive Server          |
| Backend        | C#                          |
| Authentication | ASP.NET Core Identity       |
| ORM            | Entity Framework Core       | 
| Database       | Microsoft SQL Server        |
| Architecture   | Three-layer architecture    |
| IDE            | Visual Studio               |

## Architecture

The solution is organized into three separate projects:

```text
HelpDeskSystem
│
├── HelpDesk.Web
│   └── Presentation Layer
│       ├── Blazor UI
│       ├── Authentication
│       └── User Interaction
│
├── HelpDesk.BLL
│   └── Business Logic Layer
│       ├── Business Services
│       ├── Interfaces
│       └── DTOs
│
├── HelpDesk.DAL
│   └── Data Access Layer
│       ├── Entity Framework Core
│       ├── Repositories
│       ├── Database Context
│       └── Identity
│
└── HelpDeskSystem.slnx
```

### Layer Responsibilities

- Web: Handles the user interface, authentication flow, and user interaction.
- BLL: Contains business rules, service logic, interfaces, and data transfer objects.
- DAL: Handles database operations, repositories, Entity Framework Core, and identity persistence.

## Security

Security is an important part of the application:

- Role-based authorization.
- User authentication through ASP.NET Core Identity.
- Access validation for ticket operations.
- Authorization checks for ticket comments and history.
- Validation of user permissions before performing restricted operations.

## Database

The application uses Microsoft SQL Server with Entity Framework Core.

The database manages user accounts, roles, tickets, categories, comments, and ticket history.

Ticket numbers follow a sequential format, for example:

```text
HD-000001
HD-000002
HD-000003
```

## Getting Started

### Prerequisites

- Visual Studio with ASP.NET and web development workload.
- .NET 10 SDK.
- Microsoft SQL Server.
- SQL Server Management Studio (SSMS), recommended.

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/ifsan2/HelpDeskSystem.git
   ```

2. Open `HelpDeskSystem.slnx` in Visual Studio.

3. Configure your SQL Server connection string in the appropriate application settings file.

4. Configure the database using the project's Entity Framework Core migration setup.

5. Build and run the HelpDesk.Web project.

> Note: Database configuration and any required setup values should be provided locally. Do not commit passwords, private connection strings, or other secrets.

## Screenshots

Application screenshots will be added here to demonstrate the user interface and main workflows.

## Project Purpose

This project was developed to demonstrate practical skills in:

- ASP.NET Core and Blazor development.
- C# and object-oriented programming.
- Layered application architecture.
- Entity Framework Core and SQL Server.
- Authentication and role-based authorization.
- Repository and service patterns.
- Building maintainable web applications.


Built with C#, Blazor, ASP.NET Core, Entity Framework Core, and SQL Server.