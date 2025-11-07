-- --------------------------------------------------
-- 1. Category Table (Danh mục đồng hồ)
-- --------------------------------------------------
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Img NVARCHAR(255)
);

-- --------------------------------------------------
-- 2. Customer Table (Khách Hàng)
-- --------------------------------------------------
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL, -- Email is unique
    PasswordHash NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20),
    Address NVARCHAR(255),
    RegisterDate DATETIME DEFAULT GETDATE()
);

-- --------------------------------------------------
-- 3. Product Table (TT đồng hồ)
-- Depends on: Category
-- --------------------------------------------------
CREATE TABLE Product (
    ProductID INT PRIMARY KEY,
    ProductName NVARCHAR(150) NOT NULL,
    CategoryID INT NOT NULL,
    Brand NVARCHAR(100),
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
    Discount FLOAT DEFAULT 0.00 CHECK (Discount >= 0 AND Discount <= 1), 
    Description NVARCHAR(MAX),
    Gender NVARCHAR(10),
    Origin NVARCHAR(50),
    MovementType NVARCHAR(50),
    Material NVARCHAR(100),
    Img NVARCHAR(255),
    StockQuantity INT DEFAULT 0 CHECK (StockQuantity >= 0),
    CreatedDate DATETIME DEFAULT GETDATE(),
    -- Foreign Key
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

-- --------------------------------------------------
-- 4. Admin Table (Quản trị viên)
-- Independent table for internal users
-- --------------------------------------------------
CREATE TABLE Admin (
    AdminID INT PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20)
);

-- --------------------------------------------------
-- 5. Order Table (Đơn hàng)
-- Depends on: Customer
-- --------------------------------------------------
CREATE TABLE [Order] ( -- Square brackets used because 'Order' is a reserved SQL keyword
    OrderID INT PRIMARY KEY,
    CustomerID INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(10, 2) NOT NULL CHECK (TotalAmount >= 0),
    PaymentMethod NVARCHAR(50),
    ShippingAddress NVARCHAR(255),
    Status NVARCHAR(50) DEFAULT N'Pending',
    -- Foreign Key
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- --------------------------------------------------
-- 6. Cart Table (Giỏ hàng)
-- Depends on: Customer, Product
-- --------------------------------------------------
CREATE TABLE Cart (
    CartID INT PRIMARY KEY,
    CustomerID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    AddedDate DATETIME DEFAULT GETDATE(),
    -- Foreign Keys
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID),
    -- Ensures a customer only has one entry per product in their cart
    UNIQUE (CustomerID, ProductID) 
);

-- --------------------------------------------------
-- 7. OrderDetail Table (Chi tiết đơn hàng)
-- Depends on: Order, Product
-- --------------------------------------------------
CREATE TABLE OrderDetail (
    OrderDetailID INT PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0), -- Price recorded at time of order
    -- Foreign Keys
    FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID),
    -- Ensures a product is only listed once per order
    UNIQUE (OrderID, ProductID) 
);