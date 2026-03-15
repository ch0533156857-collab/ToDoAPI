# ✅ Todo API

A RESTful API for task management, built with C# / .NET and containerized with Docker.

## 🛠️ Tech Stack

- **Backend:** C#, .NET, Entity Framework Core
- **Database:** SQL Server (via EF Core DbContext)
- **DevOps:** Docker, .dockerignore

## 🚀 Run with Docker

```bash
docker build -t todo-api .
docker run -p 8080:80 todo-api
```

## 🏃 Run Locally

```bash
dotnet restore
dotnet run
```

## 📁 Project Structure

```
ToDoAPI/
├── Program.cs          # Entry point & API configuration
├── TaskItem.cs         # Task entity model
├── ToDoDbContext.cs    # EF Core database context
├── Dockerfile          # Docker containerization
└── appsettings.json    # App configuration
```

## 📋 API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/tasks` | Get all tasks |
| GET | `/tasks/{id}` | Get task by ID |
| POST | `/tasks` | Create new task |
| PUT | `/tasks/{id}` | Update task |
| DELETE | `/tasks/{id}` | Delete task |
