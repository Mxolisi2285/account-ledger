# Account Ledger Management System

**Author:** Mxolisi Masina

A full-stack web application for managing people, their accounts, and transaction records. Built with Angular 17+ on the frontend and ASP.NET Core 8 on the backend, backed by a PostgreSQL database using stored procedures via Dapper.

---

## Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Backend Setup](#backend-setup)
  - [Frontend Setup](#frontend-setup)
- [Authentication](#authentication)
- [API Endpoints](#api-endpoints)
- [Database](#database)
- [Known Constraints](#known-constraints)

---

## Overview

The Account Ledger Management System provides a centralized platform to:

- Register and manage person records
- Create and manage accounts linked to people
- Record and view credit/debit transactions against accounts
- Search and paginate through person records
- Enforce business rules (e.g. no deletion of persons with open accounts, no posting to closed accounts)

---

## Tech Stack

### Frontend
| Technology | Purpose |
|---|---|
| Angular 17+ (Standalone) | SPA framework |
| Tailwind CSS | Utility-first styling |
| Reactive Forms | Form handling and validation |
| Angular Router | Lazy-loaded feature routing |
| RxJS | Async data streams |
| Angular Signals | Toast notification state |

### Backend
| Technology | Purpose |
|---|---|
| ASP.NET Core 8 | REST API framework |
| Dapper | Lightweight ORM |
| PostgreSQL | Relational database |
| FluentValidation | Request DTO validation |
| Npgsql | PostgreSQL .NET driver |
| Swagger / OpenAPI | API documentation |

---

## Features

### People Management
- View paginated list of registered people
- Search by Surname, ID Number, or Account Number
- Create new person records
- View and edit person details
- Delete persons (blocked if open accounts exist)

### Account Management
- View all accounts linked to a person
- Create new accounts inline from the person details page
- Open and close accounts (balance must be zero to close)
- View account balance and status

### Transaction Management
- View full transaction history per account
- Add credit (deposit) and debit (withdrawal) transactions
- Edit existing transactions
- Balance automatically recalculated after each transaction

### Security
- HTTP Basic Authentication
- Credentials stored in `sessionStorage`
- Auth guard protects all feature routes
- Auth interceptor attaches credentials to every API request
- Automatic redirect to login on 401

### UX
- Toast notifications for all success and error events (auto-dismiss after 4 seconds)
- Inline forms for adding accounts and transactions (no page navigation required)
- Loading spinners on all async operations
- Responsive layout with Tailwind CSS

---

## Project Structure

```
AccountLedger/
├── frontend-accountledger/          # Angular SPA
│   └── src/app/
│       ├── core/                    # Guards, interceptors, auth & toast services
│       ├── shared/                  # Models, reusable components (toast, pagination)
│       ├── features/                # Lazy-loaded feature modules
│       │   ├── auth/                # Login
│       │   ├── persons/             # Persons list + details
│       │   ├── accounts/            # Account details + transactions
│       │   └── transactions/        # Transaction details
│       └── layout/                  # Header and footer shell components
│
└── backend-accountledger/           # ASP.NET Core REST API
    ├── Controllers/                 # PersonsController, AccountsController, TransactionsController
    ├── Middleware/                  # BasicAuthHandler, GlobalExceptionMiddleware
    ├── Application/
    │   ├── DTOs/                    # Request/response data transfer objects
    │   ├── Interfaces/              # Repository and service contracts
    │   ├── Services/                # Business logic layer
    │   └── Validators/              # FluentValidation rules
    └── Infrastructure/
        └── Data/Repositories/       # Dapper + PostgreSQL stored procedure calls
```

---

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) v18+
- [Angular CLI](https://angular.io/cli) v17+
- [.NET SDK](https://dotnet.microsoft.com/) 8.0+
- [PostgreSQL](https://www.postgresql.org/) 14+

---

### Backend Setup

1. Clone the repository and navigate to the backend:
```bash
cd backend-accountledger
```

2. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=account_ledger;Username=your_user;Password=your_password"
  }
}
```

3. Ensure all PostgreSQL stored procedures are created in your database. Key functions include:
   - `fn_get_persons_paged` — paginated + searchable person list
   - `fn_get_persons_total` — total count for pagination
   - `fn_create_person`, `fn_update_person`, `fn_delete_person`
   - `fn_get_accounts_by_person`, `fn_create_account`
   - `fn_toggle_account_status` — open/close with balance validation
   - `fn_create_transaction`, `fn_get_transactions_by_account`

4. Run the backend:
```bash
dotnet run
```

The API will start on `http://localhost:5191`. Swagger UI is available at `http://localhost:5191/swagger`.

---

### Frontend Setup

1. Navigate to the frontend:
```bash
cd frontend-accountledger
```

2. Install dependencies:
```bash
npm install
```

3. Verify the API base URL in `src/app/core/services/api.service.ts`:
```typescript
export const API_BASE_URL = 'http://localhost:5191/api';
```

4. Start the dev server:
```bash
ng serve
```

The app will be available at `http://localhost:4200`.

> **Important:** Always run Angular on port 4200. If the port is in use, free it first:
> ```bash
> lsof -ti:4200 | xargs kill -9
> ng serve
> ```
> Running on a different port will cause CORS errors since the backend only allows `http://localhost:4200`.

---

## Authentication

The system uses **HTTP Basic Authentication**. Credentials are Base64-encoded and sent with every API request via the `Authorization` header.

- Credentials are stored in `sessionStorage` under the key `auth_credentials`
- On logout or 401 response, credentials are cleared and the user is redirected to `/login`
- The login flow validates credentials by making a test request to the persons endpoint

---

## API Endpoints

### Persons
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/persons` | Paginated list with optional search |
| GET | `/api/persons/:id` | Get person by ID |
| POST | `/api/persons` | Create new person |
| PUT | `/api/persons/:id` | Update person |
| DELETE | `/api/persons/:id` | Delete person |
| GET | `/api/persons/check-id/:idNumber` | Check if ID number exists |

### Accounts
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/accounts/person/:personId` | Get accounts by person |
| GET | `/api/accounts/:id` | Get account by ID |
| POST | `/api/accounts` | Create new account |
| PUT | `/api/accounts/:id` | Update account |
| POST | `/api/accounts/:id/toggle-status` | Open or close account |
| DELETE | `/api/accounts/:id` | Delete account |

### Transactions
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/transactions/account/:accountId` | Get transactions by account |
| GET | `/api/transactions/:id` | Get transaction by ID |
| POST | `/api/transactions` | Create transaction |
| PUT | `/api/transactions/:id` | Update transaction |
| DELETE | `/api/transactions/:id` | Delete transaction |

---

## Database

All database operations are performed through **PostgreSQL stored procedures** called via Dapper. No raw SQL is written in the application layer — all queries go through named functions.

### Business Rules enforced at DB level
- A person cannot be deleted if they have open accounts
- An account cannot be closed if the outstanding balance is not zero
- Transactions cannot be posted to a closed account
- Transaction dates cannot be in the future
- Account numbers must be unique

---

## Known Constraints

- **Page size** is capped at 10 by the backend (`Math.Clamp(pageSize, 1, 10)`)
- **Search types** must match exactly: `Surname`, `IdNumber`, or `AccountNumber`
- **Basic Auth** is not suitable for production — consider upgrading to JWT for production deployments
- CORS is configured for `http://localhost:4200` only — update `Program.cs` for production domains

---

## License

This project was developed by **Mxolisi Masina** as a full-stack portfolio project demonstrating Angular, ASP.NET Core, PostgreSQL, and clean architecture principles.
