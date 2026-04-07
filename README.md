# Veltro — Team Feature Assignments

> **Project:** Veltro (Vehicle Parts Selling & Inventory Management System)
> **Repos:** `Veltro_Backend` | `Veltro_Frontend`
> **Base branch:** Always create your feature branch from `develop`

---

## How to get started

```bash
git clone https://github.com/Rajak13/Veltro_Backend.git
cd Veltro_Backend
git checkout develop
git pull origin develop
git checkout -b feature/your-branch-name
```

Once you're done with a feature, push it and open a **Pull Request → develop**.
**Do not merge yourself.** Abdul (leader) will review and merge.

---

## 👑 Abdul Razzaq Ansari — Leader

| # | Feature | Branch |
|---|---------|--------|
| 1 | Admin login, dashboard & role-based access | `feature/admin-auth` |
| 2 | Staff registration & management | `feature/staff-management` |
| 15 | Low stock alerts & overdue credit email reminders | `feature/alerts-and-notifications` |

---

## Rijan Karki

| # | Feature | Branch |
|---|---------|--------|
| 3 | Parts management — add, edit, delete vehicle parts | `feature/parts-management` |
| 5 | Vendor management — CRUD operations | `feature/vendor-management` |
| 4 | Purchase invoices for stock updates | `feature/purchase-invoices` |

---

## Krish Adhikari

| # | Feature | Branch |
|---|---------|--------|
| 7 | Sell vehicle parts & create sales invoices | `feature/sales-invoices` |
| 11 | Send invoices via email to customers | `feature/email-invoices` |
| 16 | Loyalty program — 10% discount on purchases above 5000 | `feature/loyalty-program` |

---

## Punya Kumari Tamang

| # | Feature | Branch |
|---|---------|--------|
| 6 | Customer registration with vehicle details | `feature/customer-registration` |
| 8 | View customer details, history & vehicle info | `feature/customer-history` |
| 10 | Search customers by name, phone, ID, or vehicle number | `feature/customer-search` |

---

## Siddhartha Raj Thapa

| # | Feature | Branch |
|---|---------|--------|
| 9 | Customer reports — regulars, high spenders, pending credits | `feature/customer-reports` |
| 12 | Customer self-registration & profile management | `feature/customer-self-register` |
| 13 | Book appointments, request unavailable parts & submit reviews | `feature/appointments-and-reviews` |
| 14 | View complete purchase & service history | `feature/purchase-service-history` |

---

## Commit Message Convention

Always write meaningful commit messages — this is worth **5 marks** in the marking scheme.

```
feat: add vendor CRUD endpoints
fix: resolve invoice email not sending
docs: add API endpoint documentation
refactor: extract invoice service into separate class
```

---

## Workflow Summary

```
main          ← milestone releases only (Abdul merges here)
  └── develop ← all features merge here via Pull Request
        ├── feature/admin-auth           (Abdul)
        ├── feature/staff-management     (Abdul)
        ├── feature/alerts-and-notifications (Abdul)
        ├── feature/parts-management     (Rijan)
        ├── feature/vendor-management    (Rijan)
        ├── feature/purchase-invoices    (Rijan)
        ├── feature/sales-invoices       (Krish)
        ├── feature/email-invoices       (Krish)
        ├── feature/loyalty-program      (Krish)
        ├── feature/customer-registration  (Punya)
        ├── feature/customer-history       (Punya)
        ├── feature/customer-search        (Punya)
        ├── feature/customer-reports       (Siddhartha)
        ├── feature/customer-self-register (Siddhartha)
        ├── feature/appointments-and-reviews (Siddhartha)
        └── feature/purchase-service-history (Siddhartha)
```

---

*Pre-milestone deadline: **Wednesday, April 9, 2026**. Make sure your first commit is pushed before then.*