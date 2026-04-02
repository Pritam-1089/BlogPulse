# BlogPulse - Blogging Platform

A full-stack blogging platform with rich text editing, user profiles, and content management, built with ASP.NET Core and Angular following Clean Architecture.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | ASP.NET Core 9, Entity Framework Core, SQL Server |
| **Frontend** | Angular 20, TypeScript, SCSS |
| **Auth** | JWT Authentication + Role-Based Access |

## Features

- **Rich Text Editor** — Create and edit blog posts with a full-featured WYSIWYG editor
- **Categories & Tags** — Organize posts with categories and searchable tags
- **Comment System** — Threaded comments with moderation tools for authors
- **User Profiles** — Public author profiles with bio, avatar, and post history
- **Follow System** — Follow authors and get personalized content feeds
- **Admin Panel** — Content moderation, user management, and site analytics
- **Search & Filter** — Full-text search across posts with category and tag filters
- **SEO Friendly** — Clean URLs, meta tags, and structured content
- **JWT Authentication** — Secure login with Admin and Author roles

## Architecture

```
BlogPulse/
├── BlogPulse.API/              # Web API layer - Controllers, Middleware
├── BlogPulse.Application/      # Business logic - Services, DTOs, Validators
├── BlogPulse.Core/             # Domain layer - Entities, Interfaces
├── BlogPulse.Infrastructure/   # Data access - EF Core, Repositories
└── BlogPulse.Frontend/         # Angular 20 SPA
```

## Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js 20+
- SQL Server
- Angular CLI

### Run Backend
```bash
cd BlogPulse.API
dotnet restore
dotnet ef database update
dotnet run
```

### Run Frontend
```bash
cd BlogPulse.Frontend
npm install
ng serve
```

API will run on `https://localhost:7003` and frontend on `http://localhost:4200`

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login & get JWT token |
| GET | `/api/posts` | Get all posts (paginated) |
| GET | `/api/posts/{slug}` | Get post by slug |
| POST | `/api/posts` | Create new post |
| PUT | `/api/posts/{id}` | Update post |
| GET | `/api/categories` | Get all categories |
| POST | `/api/posts/{id}/comments` | Add comment |
| GET | `/api/users/{id}/profile` | Get author profile |
| POST | `/api/users/{id}/follow` | Follow an author |
| GET | `/api/admin/dashboard` | Admin analytics |

## Screenshots

> Coming soon

## License

This project is licensed under the MIT License.
