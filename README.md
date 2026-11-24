# Student Admin API

A learning project for student administration built with ASP.NET Core 8, C#, ADO.NET, and SQL Server. It explores API design, layered architecture, JWT authentication, and authorization policies.

## What is included

- Student listing, creation, deletion, and profile updates.
- User lookup, creation, password updates, and deletion.
- JWT access-token generation and BCrypt password hashing.
- Role-based checks on administrative student operations and an owner-or-admin profile policy.
- Swagger UI in the Development environment.
- A fixed-window rate limiter on authentication routes.

## Project structure

```text
StudentAdminProject/   API controllers, request DTOs, authorization policy, startup
BusinessLogicLayer/   Student/user services, password hashing, token generation
DataAccessLayer/      SQL Server access through ADO.NET
```

## API map

The source currently defines 13 controller actions:

| Method | Route | Purpose |
| --- | --- | --- |
| POST | `/api/Auth/Login` | Sign in |
| POST | `/api/Auth/refresh` | Request new tokens |
| POST | `/api/Auth/Logout` | Logout response |
| GET | `/api/User/{id}` | Read a user |
| PUT | `/api/User/{id}/password` | Update a password |
| POST | `/api/User` | Create a user |
| DELETE | `/api/User/{id}` | Delete a user |
| GET | `/api/Student/{userId}/profile` | Read a student profile |
| PUT | `/api/Student/{userId}/profile` | Update a student profile |
| GET | `/api/Student/all` | List students |
| GET | `/api/Student/{id}` | Read a student |
| DELETE | `/api/Student/{id}` | Delete a student |
| POST | `/api/Student` | Create a student |

## Local setup

Prerequisites: the .NET 8 SDK and a SQL Server instance.

The database schema and seed scripts are not included in this repository. You must supply a compatible database before the database-backed endpoints can work. This is not a clone-and-run release.

Configure `ConnectionStrings:DefaultConnection`, `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, and `Jwt:DurationInMinutes` for your own environment. Use local secret storage or environment variables; do not reuse or publish a signing key.

From the repository root:

```bash
dotnet restore StudentAdminProject/StudentAdminProject.csproj
dotnet run --project StudentAdminProject/StudentAdminProject.csproj --launch-profile https
```

The checked-in HTTPS launch profile uses `https://localhost:7290/swagger` in Development. Trust a local development certificate if your environment requires it.

## Current limitations

This is a local educational project, not a production-ready service.

- User-management actions currently lack authorization protection in the checked-in controller.
- Refresh-token persistence, validation, rotation, and revocation are not implemented; logout does not invalidate issued tokens.
- The checked-in JWT configuration must be replaced with environment-specific configuration before any deployment.
- Database setup scripts and automated tests are not included.

These are next steps, not features already delivered. The setup commands describe the project configuration and have not been runtime-verified as part of this documentation update.
