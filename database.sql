SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS OrderDetail;
DROP TABLE IF EXISTS Cart;
DROP TABLE IF EXISTS `Order`;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS Customer;
DROP TABLE IF EXISTS Admin;
DROP TABLE IF EXISTS Category;

SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE Category (
    CategoryID      INT AUTO_INCREMENT PRIMARY KEY,
    CategoryName    VARCHAR(100) NOT NULL,
    Description     VARCHAR(255),
    Img             VARCHAR(255)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

CREATE TABLE Customer (
    CustomerID      INT AUTO_INCREMENT PRIMARY KEY,
    FullName        VARCHAR(100) NOT NULL,
    Email           VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash    VARCHAR(255) NOT NULL,
    Phone           VARCHAR(20),
    Address         VARCHAR(255),
    RegisterDate    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

CREATE TABLE Product (
    ProductID       INT AUTO_INCREMENT PRIMARY KEY,
    ProductName     VARCHAR(150) NOT NULL,
    CategoryID      INT NOT NULL,
    Brand           VARCHAR(100),
    Price           DECIMAL(10,2) NOT NULL,
    Discount        DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    Description     TEXT,
    Gender          VARCHAR(10),
    Origin          VARCHAR(50),
    MovementType    VARCHAR(50),
    Material        VARCHAR(100),
    Img             VARCHAR(255),
    StockQuantity   INT NOT NULL DEFAULT 0,
    CreatedDate     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_product_category
        FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT chk_product_price CHECK (Price >= 0),
    CONSTRAINT chk_product_discount CHECK (Discount >= 0 AND Discount <= 100),
    CONSTRAINT chk_product_stock CHECK (StockQuantity >= 0)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

CREATE TABLE Admin (
    AdminID         INT AUTO_INCREMENT PRIMARY KEY,
    Username        VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash    VARCHAR(255) NOT NULL,
    Role            VARCHAR(20)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

CREATE TABLE `Order` (
    OrderID         INT AUTO_INCREMENT PRIMARY KEY,
    CustomerID      INT NOT NULL,
    OrderDate       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TotalAmount     DECIMAL(10,2) NOT NULL,
    PaymentMethod   VARCHAR(50),
    ShippingAddress VARCHAR(255),
    Status          VARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT fk_order_customer
        FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT chk_order_total CHECK (TotalAmount >= 0)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

CREATE TABLE Cart (
    CartID          INT AUTO_INCREMENT PRIMARY KEY,
    CustomerID      INT NOT NULL,
    ProductID       INT NOT NULL,
    Quantity        INT NOT NULL,
    AddedDate       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_cart_customer
        FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_cart_product
        FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT uq_cart UNIQUE (CustomerID, ProductID),
    CONSTRAINT chk_cart_quantity CHECK (Quantity > 0)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

CREATE TABLE OrderDetail (
    OrderDetailID   INT AUTO_INCREMENT PRIMARY KEY,
    OrderID         INT NOT NULL,
    ProductID       INT NOT NULL,
    Quantity        INT NOT NULL,
    Price           DECIMAL(10,2) NOT NULL,
    CONSTRAINT fk_orderdetail_order
        FOREIGN KEY (OrderID) REFERENCES `Order`(OrderID)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_orderdetail_product
        FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT uq_orderdetail UNIQUE (OrderID, ProductID),
    CONSTRAINT chk_orderdetail_quantity CHECK (Quantity > 0),
    CONSTRAINT chk_orderdetail_price CHECK (Price >= 0)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

-- Seed data -------------------------------------------------------------

INSERT INTO Category (CategoryName, Description, Img) VALUES
('Chronograph', 'Precision chronograph timepieces', '/img/category-chronograph.jpg'),
('Dive', 'Professional dive watches', '/img/category-dive.jpg'),
('Dress', 'Elegant dress watches', '/img/category-dress.jpg'),
('Smart', 'Connected smart watches', '/img/category-smart.jpg'),
('Skeleton', 'Open-worked mechanical watches', '/img/category-skeleton.jpg');

INSERT INTO Customer (FullName, Email, PasswordHash, Phone, Address) VALUES
('Minh Tran', 'minh@example.com', '$2a$10$ExampleHash1234567890abcd', '0901234567', '123 Nguyen Trai, District 1, HCMC'),
('Lan Nguyen', 'lan@example.com', '$2a$10$ExampleHash0987654321wxyz', '0907654321', '45 Ly Thuong Kiet, Hanoi');

INSERT INTO Product (ProductName, CategoryID, Brand, Price, Discount, Description, Gender, Origin, MovementType, Material, Img, StockQuantity) VALUES
('Aviator Chronograph Pro', 1, 'Zenith', 1899.00, 10.00, 'Stainless steel case, 42mm, automatic chronograph calibre.', 'Men', 'Switzerland', 'Automatic', 'Stainless Steel', '/img/product-aviator-chronograph.jpg', 12),
('DeepBlue Mariner 300', 2, 'Omega', 2499.00, 5.00, 'Helium escape valve, ceramic bezel, 300m water resistance.', 'Men', 'Switzerland', 'Automatic', 'Steel / Ceramic', '/img/product-deepblue-mariner.jpg', 8),
('Midnight Gala 36', 3, 'Cartier', 3299.00, 15.00, 'Diamond-set bezel, mother-of-pearl dial, alligator strap.', 'Women', 'France', 'Automatic', 'Steel / Diamonds', '/img/product-midnight-gala.jpg', 5),
('Pulse Smartwatch S9', 4, 'Garmin', 499.00, 0.00, 'AMOLED display, ECG app, dual-frequency GPS, 10-day battery.', 'Unisex', 'USA', 'Quartz', 'Aluminium', '/img/product-pulse-smartwatch.jpg', 25),
('Aurora Skeleton Tourbillon', 5, 'Hublot', 5899.00, 12.00, 'Hand-wound tourbillon with sapphire bridges.', 'Men', 'Switzerland', 'Manual', 'Titanium / Sapphire', '/img/product-aurora-skeleton.jpg', 3);

INSERT INTO Admin (Username, PasswordHash, Role) VALUES
('storeowner', '$2a$10$ExampleHashAdmin1234567890', 'Owner');

INSERT INTO Cart (CustomerID, ProductID, Quantity) VALUES
(1, 1, 1),
(1, 4, 2),
(2, 3, 1);

INSERT INTO `Order` (CustomerID, TotalAmount, PaymentMethod, ShippingAddress, Status) VALUES
(1, 2897.00, 'Credit Card', '123 Nguyen Trai, District 1, HCMC', 'Processing'),
(2, 2804.15, 'PayPal', '45 Ly Thuong Kiet, Hanoi', 'Pending');

INSERT INTO OrderDetail (OrderID, ProductID, Quantity, Price) VALUES
(1, 1, 1, 1899.00),
(1, 4, 2, 499.00),
(2, 3, 1, 2804.15);