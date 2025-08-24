# EmployeeregistrationClient

A full-stack web application for managing employee details with Angular frontend and ASP.NET Core Web API backend.

## Features

- Add, edit, delete employees
- Dynamic country/state dropdown based on selection
- Form validation with Angular reactive forms
- Mobile number uniqueness check
- Pagination and filtering
- Material Design UI (Angular Material)

## Tech Stack

- **Frontend:** Angular 16+, Angular Material, Vite
- **Backend:** ASP.NET Core Web API (.NET 6+)
- **Database:** SQL Server

## API Endpoints

- `GET /api/employee` – Get all employees (with pagination)
- `GET /api/employee/{id}` – Get employee by ID
- `POST /api/employee` – Add employee
- `PUT /api/employee/{id}` – Update employee
- `DELETE /api/employee/{id}` – Delete employee
- `GET /api/employee/check-mobile/{mobile}` – Check mobile number uniqueness


## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Karma](https://karma-runner.github.io) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
