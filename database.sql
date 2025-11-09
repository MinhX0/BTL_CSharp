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


INSERT INTO Admin (Username, PasswordHash, Role) VALUES
('storeowner', '$2a$10$ExampleHashAdmin1234567890', 'Owner');

--Category table
INSERT INTO Category (CategoryID, CategoryName, Description, Img)
VALUES
(1, N'Đồng hồ Nam', N'Phong cách mạnh mẽ, lịch lãm, phù hợp doanh nhân và người đi làm văn phòng.', N'dong_ho_nam.jpg'),
(2, N'Đồng hồ Nữ', N'Thiết kế tinh tế, sang trọng, dành cho phái đẹp.', N'dong_ho_nu.jpg'),
(3, N'Đồng hồ Đôi', N'Bộ đôi đồng hồ nam - nữ dành cho các cặp đôi.', N'dong_ho_doi.jpg'),
(4, N'Đồng hồ Cơ', N'Vận hành bằng cơ học, không cần pin, sang trọng và đẳng cấp.', N'dong_ho_co.jpg'),
(5, N'Đồng hồ Quartz', N'Hoạt động bằng pin, chính xác và phổ biến nhất hiện nay.', N'dong_ho_quartz.jpg'),
(6, N'Đồng hồ Thể Thao', N'Thiết kế năng động, bền bỉ, phù hợp cho người yêu vận động.', N'dong_ho_the_thao.jpg'),
(7, N'Đồng hồ Thông Minh', N'Kết nối Bluetooth, theo dõi sức khỏe, thông báo thông minh.', N'dong_ho_thong_minh.jpg'),
(8, N'Đồng hồ Thời Trang', N'Mẫu mã hiện đại, màu sắc đa dạng, hợp xu hướng.', N'dong_ho_thoi_trang.jpg'),
(9, N'Đồng hồ Cao Cấp', N'Thương hiệu sang trọng, chất liệu cao cấp, đẳng cấp doanh nhân.', N'dong_ho_cao_cap.jpg'),
(10, N'Đồng hồ Trẻ Em', N'Thiết kế đáng yêu, màu sắc tươi sáng, phù hợp cho trẻ nhỏ.', N'dong_ho_tre_em.jpg');

--Product table
INSERT INTO Product (ProductID, ProductName, CategoryID, Brand, Price, Discount, Description, Gender, Origin, MovementType, Material, Img, StockQuantity, CreatedDate)
VALUES
(1, N'Seiko Presage Cocktail Time', 9, N'Seiko', 12000000.00, 0.00, N'Đồng hồ Cơ cao cấp', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ', N'dong_ho_cao_cap_1.jpg', 20, '2023-01-01'),
(2, N'Tissot Le Locle Automatic', 9, N'Tissot', 18000000.00, 0.00, N'Đồng hồ Cơ cao cấp', N'Nam', N'Thụy Sĩ', N'Automatic', N'Thép không gỉ', N'dong_ho_cao_cap_2.jpg', 15, '2023-01-04'),
(3, N'Casio B640WD-1AVT', 8, N'Casio', 1800000.00, 0.00, N'Đồng hồ Điện tử, retro', N'Unisex', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_thoi_trang_1.jpg', 45, '2023-01-07'),
(4, N'Citizen Eco-Drive Automatic', 4, N'Citizen', 8500000.00, 0.00, N'Đồng hồ cơ công nghệ Eco-Drive, không cần thay pin, thân thiện môi trường.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ', N'dong_ho_co_1.jpg', 33, '2023-01-08'),
(5, N'Orient Sun & Moon', 9, N'Orient', 9200000.00, 0.00, N'Đồng hồ cơ cao cấp, hiển thị lịch ngày đêm.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_cao_cap_3.jpg', 28, '2023-01-11'),
(6, N'Rolex Submariner Date', 9, N'Rolex', 300000000.00, 0.00, N'Đồng hồ lặn cao cấp, biểu tượng của sự sang trọng.', N'Nam', N'Thụy Sĩ', N'Automatic', N'Thép không gỉ', N'dong_ho_cao_cap_4.jpg', 12, '2023-01-15'),
(7, N'DW Classic Petite Ashfield', 2, N'DW', 3200000.00, 0.00, N'Đồng hồ nữ thời trang, dây lưới kim loại đen sang trọng.', N'Nữ', N'Thụy Điển', N'Quartz', N'Thép không gỉ', N'dong_ho_nu_1.jpg', 85, '2023-01-18'),
(8, N'Casio G-Shock GA-2100', 6, N'Casio', 3000000.00, 0.00, N'Đồng hồ thể thao, bền bỉ, chống va đập, mặt carbon.', N'Nam', N'Nhật Bản', N'Quartz', N'Nhựa/Carbon', N'dong_ho_the_thao_1.jpg', 72, '2023-01-20'),
(9, N'Omega Seamaster Diver 300M', 9, N'Omega', 160000000.00, 0.00, N'Đồng hồ lặn chuyên nghiệp, dòng sản phẩm James Bond.', N'Nam', N'Thụy Sĩ', N'Automatic', N'Thép không gỉ', N'dong_ho_cao_cap_5.jpg', 18, '2023-01-24'),
(10, N'Anne Klein Diamond Accented', 2, N'Anne Klein', 2200000.00, 0.00, N'Đồng hồ nữ thời trang, đính kim cương nhỏ tại vị trí 12 giờ.', N'Nữ', N'Mỹ', N'Quartz', N'Kim loại', N'dong_ho_nu_2.jpg', 68, '2023-01-28'),
(11, N'Bulova Wilton Automatic', 4, N'Bulova', 9500000.00, 0.00, N'Đồng hồ cơ, thiết kế lộ máy (open heart) độc đáo.', N'Nam', N'Mỹ', N'Automatic', N'Thép không gỉ', N'dong_ho_co_2.jpg', 29, '2023-02-01'),
(12, N'Citizen Eco-Drive FE1190-83A', 2, N'Citizen', 4800000.00, 0.00, N'Đồng hồ nữ Eco-Drive, không cần pin, dây kim loại vàng hồng.', N'Nữ', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_nu_3.jpg', 55, '2023-02-04'),
(13, N'Casio Baby-G BGD-560CU-9DR', 6, N'Casio', 2500000.00, 0.00, N'Đồng hồ thể thao nữ, chống nước tốt, màu vàng chanh.', N'Nữ', N'Nhật Bản', N'Quartz', N'Nhựa', N'dong_ho_the_thao_2.jpg', 90, '2023-02-08'),
(14, N'Citizen Promaster Aqualand BN2036-14E', 6, N'Citizen', 11000000.00, 0.00, N'Đồng hồ lặn chuyên nghiệp, Eco-Drive, cảm biến độ sâu.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ/Dây cao su', N'dong_ho_the_thao_3.jpg', 25, '2023-02-12'),
(15, N'Orient Classic Automatic', 4, N'Orient', 7500000.00, 0.00, N'Đồng hồ cơ cổ điển, mặt kính khoáng.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_co_3.jpg', 40, '2023-02-16'),
(16, N'Citizen Eco-Drive AW1232-04A', 5, N'Citizen', 4500000.00, 0.00, N'Đồng hồ Quartz Eco-Drive, dây da cá sấu.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_quartz_1.jpg', 50, '2023-02-20'),
(17, N'Omega Speedmaster Moonwatch', 9, N'Omega', 190000000.00, 0.00, N'Đồng hồ Chronograph nổi tiếng, gắn liền với các chuyến bay vào không gian.', N'Nam', N'Thụy Sĩ', N'Manual', N'Thép không gỉ', N'dong_ho_cao_cap_6.jpg', 14, '2023-02-24'),
(18, N'DW Classic Sheffield', 1, N'DW', 3500000.00, 0.00, N'Đồng hồ nam thời trang, mặt tối giản, dây da đen.', N'Nam', N'Thụy Điển', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_nam_2.jpg', 55, '2023-02-28'),
(19, N'Casio Vintage A168WA-1WDT', 8, N'Casio', 1600000.00, 0.00, N'Đồng hồ điện tử retro, dây kim loại bạc.', N'Unisex', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_thoi_trang_2.jpg', 95, '2023-03-04'),
(20, N'Longines La Grande Classique', 9, N'Longines', 22000000.00, 0.00, N'Đồng hồ mỏng, mặt tối giản, mang tính biểu tượng cao về thanh lịch.', N'Nữ', N'Thụy Sĩ', N'Quartz', N'Thép không gỉ', N'dong_ho_cao_cap_7.jpg', 19, '2023-03-08'),
(21, N'Seiko 5 Automatic SNK809K2', 4, N'Seiko', 3800000.00, 0.00, N'Đồng hồ cơ quân đội, giá phải chăng, bền bỉ.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Dây vải', N'dong_ho_co_4.jpg', 40, '2023-03-12'),
(22, N'DW Classic Bangle & Watch Set', 3, N'DW', 6800000.00, 0.00, N'Bộ đôi đồng hồ và vòng tay cho cặp đôi.', N'Đôi', N'Thụy Điển', N'Quartz', N'Thép không gỉ', N'dong_ho_doi_1.jpg', 25, '2023-03-16'),
(23, N'Orient Bambino V4 Automatic', 4, N'Orient', 6500000.00, 0.00, N'Đồng hồ cơ cổ điển, mặt kính cong (domed crystal).', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_co_5.jpg', 38, '2023-03-20'),
(24, N'Casio Couple Set MTP-V005D & LTP-V005D', 3, N'Casio', 1800000.00, 0.00, N'Đồng hồ đôi giá rẻ, dây kim loại cơ bản.', N'Đôi', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_doi_2.jpg', 60, '2023-03-24'),
(25, N'Tissot PRX Powermatic 80', 9, N'Tissot', 18500000.00, 0.00, N'Đồng hồ cơ cao cấp, thiết kế thập niên 70, dự trữ năng lượng 80 giờ.', N'Nam', N'Thụy Sĩ', N'Automatic', N'Thép không gỉ', N'dong_ho_cao_cap_8.jpg', 21, '2023-03-28'),
(26, N'DW Couple Cuff & Watch', 3, N'DW', 7800000.00, 0.00, N'Bộ đôi đồng hồ và vòng tay cho cặp đôi, mặt lưới.', N'Đôi', N'Thụy Điển', N'Quartz', N'Thép không gỉ', N'dong_ho_doi_3.jpg', 30, '2023-04-01'),
(27, N'Citizen Eco-Drive Chronograph CA0620-54A', 6, N'Citizen', 8000000.00, 0.00, N'Đồng hồ thể thao Chronograph, Eco-Drive.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_the_thao_4.jpg', 33, '2023-04-05'),
(28, N'DW Classic Couple Set', 3, N'DW', 7000000.00, 0.00, N'Đồng hồ đôi mặt trắng, dây da classic.', N'Đôi', N'Thụy Điển', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_doi_4.jpg', 36, '2023-04-09'),
(29, N'Bulova Couple Set 96P198 & 96A204', 3, N'Bulova', 12000000.00, 0.00, N'Bộ đôi đồng hồ Bulova, dây kim loại.', N'Đôi', N'Mỹ', N'Quartz', N'Thép không gỉ', N'dong_ho_doi_5.jpg', 23, '2023-04-13'),
(30, N'Orient Couple Set RA-AG0020S & RA-AG0020S', 3, N'Orient', 14000000.00, 0.00, N'Đồng hồ đôi cơ lộ máy, mặt vàng hồng.', N'Đôi', N'Nhật Bản', N'Automatic', N'Thép không gỉ', N'dong_ho_doi_6.jpg', 28, '2023-04-17'),
(31, N'Tissot Couple T099.407.16.037.00 & T099.207.16.037.00', 3, N'Tissot', 35000000.00, 0.00, N'Bộ đôi đồng hồ Tissot cơ Powermatic 80 cao cấp.', N'Đôi', N'Thụy Sĩ', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_doi_7.jpg', 18, '2023-04-21'),
(32, N'Bulova Classic Automatic', 4, N'Bulova', 8800000.00, 0.00, N'Đồng hồ cơ cổ điển, mặt trắng, số La Mã.', N'Nam', N'Mỹ', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_co_6.jpg', 29, '2023-04-25'),
(33, N'Orient Mako II Automatic', 6, N'Orient', 6500000.00, 0.00, N'Đồng hồ lặn tự động, giá phải chăng.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ', N'dong_ho_the_thao_5.jpg', 40, '2023-04-29'),
(34, N'Citizen Promaster Diver BN0150-10E', 6, N'Citizen', 7000000.00, 0.00, N'Đồng hồ lặn Eco-Drive, chịu nước 200m.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ/Dây cao su', N'dong_ho_the_thao_6.jpg', 35, '2023-05-03'),
(35, N'Tissot Chrono XL Classic', 6, N'Tissot', 9800000.00, 0.00, N'Đồng hồ thể thao Chronograph cỡ lớn.', N'Nam', N'Thụy Sĩ', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_the_thao_7.jpg', 28, '2023-05-07'),
(36, N'Citizen Promaster Skyhawk JY8078-52L', 6, N'Citizen', 15000000.00, 0.00, N'Đồng hồ phi công, nhiều chức năng, công nghệ Radio Controlled.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_the_thao_8.jpg', 16, '2023-05-11'),
(37, N'Citizen Caliber 8700 BL8000-54L', 9, N'Citizen', 8800000.00, 0.00, N'Đồng hồ cao cấp, lịch vạn niên, Eco-Drive.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_cao_cap_9.jpg', 19, '2023-05-15'),
(38, N'Orient Star Classic Automatic', 9, N'Orient', 15000000.00, 0.00, N'Đồng hồ cơ cao cấp Orient Star, mặt kính sapphire.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_cao_cap_10.jpg', 24, '2023-05-19'),
(39, N'Seiko Presage SSA347J1 Cocktail Time', 9, N'Seiko', 12500000.00, 0.00, N'Đồng hồ Cơ cao cấp, kim Power Reserve.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ', N'dong_ho_cao_cap_11.jpg', 22, '2023-05-23'),
(40, N'Tissot V8 Quartz Chronograph', 6, N'Tissot', 7500000.00, 0.00, N'Đồng hồ thể thao Chronograph, mặt lớn, phong cách đua xe.', N'Nam', N'Thụy Sĩ', N'Quartz', N'Thép không gỉ', N'dong_ho_the_thao_9.jpg', 31, '2023-05-27'),
(41, N'Casio Quartz Classic MTP-1302D-7A1V', 5, N'Casio', 2000000.00, 0.00, N'Đồng hồ Quartz cổ điển, dây kim loại, mặt trắng.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_quartz_2.jpg', 98, '2023-05-31'),
(42, N'Orient Star Semi Skeleton RK-AT0004S', 9, N'Orient', 20000000.00, 0.00, N'Đồng hồ cơ cao cấp, lộ máy một phần, kim Power Reserve.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ', N'dong_ho_cao_cap_12.jpg', 17, '2023-06-04'),
(43, N'Seiko Prospex SRPD25K1 Monster', 6, N'Seiko', 9000000.00, 0.00, N'Đồng hồ lặn tự động, biệt danh "Monster", thiết kế hầm hố.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ', N'dong_ho_the_thao_10.jpg', 26, '2023-06-08'),
(44, N'Citizen Eco-Drive Radio Controlled AT8110-61E', 9, N'Citizen', 12500000.00, 0.00, N'Đồng hồ cao cấp, chức năng điều khiển bằng sóng Radio, giờ thế giới.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_cao_cap_13.jpg', 20, '2023-06-12'),
(45, N'Fossil Neely Chronograph', 8, N'Fossil', 4500000.00, 0.00, N'Đồng hồ nữ thời trang, Chronograph nhỏ, dây da.', N'Nữ', N'Mỹ', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_thoi_trang_3.jpg', 55, '2023-06-16'),
(46, N'Michael Kors Darci Watch', 8, N'MK', 5200000.00, 0.00, N'Đồng hồ nữ thời trang, đính đá, dây kim loại vàng hồng.', N'Nữ', N'Mỹ', N'Quartz', N'Thép không gỉ', N'dong_ho_thoi_trang_4.jpg', 48, '2023-06-20'),
(47, N'Skagen Annelie Mesh Blue', 8, N'Skagen', 3200000.00, 0.00, N'Đồng hồ Quartz thời trang, mặt xanh dương, phong cách tối giản.', N'Nữ', N'Đan Mạch', N'Quartz', N'Thép không gỉ', N'dong_ho_thoi_trang_5.jpg', 80, '2023-06-24'),
(48, N'Versace V-Ray VARY01017', 9, N'Versace', 25000000.00, 0.00, N'Đồng hồ cao cấp Versace, thiết kế độc đáo.', N'Nam', N'Ý', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_cao_cap_14.jpg', 13, '2023-06-28'),
(49, N'Rado Coupole Classic Quartz', 9, N'Rado', 22000000.00, 0.00, N'Đồng hồ cao cấp Rado, mặt kính sapphire lồi.', N'Nam', N'Thụy Sĩ', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_cao_cap_15.jpg', 12, '2023-07-02'),
(50, N'Longines Master Collection Automatic', 9, N'Longines', 40000000.00, 0.00, N'Đồng hồ cơ cao cấp Longines, bộ sưu tập Master Collection.', N'Nam', N'Thụy Sĩ', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_cao_cap_16.jpg', 10, '2023-07-06'),
(51, N'G-Shock Mudmaster GWG-1000', 6, N'G-Shock', 9500000.00, 0.00, N'Đồng hồ thể thao chống bùn, chống sốc, kết hợp kim và số.', N'Nam', N'Nhật Bản', N'Quartz', N'Nhựa', N'dong_ho_the_thao_11.jpg', 30, '2023-07-10'),
(52, N'Seiko Prospex PADI Solar Diver SNE435P1', 6, N'Seiko', 6800000.00, 0.00, N'Đồng hồ lặn Solar PADI, không cần thay pin.', N'Nam', N'Nhật Bản', N'Solar Quartz', N'Thép không gỉ', N'dong_ho_the_thao_12.jpg', 35, '2023-07-14'),
(53, N'Citizen Promaster Marine Dive BN0190-82E', 6, N'Citizen', 6000000.00, 0.00, N'Đồng hồ lặn Eco-Drive, dây kim loại.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_the_thao_13.jpg', 42, '2023-07-18'),
(54, N'Orient Classic Power Reserve FDJ05001W', 4, N'Orient', 10500000.00, 0.00, N'Đồng hồ cơ, hiển thị dự trữ năng lượng.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_co_7.jpg', 27, '2023-07-22'),
(55, N'DW Iconic Link Lumine', 8, N'DW', 5500000.00, 0.00, N'Đồng hồ nữ thời trang, dây kim loại bạc, mặt khảm trai.', N'Nữ', N'Thụy Điển', N'Quartz', N'Thép không gỉ', N'dong_ho_thoi_trang_6.jpg', 50, '2023-07-26'),
(56, N'Citizen Eco-Drive BL5400-52A', 9, N'Citizen', 8500000.00, 0.00, N'Đồng hồ cao cấp, Chronograph, lịch vạn niên, Eco-Drive.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_cao_cap_17.jpg', 23, '2023-07-30'),
(57, N'Garmin Forerunner 955 Solar', 7, N'Garmin', 15990000.00, 0.00, N'Đồng hồ thông minh thể thao, sạc bằng năng lượng mặt trời.', N'Unisex', N'Mỹ', N'Digital', N'Polymer', N'dong_ho_thong_minh_1.jpg', 15, '2023-08-03'),
(58, N'Apple Watch SE (2022)', 7, N'Apple', 7500000.00, 0.00, N'Đồng hồ thông minh cơ bản của Apple, nhiều tính năng sức khỏe.', N'Unisex', N'Mỹ', N'Digital', N'Nhôm', N'dong_ho_thong_minh_2.jpg', 90, '2023-08-07'),
(59, N'Samsung Galaxy Watch 5 Pro', 7, N'Samsung', 10990000.00, 0.00, N'Đồng hồ thông minh cao cấp, bền bỉ, GPS, pin trâu.', N'Unisex', N'Hàn Quốc', N'Digital', N'Titanium', N'dong_ho_thong_minh_3.jpg', 30, '2023-08-11'),
(60, N'Tag Heuer Aquaracer Professional 200M', 9, N'Tag Heuer', 55000000.00, 0.00, N'Đồng hồ lặn cao cấp, chịu nước 200m.', N'Nam', N'Thụy Sĩ', N'Quartz', N'Thép không gỉ', N'dong_ho_cao_cap_18.jpg', 11, '2023-08-15'),
(61, N'Seiko 5 Sports SRPD65K1', 6, N'Seiko', 7000000.00, 0.00, N'Đồng hồ thể thao Seiko 5, phiên bản màu đen.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ', N'dong_ho_the_thao_14.jpg', 35, '2023-08-19'),
(62, N'Apple Watch Series 8 GPS', 7, N'Apple', 11990000.00, 0.00, N'Đồng hồ thông minh mới nhất của Apple, có tính năng đo nhiệt độ.', N'Unisex', N'Mỹ', N'Digital', N'Nhôm', N'dong_ho_thong_minh_4.jpg', 80, '2023-08-23'),
(63, N'Garmin Instinct 2 Solar', 7, N'Garmin', 9500000.00, 0.00, N'Đồng hồ thông minh dã ngoại, sạc bằng năng lượng mặt trời, siêu bền.', N'Unisex', N'Mỹ', N'Digital', N'Polymer', N'dong_ho_thong_minh_5.jpg', 25, '2023-08-27'),
(64, N'Amazfit GTR 4 Active', 7, N'Amazfit', 4500000.00, 0.00, N'Đồng hồ thông minh giá phải chăng, màn hình AMOLED, hơn 117 chế độ thể thao.', N'Unisex', N'Trung Quốc', N'Digital', N'Nhôm/Silicone', N'dong_ho_thong_minh_6.jpg', 90, '2023-08-31'),
(65, N'Citizen Eco-Drive BM8180-03E', 5, N'Citizen', 3500000.00, 0.00, N'Đồng hồ Quartz Eco-Drive, mặt đen, dây vải dù.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ/Vải', N'dong_ho_quartz_3.jpg', 60, '2023-09-04'),
(66, N'Casio AE-1200WH-1AVDF', 8, N'Casio', 1800000.00, 0.00, N'Đồng hồ điện tử "World Time" (Giờ thế giới), phong cách Casio Royale.', N'Unisex', N'Nhật Bản', N'Digital', N'Nhựa', N'dong_ho_thoi_trang_7.jpg', 75, '2023-09-08'),
(67, N'Orient Classic RA-AG0005L10B', 4, N'Orient', 6500000.00, 0.00, N'Đồng hồ cơ, mặt xanh dương, dây da.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_co_8.jpg', 38, '2023-09-12'),
(68, N'Seiko 5 Sports SRPH31K1', 6, N'Seiko', 7500000.00, 0.00, N'Đồng hồ thể thao Seiko 5, mặt xanh rêu, dây vải.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Vải', N'dong_ho_the_thao_15.jpg', 32, '2023-09-16'),
(69, N'DW Quadro Pressed Sheffield', 2, N'DW', 4500000.00, 0.00, N'Đồng hồ nữ mặt vuông, dây da đen, tối giản.', N'Nữ', N'Thụy Điển', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_nu_4.jpg', 65, '2023-09-20'),
(70, N'Fossil Grant Chronograph FS4813', 8, N'Fossil', 5000000.00, 0.00, N'Đồng hồ nam thời trang, Chronograph, số La Mã.', N'Nam', N'Mỹ', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_thoi_trang_8.jpg', 48, '2023-09-24'),
(71, N'Tissot T-Race T115.417.37.051.00', 6, N'Tissot', 10500000.00, 0.00, N'Đồng hồ thể thao T-Race, lấy cảm hứng từ xe đua.', N'Nam', N'Thụy Sĩ', N'Quartz', N'Thép không gỉ/Silicone', N'dong_ho_the_thao_16.jpg', 22, '2023-09-28'),
(72, N'Citizen Eco-Drive Promaster Sky PMX56-2811', 6, N'Citizen', 12000000.00, 0.00, N'Đồng hồ phi công Eco-Drive, vỏ Titanium siêu nhẹ.', N'Nam', N'Nhật Bản', N'Quartz', N'Titanium', N'dong_ho_the_thao_17.jpg', 15, '2023-10-02'),
(73, N'DW Petite Lumine', 8, N'DW', 5200000.00, 0.00, N'Đồng hồ nữ thời trang, dây kim loại, đính pha lê lấp lánh.', N'Nữ', N'Thụy Điển', N'Quartz', N'Thép không gỉ', N'dong_ho_thoi_trang_9.jpg', 55, '2023-10-06'),
(74, N'Orient Classic Sun & Moon RA-AS0101S10B', 4, N'Orient', 9000000.00, 0.00, N'Đồng hồ cơ, hiển thị Sun & Moon, mặt kính sapphire.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_co_9.jpg', 35, '2023-10-10'),
(75, N'Versus Versace Covent Garden VSPHJ0520', 8, N'Versus Versace', 6500000.00, 0.00, N'Đồng hồ nữ thời trang, logo sư tử nổi bật.', N'Nữ', N'Ý', N'Quartz', N'Thép không gỉ', N'dong_ho_thoi_trang_10.jpg', 42, '2023-10-14'),
(76, N'Citizen Eco-Drive AW1232-04A (Rose Gold)', 5, N'Citizen', 4500000.00, 0.00, N'Đồng hồ Quartz Eco-Drive, vỏ vàng hồng, dây da.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_quartz_4.jpg', 48, '2023-10-18'),
(77, N'Seiko Prospex "King Samurai" SRPE37K1', 6, N'Seiko', 12000000.00, 0.00, N'Đồng hồ lặn "King Samurai", mặt kính sapphire, vành bezel gốm.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ', N'dong_ho_the_thao_18.jpg', 20, '2023-10-22'),
(78, N'Orient Star Open Heart RE-AV0001S', 9, N'Orient', 21000000.00, 0.00, N'Đồng hồ cơ cao cấp, lộ máy, mặt kính sapphire.', N'Nam', N'Nhật Bản', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_cao_cap_19.jpg', 14, '2023-10-26'),
(79, N'Bulova Sutton Skeleton 97A138', 4, N'Bulova', 11000000.00, 0.00, N'Đồng hồ cơ lộ máy hoàn toàn, vỏ vàng hồng, dây da cá sấu.', N'Nam', N'Mỹ', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_co_10.jpg', 18, '2023-10-30'),
(80, N'Citizen Eco-Drive Chronograph AT2140-55L', 6, N'Citizen', 8500000.00, 0.00, N'Đồng hồ thể thao Chronograph, Eco-Drive, mặt xanh.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_the_thao_19.jpg', 29, '2023-11-03'),
(81, N'Tissot Classic Dream T033.410.16.013.01', 5, N'Tissot', 4800000.00, 0.00, N'Đồng hồ Quartz cổ điển, tối giản, dây da.', N'Nam', N'Thụy Sĩ', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_quartz_5.jpg', 55, '2023-11-07'),
(82, N'Casio Quartz LA670WA-1DF Vintage', 8, N'Casio', 1500000.00, 0.00, N'Đồng hồ điện tử retro nữ, dây kim loại nhỏ.', N'Nữ', N'Nhật Bản', N'Digital', N'Thép không gỉ', N'dong_ho_thoi_trang_11.jpg', 90, '2023-11-11'),
(83, N'Tag Heuer Carrera Calibre 5 Automatic', 9, N'Tag Heuer', 85000000.00, 0.00, N'Đồng hồ cơ cao cấp Carrera, thiết kế sang trọng.', N'Nam', N'Thụy Sĩ', N'Automatic', N'Thép không gỉ', N'dong_ho_cao_cap_20.jpg', 9, '2023-11-15'),
(84, N'Omega De Ville Prestige Co-Axial', 9, N'Omega', 150000000.00, 0.00, N'Đồng hồ cao cấp, công nghệ Co-Axial, thiết kế thanh lịch.', N'Nam', N'Thụy Sĩ', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_cao_cap_21.jpg', 12, '2023-11-19'),
(85, N'Citizen Promaster Tough BN0211-50E', 6, N'Citizen', 7500000.00, 0.00, N'Đồng hồ thể thao siêu bền, vỏ monocoque, Eco-Drive.', N'Nam', N'Nhật Bản', N'Quartz', N'Titanium', N'dong_ho_the_thao_20.jpg', 25, '2023-11-23'),
(86, N'Bulova Classic Automatic 98A166', 4, N'Bulova', 9000000.00, 0.00, N'Đồng hồ cơ, mặt xanh navy, dây lưới.', N'Nam', N'Mỹ', N'Automatic', N'Thép không gỉ', N'dong_ho_co_11.jpg', 29, '2023-11-27'),
(87, N'Citizen Eco-Drive World Chronograph AT8020-03L', 9, N'Citizen', 9900000.00, 0.00, N'Đồng hồ cao cấp, vỏ thép không gỉ mạ PVD Titanium siêu bền.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_cao_cap_22.jpg', 23, '2023-12-01'),
(88, N'Tissot Heritage Visodate Automatic', 4, N'Tissot', 16000000.00, 0.00, N'Đồng hồ cơ cổ điển, mặt kính sapphire lồi, hiển thị ngày và thứ.', N'Nam', N'Thụy Sĩ', N'Automatic', N'Thép không gỉ/Da', N'dong_ho_co_12.jpg', 17, '2023-12-05'),
(89, N'Tissot PR 100 Chronograph T101.417.16.031.00', 5, N'Tissot', 7000000.00, 0.00, N'Đồng hồ Quartz Chronograph, độ chính xác cao.', N'Nam', N'Thụy Sĩ', N'Quartz', N'Thép không gỉ/Da', N'dong_ho_quartz_6.jpg', 40, '2023-12-09'),
(90, N'Casio Edifice EFR-539D-1AVUDF', 6, N'Casio', 4500000.00, 0.00, N'Đồng hồ thể thao Edifice Chronograph, dây kim loại.', N'Nam', N'Nhật Bản', N'Quartz', N'Thép không gỉ', N'dong_ho_the_thao_21.jpg', 50, '2023-12-13'),
(91, N'Bulova Lunar Pilot Chronograph 96B258', 6, N'Bulova', 13000000.00, 0.00, N'Đồng hồ Chronograph, phiên bản tái hiện đồng hồ được sử dụng trên mặt trăng.', N'Nam', N'Mỹ', N'Quartz', N'Thép không gỉ/Dây vải', N'dong_ho_the_thao_22.jpg', 16, '2023-12-17'),
(92, N'DW Iconic Link Emerald', 8, N'DW', 6200000.00, 0.00, N'Đồng hồ nữ thời trang, mặt màu xanh ngọc lục bảo, sang trọng.', N'Nữ', N'Thụy Điển', N'Quartz', N'Thép không gỉ', N'dong_ho_thoi_trang_12.jpg', 50, '2023-12-21'),
(93, N'Xiaomi Mi Watch Color 2', 7, N'Xiaomi', 3500000.00, 0.00, N'Đồng hồ thông minh giá rẻ, nhiều tính năng thể thao, màn hình AMOLED.', N'Unisex', N'Trung Quốc', N'Digital', N'Nhựa/Silicone', N'dong_ho_thong_minh_7.jpg', 120, '2023-12-25'),
(94, N'Huawei Watch GT 3 Pro', 7, N'Huawei', 6990000.00, 0.00, N'Đồng hồ thông minh cao cấp, pin siêu bền, theo dõi sức khỏe chuyên sâu.', N'Unisex', N'Trung Quốc', N'Digital', N'Titanium/Ceramic', N'dong_ho_thong_minh_8.jpg', 60, '2023-12-29'),
(95, N'Garmin Venu 2 Plus', 7, N'Garmin', 11990000.00, 0.00, N'Đồng hồ thông minh, màn hình AMOLED, có mic và loa, hỗ trợ gọi điện.', N'Unisex', N'Mỹ', N'Digital', N'Polymer/Silicone', N'dong_ho_thong_minh_9.jpg', 35, '2024-01-02'),
(96, N'Disney Kids Digital Watch (Mickey Mouse)', 10, N'Disney', 450000.00, 0.00, N'Đồng hồ điện tử trẻ em, hình ảnh nhân vật Disney.', N'Trẻ Em', N'Trung Quốc', N'Digital', N'Nhựa', N'dong_ho_tre_em_1.jpg', 150, '2024-01-06'),
(97, N'Hello Kitty Kids Analog Watch', 10, N'Hello Kitty', 550000.00, 0.00, N'Đồng hồ kim trẻ em, hình Hello Kitty dễ thương.', N'Trẻ Em', N'Trung Quốc', N'Quartz', N'Nhựa/Silicone', N'dong_ho_tre_em_2.jpg', 130, '2024-01-10'),
(98, N'Smartwatch for Kids (Pink)', 10, N'No Brand', 1200000.00, 0.00, N'Đồng hồ thông minh trẻ em, có chức năng gọi video, GPS.', N'Trẻ Em', N'Trung Quốc', N'Digital', N'Nhựa', N'dong_ho_tre_em_3.jpg', 70, '2024-01-14'),
(99, N'Garmin VIVOFIT JR 3 (Black Panther)', 10, N'Garmin', 2500000.00, 0.00, N'Đồng hồ thể thao trẻ em, theo dõi hoạt động, có các thử thách vui nhộn.', N'Trẻ Em', N'Mỹ', N'Digital', N'Silicone', N'dong_ho_tre_em_4.jpg', 45, '2024-01-18'),
(100, N'Smartwatch 4G cho Kids (Blue)', 10, N'No Brand', 1800000.00, 0.00, N'Đồng hồ thông minh trẻ em, kết nối 4G, camera, cuộc gọi khẩn cấp.', N'Trẻ Em', N'Trung Quốc', N'Digital', N'Nhựa/Silicone', N'dong_ho_tre_em_5.jpg', 55, '2024-01-22');