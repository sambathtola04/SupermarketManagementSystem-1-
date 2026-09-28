/* ==========================================================
   Supermarket Management System - Database Script
   Run this in SQL Server Management Studio (SSMS)
   ========================================================== */

IF DB_ID('SupermarketDB') IS NOT NULL
BEGIN
    ALTER DATABASE SupermarketDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SupermarketDB;
END
GO

CREATE DATABASE SupermarketDB;
GO

USE SupermarketDB;
GO

/* ---------------- Users (login) ---------------- */
CREATE TABLE Users (
    UserID       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    FullName     NVARCHAR(100) NOT NULL,
    Role         NVARCHAR(20)  NOT NULL DEFAULT 'Cashier', -- Admin / Manager / Cashier
    IsActive     BIT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------- Categories ---------------- */
CREATE TABLE Categories (
    CategoryID   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description  NVARCHAR(255) NULL
);
GO

/* ---------------- Suppliers ---------------- */
CREATE TABLE Suppliers (
    SupplierID   INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(150) NOT NULL,
    ContactName  NVARCHAR(100) NULL,
    Phone        NVARCHAR(30)  NULL,
    Email        NVARCHAR(100) NULL,
    Address      NVARCHAR(255) NULL
);
GO

/* ---------------- Products ---------------- */
CREATE TABLE Products (
    ProductID    INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode  NVARCHAR(30)  NOT NULL UNIQUE,
    ProductName  NVARCHAR(150) NOT NULL,
    CategoryID   INT NULL FOREIGN KEY REFERENCES Categories(CategoryID),
    SupplierID   INT NULL FOREIGN KEY REFERENCES Suppliers(SupplierID),
    CostPrice    DECIMAL(12,2) NOT NULL DEFAULT 0,
    SellPrice    DECIMAL(12,2) NOT NULL DEFAULT 0,
    QuantityInStock INT       NOT NULL DEFAULT 0,
    ReorderLevel INT           NOT NULL DEFAULT 5,
    Unit         NVARCHAR(20)  NOT NULL DEFAULT 'pcs',
    IsActive     BIT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------- Customers ---------------- */
CREATE TABLE Customers (
    CustomerID   INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(150) NOT NULL,
    Phone        NVARCHAR(30)  NULL,
    Email        NVARCHAR(100) NULL,
    Address      NVARCHAR(255) NULL,
    LoyaltyPoints INT          NOT NULL DEFAULT 0
);
GO

/* ---------------- Sales (POS header) ---------------- */
CREATE TABLE Sales (
    SaleID       INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNo    NVARCHAR(30)  NOT NULL UNIQUE,
    SaleDate     DATETIME      NOT NULL DEFAULT GETDATE(),
    CustomerID   INT NULL FOREIGN KEY REFERENCES Customers(CustomerID),
    UserID       INT NULL FOREIGN KEY REFERENCES Users(UserID),
    SubTotal     DECIMAL(12,2) NOT NULL DEFAULT 0,
    Discount     DECIMAL(12,2) NOT NULL DEFAULT 0,
    Tax          DECIMAL(12,2) NOT NULL DEFAULT 0,
    TotalAmount  DECIMAL(12,2) NOT NULL DEFAULT 0,
    AmountPaid   DECIMAL(12,2) NOT NULL DEFAULT 0,
    ChangeDue    DECIMAL(12,2) NOT NULL DEFAULT 0,
    PaymentMethod NVARCHAR(20) NOT NULL DEFAULT 'Cash'
);
GO

/* ---------------- Sale Details (POS lines) ---------------- */
CREATE TABLE SaleDetails (
    SaleDetailID INT IDENTITY(1,1) PRIMARY KEY,
    SaleID       INT NOT NULL FOREIGN KEY REFERENCES Sales(SaleID) ON DELETE CASCADE,
    ProductID    INT NOT NULL FOREIGN KEY REFERENCES Products(ProductID),
    Quantity     INT NOT NULL,
    UnitPrice    DECIMAL(12,2) NOT NULL,
    LineTotal    AS (Quantity * UnitPrice) PERSISTED
);
GO

/* ---------------- Helpful indexes ---------------- */
CREATE INDEX IX_Products_Category ON Products(CategoryID);
CREATE INDEX IX_Sales_Date ON Sales(SaleDate);
GO

/* ==========================================================
   Seed data
   ========================================================== */

INSERT INTO Users (Username, PasswordHash, FullName, Role)
VALUES ('admin', 'admin123', N'System Administrator', 'Admin');

INSERT INTO Categories (CategoryName, Description) VALUES
(N'Beverages', N'Drinks, juices, water'),
(N'Snacks', N'Chips, cookies, candy'),
(N'Dairy', N'Milk, cheese, yogurt'),
(N'Household', N'Cleaning and home supplies');

INSERT INTO Suppliers (SupplierName, ContactName, Phone, Email) VALUES
(N'ABC Distribution', N'Mr. Sok Dara', '012345678', 'abc@example.com'),
(N'Fresh Foods Co.', N'Ms. Lina', '098765432', 'fresh@example.com');

INSERT INTO Products (ProductCode, ProductName, CategoryID, SupplierID, CostPrice, SellPrice, QuantityInStock, ReorderLevel, Unit) VALUES
('P0001', N'Coca-Cola 330ml', 1, 1, 0.30, 0.60, 200, 20, 'can'),
('P0002', N'Bottled Water 500ml', 1, 1, 0.15, 0.35, 300, 30, 'bottle'),
('P0003', N'Potato Chips 100g', 2, 2, 0.50, 1.00, 150, 15, 'pack'),
('P0004', N'Fresh Milk 1L', 3, 2, 0.80, 1.50, 80, 10, 'carton'),
('P0005', N'Dish Soap 500ml', 4, 1, 1.00, 2.00, 60, 10, 'bottle');

INSERT INTO Customers (CustomerName, Phone) VALUES
(N'Walk-in Customer', NULL),
(N'Sokha Chan', '011222333');
GO

PRINT 'SupermarketDB created and seeded successfully.';
