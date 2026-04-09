# Veltro API — Vehicle Parts Selling & Inventory Management System

A production-ready ASP.NET Core Web API for managing vehicle parts inventory, sales, vendors, customers, and service appointments. Built as a university group project.

---

## User Roles

| Role | Capabilities |
|------|-------------|
| **Admin** | Manage staff, vendors, parts, purchase invoices, financial reports, notifications |
| **Staff** | Register customers & vehicles, create sales invoices, search customers, view reports |
| **Customer** | Self-register, manage profile & vehicles, book appointments, request parts, submit reviews, view own history |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (running locally or remote)
- An SMTP email account (Gmail recommended with App Password)

---

## Setup

**1. Clone and checkout develop branch**
```bash
git clone <repo-url>
cd Veltro_Backend
git checkout develop
```

**2. Configure appsettings.json**

Open `Veltro/Veltro/appsettings.json` and fill in:
- `ConnectionStrings.DefaultConnection` — your PostgreSQL connection string
- `EmailSettings.SenderEmail` / `SenderPassword` — your SMTP credentials

**3. Run migrations**
```bash
cd Veltro/Veltro
dotnet ef database update
```

**4. Run the API**
```bash
dotnet run
```

**5. Open Swagger UI**

Navigate to `https://localhost:{port}/swagger` — all endpoints are documented and testable.

Default Admin credentials (seeded automatically):
- Email: `admin@veltro.com`
- Password: `Admin@1234`

---

## API Endpoints

### Feature 1 — Financial Reports (Admin)
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/reports/financial?period=daily\|monthly\|yearly` | Sales, purchases, net profit, top parts |

### Feature 2 — Staff Management (Admin)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/admin/staff` | Register new staff |
| GET | `/api/admin/staff` | List all staff |
| PUT | `/api/admin/staff/{id}` | Update staff |
| DELETE | `/api/admin/staff/{id}` | Deactivate staff |

### Feature 3 — Parts Management (Admin)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/parts` | Add new part |
| GET | `/api/parts` | List all parts with stock levels |
| PUT | `/api/parts/{id}` | Edit part |
| DELETE | `/api/parts/{id}` | Delete part |

### Feature 4 — Purchase Invoices (Admin)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/invoices/purchase` | Create purchase invoice, updates stock |
| GET | `/api/invoices/purchase` | List all purchase invoices |
| GET | `/api/invoices/purchase/{id}` | Get single purchase invoice |

### Feature 5 — Vendor Management (Admin)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/vendors` | Create vendor |
| GET | `/api/vendors` | List vendors |
| PUT | `/api/vendors/{id}` | Update vendor |
| DELETE | `/api/vendors/{id}` | Deactivate vendor |

### Feature 6 — Customer Registration with Vehicle (Staff)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/staff/customers` | Register customer + optional vehicle |

### Feature 7 — Sales Invoices (Staff)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/invoices/sales` | Create sales invoice (applies loyalty discount, reduces stock) |
| GET | `/api/invoices/sales` | List all sales invoices |
| GET | `/api/invoices/sales/{id}` | Get single sales invoice |

### Feature 8 — Customer Details & History (Staff)
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/staff/customers/{id}` | Customer profile + vehicles |
| GET | `/api/staff/customers/{id}/history` | Purchase + appointment history |

### Feature 9 — Customer Reports (Staff)
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/reports/customers/top-spenders` | Top spending customers |
| GET | `/api/reports/customers/regulars` | Regular customers (3+ purchases) |
| GET | `/api/reports/customers/overdue-credits` | Customers with overdue balances |

### Feature 10 — Customer Search (Staff)
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/staff/customers/search?q={query}&type=name\|phone\|id\|vehicle` | Search customers |

### Feature 11 — Email Invoice (Staff)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/invoices/sales/{id}/send-email` | Email invoice to customer via MailKit |

### Feature 12 — Customer Self-Service
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/auth/register` | Customer self-registration |
| GET | `/api/customers/profile` | View own profile |
| PUT | `/api/customers/profile` | Update own profile |
| POST | `/api/customers/vehicles` | Add vehicle |
| PUT | `/api/customers/vehicles/{id}` | Update vehicle |

### Feature 13 — Appointments, Part Requests & Reviews (Customer)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/appointments` | Book appointment |
| GET | `/api/appointments` | View own appointments |
| PUT | `/api/appointments/{id}` | Cancel appointment |
| POST | `/api/part-requests` | Request unavailable part |
| POST | `/api/reviews` | Submit review with rating |

### Feature 14 — Purchase & Service History (Customer)
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/customers/history/purchases` | Own purchase history |
| GET | `/api/customers/history/appointments` | Own appointment history |

### Feature 15 — Automated Notifications
- Low stock: triggered in `PartService` / `InvoiceService` when stock drops below threshold → creates `Notification` record for Admin
- Overdue credit: `OverdueCreditReminderService` (IHostedService) runs daily at midnight, emails customers with unpaid invoices > 1 month old

### Feature 16 — Loyalty Program
- Handled in `InvoiceService.CreateSalesInvoiceAsync()`
- If subtotal > 5000 → 10% discount applied automatically
- `DiscountApplied` and `TotalAmount` stored on `SalesInvoice`

---

## Library Justifications

| Library | Why |
|---------|-----|
| **Npgsql EF Core** | First-class PostgreSQL support with LINQ-to-SQL and migration tooling |
| **JWT Bearer** | Stateless auth; role claims enable fine-grained access control without session storage |
| **AutoMapper** | Eliminates DTO ↔ Model mapping boilerplate, keeping services focused on business logic |
| **MailKit** | Recommended replacement for deprecated `SmtpClient`; supports TLS and HTML emails |
| **Swashbuckle** | Auto-generates interactive Swagger UI from XML comments for team collaboration |
| **Serilog** | Structured logging with rolling file sinks; machine-readable logs simplify debugging |
| **BCrypt.Net-Next** | Adaptive password hashing with built-in salting; industry standard for credential storage |

---

## ERD

The full Entity Relationship Diagram is included in the project report. Key relationships:

- `User` ←1:1→ `Customer` / `Staff`
- `Customer` ←1:N→ `Vehicle`, `SalesInvoice`, `Appointment`, `PartRequest`, `Review`
- `Vendor` ←1:N→ `Part`, `PurchaseInvoice`
- `SalesInvoice` ←1:N→ `SalesInvoiceItem` ←N:1→ `Part`
- `PurchaseInvoice` ←1:N→ `PurchaseInvoiceItem` ←N:1→ `Part`
