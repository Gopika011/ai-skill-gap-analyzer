# AI Skill Gap Analyzer

An application designed to analyze project skill requirements against employee profiles to identify skill gaps and provide AI-generated training recommendations using Google Gemini.

---

## Project Structure

```text
d:\DOTNET\
├── api\               # Backend (.NET 10.0 Web API & SQLite)
│   ├── Project.Api\   # Web API core logic and controllers
│   └── Project.Tests\ # Unit tests (Authentication, Employees, Projects)
└── ui\                # Frontend (Angular 21.0)
```

---

## Prerequisites

Make sure you have the following installed on your machine:
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18 or higher recommended)

---

## Setup & Running Instructions

### 1. Run the Backend (API)

Open a terminal and navigate to the backend API directory:
```bash
cd api/Project.Api
```

Start the .NET Web API development server:
```bash
dotnet run
```
The API server runs at `http://localhost:5213`.

### 2. Run the Frontend (UI)

Open a new terminal and navigate to the frontend directory:
```bash
cd ui
```

Install dependencies (first-time setup only):
```bash
npm install
```

Start the Angular development server:
```bash
npm start
```
Open your browser and navigate to `http://localhost:4200` to access the application.

---

## Running Unit Tests

To run the backend test suite:
```bash
cd api/Project.Tests
dotnet test
```
