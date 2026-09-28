# Supermarket Management System (C# WinForms + SQL Server)

A desktop supermarket management system built with **C# WinForms (.NET 8)** and **SQL Server**.

## Features
- Login screen (default: username `admin`, password `admin123`)
- Products management (with category & supplier lookups, stock levels, reorder level)
- Categories management
- Suppliers management
- Customers management
- Point of Sale (POS) screen — scan/type a product code, build a cart, checkout, auto-updates stock
- Reports — sales report by date range + total revenue, and a low-stock alert list

## Folder structure
```
SupermarketMS/
├── Database/
│   └── SupermarketDB.sql        <- run this in SSMS first
├── Src/
│   ├── SupermarketManagementSystem.csproj
│   ├── Program.cs
│   ├── Helpers/DBHelper.cs      <- connection string lives here
│   ├── Models/Product.cs
│   └── Forms/
│       ├── LoginForm.cs
│       ├── MainForm.cs
│       ├── ProductsForm.cs
│       ├── CategoriesForm.cs
│       ├── SuppliersForm.cs
│       ├── CustomersForm.cs
│       ├── POSForm.cs
│       └── ReportsForm.cs
└── SupermarketManagementSystem.sln
```

## 1) Set up the database (SQL Server Management Studio)
1. Open **SQL Server Management Studio (SSMS)** and connect to your SQL Server instance.
2. Open `Database/SupermarketDB.sql`.
3. Click **Execute** (F5). This creates the `SupermarketDB` database, all tables, and seed/sample data
   (an admin user, sample categories, suppliers, products, customers).

## 2) Run the application
1. Requires **.NET 8 SDK** (or Visual Studio 2022 17.8+ with the ".NET desktop development" workload).
2. Open `SupermarketManagementSystem.sln` in Visual Studio, **or** from a terminal:
   ```
   cd Src
   dotnet restore
   dotnet run
   ```
3. On first run, edit the connection string in `Src/Helpers/DBHelper.cs` if needed:
   ```csharp
   public static string ConnectionString { get; set; } =
       @"Server=.\SQLEXPRESS;Database=SupermarketDB;Trusted_Connection=True;TrustServerCertificate=True;";
   ```
   - Use `Server=.` for the default (non-named) instance.
   - Use `Server=.\SQLEXPRESS` for the free SQL Server Express named instance.
   - If you use SQL Server authentication instead of Windows authentication, replace
     `Trusted_Connection=True` with `User Id=...;Password=...`.
4. Log in with `admin` / `admin123`.

## Notes
- Passwords are stored in plain text in the seed script for simplicity (`PasswordHash` column).
  For real use, hash passwords (e.g. with `BCrypt.Net`) before storing/comparing them.
- The POS form runs the sale insert + stock deduction inside a single SQL transaction, so a failure
  midway rolls everything back and nothing is left half-saved.
- All SQL access uses parameterized queries (`Microsoft.Data.SqlClient`) to avoid SQL injection.
