# Employee CRUD - Angular + .NET Core

A full-stack employee registration and management system built with **Angular** (frontend) and **ASP.NET Core Web API** (backend), demonstrating simple CRUD operations with form validations and a SQL Server database.

---

## Features

- Employee Registration Form with the following fields:
  - Auto-generated Employee ID (non-editable)
  - Name, Age, Mobile Number (with validations)
  - Date of Birth with date picker and auto-age calculation
  - Address Line 1 and 2 (with validation for special characters)
  - Pincode, State, Country (dynamic dropdowns)

- Validations:
  - Only alphabets for Name
  - 1–3 digit Age
  - 10-digit unique Mobile Number
  - Future DOB not allowed
  - Custom validations for address fields and pincode

- Display:
  - Paginated (5 records per page) employee list
  - Filter by Name or Mobile Number

- CRUD:
  - Add, Edit, Delete employees
  - Confirm before delete
  - Real-time feedback with toasts

- API:
  - Built with .NET Core Web API
  - Endpoints for Create, Read, Update, Delete

---

## Tech Stack

| Frontend  | Backend     | Database     |
|-----------|-------------|--------------|
| Angular   | ASP.NET Core Web API | SQL Server |

---

##  Project Structure

/employee-crud-angular-dotnet/
│
├── /employeeregistration.client/       <- Angular frontend
├── /EmployeeRegistration.Server/       <- .NET backend
├── README.md
├── DB.txt         <- contain Functions and procedure used in DB
└── .gitignore     <- Root-level .gitignore


---

## Setup Instructions

### Prerequisites:
- Node.js and Angular CLI
- .NET 6.0 SDK or later
- SQL Server
- Git

---

### 🔹 1. Clone the Repository

git clone https://github.com/KishoreSolairaj/angular-dotnet-crud.git
cd angular-dotnet-crud

### 🔹 2. Backend Setup (/server)
cd server
dotnet restore
dotnet ef database update  # Run EF migrations or use SQL script manually
dotnet run

### 🔹 2. B3. Frontend Setup (/client)
cd ../client
npm install
ng serve
