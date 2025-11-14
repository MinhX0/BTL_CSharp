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
    Price           BIGINT NOT NULL DEFAULT 0,
    DiscountPrice   BIGINT,
    Description     TEXT,
    Gender          VARCHAR(10),
    Origin          VARCHAR(50),
    MovementType    VARCHAR(50),
    Material        VARCHAR(100),
    Img             VARCHAR(255),
    IsActive        TINYINT(1) NOT NULL DEFAULT 1,
    StockQuantity   INT NOT NULL DEFAULT 0,
    CreatedDate     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_product_category
        FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT chk_product_price CHECK (Price >= 0),
    CONSTRAINT chk_product_discount CHECK (DiscountPrice IS NULL OR (DiscountPrice >= 0 AND DiscountPrice < Price)),
    CONSTRAINT chk_product_stock CHECK (StockQuantity >= 0)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

CREATE TABLE Admin (
    AdminID         INT AUTO_INCREMENT PRIMARY KEY,
    Username        VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash    VARCHAR(255) NOT NULL,
    Role            VARCHAR(20),
    CONSTRAINT chk_admin_role CHECK (Role IN ('Owner', 'Manager', 'Staff'))
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

CREATE TABLE `Order` (
    OrderID         INT AUTO_INCREMENT PRIMARY KEY,
    CustomerID      INT NOT NULL,
    OrderDate       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TotalAmount     BIGINT NOT NULL,
    PaymentMethod   VARCHAR(50),
    ShippingAddress VARCHAR(255),
    Status          VARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT fk_order_customer
        FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT chk_order_total CHECK (TotalAmount >= 0),
    CONSTRAINT chk_order_status CHECK (Status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'))
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
    Price           BIGINT NOT NULL,
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


-- Category Insert --

INSERT INTO `category` (`CategoryName`, `Description`, `Img`) VALUES
    ('Đồng hồ Nam', 'Phong cách mạnh mẽ, lịch lãm, phù hợp doanh nhân và người đi làm văn phòng.', 'dong_ho_nam.jpg'),
    ('Đồng hồ Nữ', 'Thiết kế tinh tế, sang trọng, dành cho phái đẹp.', 'dong_ho_nu.jpg'),
    ('Đồng hồ Đôi', 'Bộ đôi đồng hồ nam - nữ dành cho các cặp đôi.', 'dong_ho_doi.jpg'),
    ('Đồng hồ Cơ', 'Vận hành bằng cơ học, không cần pin, sang trọng và đẳng cấp.', 'dong_ho_co.jpg'),
    ('Đồng hồ Quartz', 'Hoạt động bằng pin, chính xác và phổ biến nhất hiện nay.', 'dong_ho_quartz.jpg'),
    ('Đồng hồ Thể Thao', 'Thiết kế năng động, bền bỉ, phù hợp cho người yêu vận động.', 'dong_ho_the_thao.jpg'),
    ('Đồng hồ Thông Minh', 'Kết nối Bluetooth, theo dõi sức khỏe, thông báo thông minh.', 'dong_ho_thong_minh.jpg'),
    ('Đồng hồ Thời Trang', 'Mẫu mã hiện đại, màu sắc đa dạng, hợp xu hướng.', 'dong_ho_thoi_trang.jpg'),
    ('Đồng hồ Cao Cấp', 'Thương hiệu sang trọng, chất liệu cao cấp, đẳng cấp doanh nhân.', 'dong_ho_cao_cap.jpg'),
    ('Đồng hồ Trẻ Em', 'Thiết kế đáng yêu, màu sắc tươi sáng, phù hợp cho trẻ nhỏ.', 'dong_ho_tre_em.jpg');

-- Product Insert --
INSERT INTO `product` (`ProductName`, `CategoryID`, `Brand`, `Price`, `DiscountPrice`, `Description`, `Gender`, `Origin`, `MovementType`, `Material`, `Img`, `StockQuantity`) VALUES
    ('Seiko Presage Cocktail Time', '1', 'Seiko', '12500000', NULL, 'Đồng hồ cơ tự động dành cho doanh nhân, mặt kính cong Hardlex độc đáo.', 'Nam', 'Nhật Bản', 'Automatic', 'Thép không gỉ/Da', 'dong_ho_nam_1.jpg', '45'),
    ('Casio Edifice EFR', '1', 'Casio', '3500000', NULL, 'Đồng hồ 6 kim thể thao, vỏ thép không gỉ, độ chịu nước 100m.', 'Nam', 'Nhật Bản', 'Quartz', 'Thép không gỉ', 'dong_ho_nam_2.jpg', '60'),
    ('Tissot Le Locle Automatic', '1', 'Tissot', '18000000', NULL, 'Đồng hồ cơ Thụy Sĩ sang trọng, mặt số vân Guilloche cổ điển.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ/Da', 'dong_ho_nam_3.jpg', '20'),
    ('Fossil Grant Chronograph', '1', 'Fossil', '4100000', NULL, 'Đồng hồ thời trang Mỹ, phong cách vintage, mặt số La Mã.', 'Nam', 'Mỹ', 'Quartz', 'Thép không gỉ/Da', 'dong_ho_nam_4.jpg', '50'),
    ('Citizen Eco-Drive BM8475', '1', 'Citizen', '4800000', NULL, 'Sử dụng công nghệ Eco-Drive, không cần thay pin, thiết kế field watch mạnh mẽ.', 'Nam', 'Nhật Bản', 'Eco-Drive Quartz', 'Vải dù/Thép không gỉ', 'dong_ho_nam_5.jpg', '75'),
    ('Orient Sun & Moon 4', '1', 'Orient', '9500000', NULL, 'Chức năng lịch ngày/đêm độc đáo, máy cơ Nhật Bản.', 'Nam', 'Nhật Bản', 'Automatic', 'Thép không gỉ/Da', 'dong_ho_nam_6.jpg', '35'),
    ('Rolex Submariner Replica', '1', 'No Brand', '2000000', NULL, 'Phiên bản sao chép (replica) với chất lượng tốt, chống nước cơ bản.', 'Nam', 'Trung Quốc', 'Automatic', 'Thép không gỉ', 'dong_ho_nam_7.jpg', '15'),
    ('Hamilton Khaki Field', '1', 'Hamilton', '15000000', NULL, 'Đồng hồ quân đội kinh điển của Thụy Sĩ, độ bền cao, dễ đọc.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ/Vải', 'dong_ho_nam_8.jpg', '22'),
    ('MVMT Black Leather', '1', 'MVMT', '3800000', NULL, 'Phong cách tối giản, mặt số đen, dây da đen, phù hợp giới trẻ.', 'Nam', 'Mỹ', 'Quartz', 'Hợp kim/Da', 'dong_ho_nam_9.jpg', '40'),
    ('Adriatica Classic A8202', '1', 'Adriatica', '6200000', NULL, 'Thương hiệu Thụy Sĩ với thiết kế cổ điển, lịch ngày đơn giản.', 'Nam', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ', 'dong_ho_nam_10.jpg', '28'),
    ('Citizen Eco-Drive Elegance', '2', 'Citizen', '6800000', NULL, 'Sử dụng công nghệ Eco-Drive, mặt khảm trai, thiết kế mỏng nhẹ, sang trọng.', 'Nữ', 'Nhật Bản', 'Eco-Drive Quartz', 'Thép không gỉ', 'dong_ho_nu_1.jpg', '60'),
    ('Daniel Wellington Petite Melrose', '2', 'DW', '3200000', NULL, 'Đồng hồ nữ mặt nhỏ, dây lưới kim loại vàng hồng, thanh lịch.', 'Nữ', 'Thụy Điển', 'Quartz', 'Thép không gỉ/Mesh', 'dong_ho_nu_2.jpg', '85'),
    ('Michael Kors Darci MK3402', '2', 'MK', '5500000', NULL, 'Đồng hồ thời trang Mỹ, viền đính đá lấp lánh, mạ vàng hồng.', 'Nữ', 'Mỹ', 'Quartz', 'Thép không gỉ', 'dong_ho_nu_3.jpg', '45'),
    ('Citizen L Ambiluna EM0640', '2', 'Citizen', '11000000', NULL, 'Thiết kế trang sức độc đáo, sử dụng năng lượng ánh sáng Eco-Drive.', 'Nữ', 'Nhật Bản', 'Eco-Drive Quartz', 'Thép không gỉ/Da', 'dong_ho_nu_4.jpg', '25'),
    ('Skagen Signatur Leather', '2', 'Skagen', '2900000', NULL, 'Phong cách Đan Mạch tối giản, dây da mỏng, nhẹ nhàng.', 'Nữ', 'Đan Mạch', 'Quartz', 'Hợp kim/Da', 'dong_ho_nu_5.jpg', '70'),
    ('Olym Pianus OP2472L', '2', 'Olym Pianus', '2800000', NULL, 'Đồng hồ Quartz giá phải chăng, thiết kế mặt tròn cơ bản.', 'Nữ', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ', 'dong_ho_nu_6.jpg', '55'),
    ('Versace V-Palazzo Pink', '2', 'Versace', '35000000', NULL, 'Thương hiệu thời trang Ý, biểu tượng Medusa lớn, dây da cá sấu.', 'Nữ', 'Ý', 'Quartz', 'Vàng/Da cá sấu', 'dong_ho_nu_7.jpg', '12'),
    ('Calvin Klein Seduce K4E2N111', '2', 'CK', '5200000', NULL, 'Thiết kế vòng tay trang sức, mặt chữ nhật nhỏ, rất thời trang.', 'Nữ', 'Mỹ', 'Quartz', 'Thép không gỉ', 'dong_ho_nu_8.jpg', '38'),
    ('Marc Jacobs Riley MJ3540', '2', 'Marc Jacobs', '4300000', NULL, 'Đồng hồ nữ trẻ trung, mặt số lớn, kim giây màu nổi bật.', 'Nữ', 'Mỹ', 'Quartz', 'Thép không gỉ', 'dong_ho_nu_9.jpg', '42'),
    ('Longines La Grande Classique', '2', 'Longines', '22000000', NULL, 'Mẫu đồng hồ siêu mỏng, mặt số tối giản, mang tính biểu tượng của sự thanh lịch.', 'Nữ', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ', 'dong_ho_nu_10.jpg', '18'),
    ('Daniel Wellington Classic Couple', '3', 'DW', '4500000', NULL, 'Bộ đôi đồng hồ mặt tối giản, dây lưới kim loại thời thượng.', 'Đôi', 'Thụy Điển', 'Quartz', 'Thép không gỉ/Mesh', 'dong_ho_doi_1.jpg', '30'),
    ('Seiko Couple Set SNKL & SYMG', '3', 'Seiko', '9800000', NULL, 'Bộ đôi đồng hồ cơ tự động, thiết kế đồng nhất.', 'Đôi', 'Nhật Bản', 'Automatic', 'Thép không gỉ', 'dong_ho_doi_2.jpg', '25'),
    ('Citizen Couple Eco-Drive BM/EM', '3', 'Citizen', '10500000', NULL, 'Bộ đôi Eco-Drive, không cần thay pin, thiết kế đơn giản, thanh lịch.', 'Đôi', 'Nhật Bản', 'Eco-Drive Quartz', 'Thép không gỉ', 'dong_ho_doi_3.jpg', '40'),
    ('Casio General A158W-1D & LA670W', '3', 'Casio', '1800000', NULL, 'Bộ đôi đồng hồ điện tử retro giá rẻ, chống nước cơ bản.', 'Đôi', 'Nhật Bản', 'Digital Quartz', 'Nhựa/Thép', 'dong_ho_doi_4.jpg', '80'),
    ('Olym Pianus & OPA Couple', '3', 'OP', '5200000', NULL, 'Bộ đôi đồng hồ Quartz, thiết kế mạ vàng PVD sang trọng.', 'Đôi', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ', 'dong_ho_doi_5.jpg', '35'),
    ('Sunrise Couple Set', '3', 'Sunrise', '3500000', NULL, 'Bộ đôi đồng hồ giá bình dân, mặt kính Sapphire chống xước.', 'Đôi', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ', 'dong_ho_doi_6.jpg', '50'),
    ('Calvin Klein K2G2G1Z6 & K2G2G6Z6', '3', 'CK', '8500000', NULL, 'Thiết kế tối giản hiện đại của Thụy Sĩ, dây lưới độc đáo.', 'Đôi', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ/Mesh', 'dong_ho_doi_7.jpg', '28'),
    ('BULOVA Couple Diamond', '3', 'BULOVA', '12000000', NULL, 'Bộ đôi có đính kim cương nhỏ tại cọc số, sang trọng.', 'Đôi', 'Mỹ', 'Quartz', 'Thép không gỉ', 'dong_ho_doi_8.jpg', '22'),
    ('Longines Elegance L4.755.4 & L4.209.4', '3', 'Longines', '40000000', NULL, 'Bộ đôi cao cấp siêu mỏng, Thụy Sĩ.', 'Đôi', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ/Da', 'dong_ho_doi_9.jpg', '10'),
    ('Adriatica Couple A1156 & A3601', '3', 'Adriatica', '7800000', NULL, 'Bộ đôi đồng hồ Thụy Sĩ cổ điển, mặt số đơn giản, dễ đeo.', 'Đôi', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ', 'dong_ho_doi_10.jpg', '32'),
    ('Orient Bambino Gen 2', '4', 'Orient', '8900000', NULL, 'Máy cơ tự động (Automatic) Nhật Bản, kính cong cổ điển, lộ máy phía sau.', 'Nam', 'Nhật Bản', 'Automatic', 'Thép không gỉ/Da', 'dong_ho_co_1.jpg', '25'),
    ('Seiko 5 Sports Automatic SRPE', '4', 'Seiko', '7500000', NULL, 'Đồng hồ cơ 4R36, vỏ thép không gỉ, thiết kế lấy cảm hứng từ đồng hồ quân đội.', 'Nam', 'Nhật Bản', 'Automatic', 'Thép không gỉ', 'dong_ho_co_2.jpg', '30'),
    ('Orient Kamasu Dive Watch', '4', 'Orient', '6800000', NULL, 'Đồng hồ lặn (Diver) máy cơ, chống nước 200m, kính Sapphire chống xước.', 'Nam', 'Nhật Bản', 'Automatic', 'Thép không gỉ', 'dong_ho_co_3.jpg', '40'),
    ('Tissot Gentleman Powermatic 80', '4', 'Tissot', '25000000', NULL, 'Máy cơ Powermatic 80 Thụy Sĩ, trữ cót lên đến 80 giờ, kính Sapphire.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ', 'dong_ho_co_4.jpg', '15'),
    ('Citizen NJ0150 Automatic', '4', 'Citizen', '5500000', NULL, 'Đồng hồ cơ giá phải chăng của Citizen, thiết kế tối giản.', 'Nam', 'Nhật Bản', 'Automatic', 'Thép không gỉ', 'dong_ho_co_5.jpg', '55'),
    ('Hamilton Jazzmaster Viewmatic', '4', 'Hamilton', '18500000', NULL, 'Đồng hồ cơ Thụy Sĩ lịch lãm, mặt số đơn giản, máy H-10.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ/Da', 'dong_ho_co_6.jpg', '20'),
    ('Seiko Prospex Alpinist SPB197J1', '4', 'Seiko', '21000000', NULL, 'Đồng hồ thám hiểm chuyên nghiệp, máy cơ cao cấp 6R35, chống nước 200m.', 'Nam', 'Nhật Bản', 'Automatic', 'Thép không gỉ', 'dong_ho_co_7.jpg', '18'),
    ('Invicta Pro Diver 8926OB', '4', 'Invicta', '3200000', NULL, 'Đồng hồ lặn cơ học, phiên bản homage nổi tiếng.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ', 'dong_ho_co_8.jpg', '48'),
    ('Olym Pianus OP99083-82AGS-GL', '4', 'OP', '7000000', NULL, 'Đồng hồ cơ lộ máy (Skeleton) Thụy Sĩ, thiết kế độc đáo.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ/Da', 'dong_ho_co_9.jpg', '27'),
    ('Zenith Defy Classic', '4', 'Zenith', '99999999', NULL, 'Đồng hồ cơ Thụy Sĩ cao cấp, vỏ Titanium, độ chính xác cao.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Titanium', 'dong_ho_co_10.jpg', '5'),
    ('Casio F-91W Retro Classic', '5', 'Casio', '450000', NULL, 'Mẫu đồng hồ Quartz điện tử kinh điển, chức năng cơ bản.', 'Nam/Nữ', 'Nhật Bản', 'Digital Quartz', 'Nhựa Resin', 'dong_ho_quartz_1.jpg', '150'),
    ('Casio G-Shock DW-5600E', '5', 'G-Shock', '1800000', NULL, 'G-Shock vuông kinh điển, chống va đập, chống nước 200m.', 'Nam/Nữ', 'Nhật Bản', 'Digital Quartz', 'Nhựa Resin', 'dong_ho_quartz_2.jpg', '120'),
    ('Citizen Quartz Minimalist BI1050', '5', 'Citizen', '2500000', NULL, 'Đồng hồ Quartz pin, mặt số đơn giản, kim mỏng, dễ đeo hàng ngày.', 'Nam', 'Nhật Bản', 'Quartz', 'Thép không gỉ', 'dong_ho_quartz_3.jpg', '90'),
    ('Timex Expedition Field', '5', 'Timex', '1500000', NULL, 'Đồng hồ Quartz Mỹ, chức năng Indiglo (đèn nền sáng toàn mặt số), bền bỉ.', 'Nam', 'Mỹ', 'Quartz', 'Hợp kim/Vải', 'dong_ho_quartz_4.jpg', '70'),
    ('Fossil Neutra Chronograph', '5', 'Fossil', '4500000', NULL, 'Đồng hồ Quartz 6 kim, thiết kế Chronograph cổ điển, dây da.', 'Nam', 'Mỹ', 'Quartz', 'Thép không gỉ/Da', 'dong_ho_quartz_5.jpg', '55'),
    ('Seiko Quartz SGEH79P1', '5', 'Seiko', '3100000', NULL, 'Đồng hồ Quartz pin tiêu chuẩn, kính Sapphire chống trầy.', 'Nam', 'Nhật Bản', 'Quartz', 'Thép không gỉ', 'dong_ho_quartz_6.jpg', '65'),
    ('Skagen Aaren Kulor Blue', '5', 'Skagen', '2200000', NULL, 'Đồng hồ Quartz thời trang, màu xanh dương nổi bật, phong cách trẻ trung.', 'Nam/Nữ', 'Đan Mạch', 'Quartz', 'Hợp kim/Silicone', 'dong_ho_quartz_7.jpg', '80'),
    ('Casio Vintage A700WEMG', '5', 'Casio', '1600000', NULL, 'Đồng hồ Quartz điện tử mạ vàng, thiết kế mỏng, siêu nhẹ.', 'Nam/Nữ', 'Nhật Bản', 'Digital Quartz', 'Thép không gỉ', 'dong_ho_quartz_8.jpg', '100'),
    ('Rado Coupole Classic Quartz', '5', 'Rado', '28000000', NULL, 'Đồng hồ Quartz Thụy Sĩ cao cấp, mặt kính Sapphire lồi.', 'Nam', 'Thụy Sĩ', 'Quartz', 'Gốm/Thép', 'dong_ho_quartz_9.jpg', '12'),
    ('Adriatica Chronograph A8303', '5', 'Adriatica', '5800000', NULL, 'Đồng hồ Quartz Chronograph Thụy Sĩ, chức năng bấm giờ chính xác.', 'Nam', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ/Da', 'dong_ho_quartz_10.jpg', '38'),
    ('G-Shock Mudmaster Carbon Core', '6', 'G-Shock', '9200000', NULL, 'Thiết kế chống bùn đất và va đập, cấu trúc Carbon Core Guard.', 'Nam', 'Nhật Bản', 'Quartz', 'Nhựa/Carbon', 'dong_ho_the_thao_1.jpg', '55'),
    ('Casio G-Shock GA-2100 Black', '6', 'G-Shock', '2900000', NULL, 'G-Shock ""Casioak"" vỏ carbon mỏng, chống sốc, chống nước 200m.', 'Nam', 'Nhật Bản', 'Ana-Digi Quartz', 'Carbon/Resin', 'dong_ho_the_thao_2.jpg', '95'),
    ('Seiko Prospex SNE583 Solar Dive', '6', 'Seiko', '8500000', NULL, 'Đồng hồ lặn năng lượng mặt trời (Solar), chống nước 200m.', 'Nam', 'Nhật Bản', 'Solar Quartz', 'Thép không gỉ', 'dong_ho_the_thao_3.jpg', '30'),
    ('Garmin Instinct Solar Tactical', '6', 'Garmin', '9900000', NULL, 'Đồng hồ GPS thể thao, sạc năng lượng mặt trời, chống nhiệt/sốc chuẩn quân đội.', 'Nam', 'Mỹ', 'Digital', 'Polymer', 'dong_ho_the_thao_4.jpg', '40'),
    ('Citizen Promaster Diver BN0151', '6', 'Citizen', '7500000', NULL, 'Đồng hồ lặn chuyên nghiệp, Eco-Drive, đạt chuẩn ISO.', 'Nam', 'Nhật Bản', 'Eco-Drive Quartz', 'Thép không gỉ/Cao su', 'dong_ho_the_thao_5.jpg', '50'),
    ('Suunto 9 Baro Titanium', '6', 'Suunto', '15000000', NULL, 'Đồng hồ thể thao đa năng, GPS, đo độ cao, pin lên đến 120 giờ.', 'Nam/Nữ', 'Phần Lan', 'Digital', 'Titanium/Silicone', 'dong_ho_the_thao_6.jpg', '20'),
    ('Timex Ironman T5K822', '6', 'Timex', '1900000', NULL, 'Đồng hồ thể thao 100 lap, chống nước 100m, thiết kế nhẹ.', 'Nam', 'Mỹ', 'Digital Quartz', 'Nhựa', 'dong_ho_the_thao_7.jpg', '75'),
    ('Luminox Navy Seal 3501', '6', 'Luminox', '11000000', NULL, 'Đồng hồ quân đội, sử dụng công nghệ ánh sáng tự động LLT.', 'Nam', 'Thụy Sĩ', 'Quartz', 'Carbonox', 'dong_ho_the_thao_8.jpg', '25'),
    ('Casio Pro Trek PRG-600YB', '6', 'Casio', '6500000', NULL, 'Đồng hồ ngoài trời, 3 cảm biến (Áp suất, Độ cao, La bàn), sạc năng lượng mặt trời.', 'Nam', 'Nhật Bản', 'Ana-Digi Quartz', 'Thép không gỉ/Vải', 'dong_ho_the_thao_9.jpg', '38'),
    ('Tag Heuer Aquaracer Professional 200', '6', 'Tag Heuer', '55000000', NULL, 'Đồng hồ lặn Thụy Sĩ cao cấp, chống nước 200m, kính Sapphire.', 'Nam', 'Thụy Sĩ', 'Quartz', 'Thép không gỉ', 'dong_ho_the_thao_10.jpg', '15'),
    ('Garmin Forerunner 55', '7', 'Garmin', '5900000', NULL, 'Đồng hồ thông minh theo dõi GPS, đo nhịp tim, giấc ngủ.', 'Nam/Nữ', 'Mỹ', 'Digital', 'Polymer/Silicone', 'dong_ho_thong_minh_1.jpg', '80'),
    ('Apple Watch SE (2nd Gen)', '7', 'Apple', '7500000', NULL, 'Đồng hồ thông minh phổ biến, theo dõi hoạt động, nhịp tim.', 'Nam/Nữ', 'Mỹ', 'Digital', 'Nhôm/Silicone', 'dong_ho_thong_minh_2.jpg', '150'),
    ('Samsung Galaxy Watch 5', '7', 'Samsung', '8000000', NULL, 'Đồng hồ thông minh Android, đo thành phần cơ thể, GPS tích hợp.', 'Nam/Nữ', 'Hàn Quốc', 'Digital', 'Armor Aluminum/Fluoroelastomer', 'dong_ho_thong_minh_3.jpg', '120'),
    ('Xiaomi Watch S1 Active', '7', 'Xiaomi', '3500000', NULL, 'Đồng hồ thông minh giá phải chăng, màn hình AMOLED, hơn 117 chế độ thể thao.', 'Nam/Nữ', 'Trung Quốc', 'Digital', 'Hợp kim/TPU', 'dong_ho_thong_minh_4.jpg', '100'),
    ('Garmin Venu 2 Plus', '7', 'Garmin', '12000000', NULL, 'Theo dõi sức khỏe chuyên sâu, nghe gọi trực tiếp, thời lượng pin dài.', 'Nam/Nữ', 'Mỹ', 'Digital', 'Thép không gỉ/Silicone', 'dong_ho_thong_minh_5.jpg', '60'),
    ('Fitbit Sense 2', '7', 'Fitbit', '6500000', NULL, 'Đồng hồ thông minh tập trung vào quản lý stress, theo dõi giấc ngủ.', 'Nam/Nữ', 'Mỹ', 'Digital', 'Nhôm/Silicone', 'dong_ho_thong_minh_6.jpg', '75'),
    ('Huawei Watch GT 3 Pro', '7', 'Huawei', '7800000', NULL, 'Thiết kế sang trọng, pin 14 ngày, theo dõi nhịp tim và SpO2.', 'Nam/Nữ', 'Trung Quốc', 'Digital', 'Titanium/Gốm', 'dong_ho_thong_minh_7.jpg', '85'),
    ('Amazfit GTR 4', '7', 'Amazfit', '4500000', NULL, 'Đồng hồ thông minh giá tốt, GPS độ chính xác cao, pin 14 ngày.', 'Nam', 'Trung Quốc', 'Digital', 'Nhôm/Nylon', 'dong_ho_thong_minh_8.jpg', '90'),
    ('Oppo Watch Free', '7', 'Oppo', '2500000', NULL, 'Smartwatch giá rẻ, theo dõi giấc ngủ chuyên sâu, màn hình AMOLED.', 'Nam/Nữ', 'Trung Quốc', 'Digital', 'Nhựa/Silicone', 'dong_ho_thong_minh_9.jpg', '110'),
    ('Fossil Gen 6 Smartwatch', '7', 'Fossil', '8900000', NULL, 'Sử dụng hệ điều hành Wear OS, sạc siêu nhanh, kiểu dáng đồng hồ truyền thống.', 'Nam/Nữ', 'Mỹ', 'Digital', 'Thép không gỉ', 'dong_ho_thong_minh_10.jpg', '55'),
    ('Skagen Anita Mesh', '8', 'Skagen', '3150000', NULL, 'Thiết kế phong cách Đan Mạch tối giản, mặt đồng hồ mỏng, dây lưới kim loại.', 'Nữ', 'Đan Mạch', 'Quartz', 'Thép không gỉ/Mesh', 'dong_ho_thoi_trang_1.jpg', '70'),
    ('DW Iconic Link Lumine', '8', 'DW', '5200000', NULL, 'Đồng hồ thời trang nữ, viền đính đá, dây thép không gỉ mắc xích.', 'Nữ', 'Thụy Điển', 'Quartz', 'Thép không gỉ', 'dong_ho_thoi_trang_2.jpg', '60'),
    ('Tommy Hilfiger TH1781977', '8', 'Tommy Hilfiger', '3800000', NULL, 'Đồng hồ thời trang Mỹ, mặt số lớn, phong cách màu cờ đặc trưng.', 'Nam', 'Mỹ', 'Quartz', 'Thép không gỉ/Silicone', 'dong_ho_thoi_trang_3.jpg', '50'),
    ('Gucci G-Timeless YA1264028', '8', 'Gucci', '25000000', NULL, 'Đồng hồ thời trang cao cấp, họa tiết ong đặc trưng của thương hiệu.', 'Nữ', 'Ý', 'Quartz', 'Thép không gỉ/Vải', 'dong_ho_thoi_trang_4.jpg', '15'),
    ('Coach Pave C-Logo 14503792', '8', 'Coach', '4500000', NULL, 'Đồng hồ nữ thời trang, mặt chữ C đính đá lấp lánh.', 'Nữ', 'Mỹ', 'Quartz', 'Thép không gỉ', 'dong_ho_thoi_trang_5.jpg', '45'),
    ('Versus Versace Covent Garden', '8', 'Versus Versace', '3100000', NULL, 'Thiết kế ấn tượng, logo sư tử 3D, dây da mỏng.', 'Nữ', 'Ý', 'Quartz', 'Thép không gỉ/Da', 'dong_ho_thoi_trang_6.jpg', '65'),
    ('Diesel Mega Chief DZ4309', '8', 'Diesel', '6200000', NULL, 'Đồng hồ thời trang nam, mặt số lớn 51mm, hầm hố, cá tính.', 'Nam', 'Ý', 'Quartz', 'Thép không gỉ', 'dong_ho_thoi_trang_7.jpg', '35'),
    ('Olivia Burton OB16FG14', '8', 'Olivia Burton', '3900000', NULL, 'Đồng hồ nữ phong cách Anh, mặt số họa tiết hoa lá, thiên nhiên.', 'Nữ', 'Anh', 'Quartz', 'Hợp kim/Da', 'dong_ho_thoi_trang_8.jpg', '55'),
    ('Armani Exchange AX2104', '8', 'AX', '4800000', NULL, 'Đồng hồ nam thời trang, thiết kế đơn giản, phù hợp công sở.', 'Nam', 'Ý', 'Quartz', 'Thép không gỉ', 'dong_ho_thoi_trang_9.jpg', '40'),
    ('Cluse La Bohème CW0101201017', '8', 'Cluse', '2500000', NULL, 'Đồng hồ nữ Hà Lan, mặt số siêu mỏng, dây da có thể thay đổi.', 'Nữ', 'Hà Lan', 'Quartz', 'Thép không gỉ/Da', 'dong_ho_thoi_trang_10.jpg', '70'),
    ('Longines Master Collection Moonphase', '9', 'Longines', '48000000', NULL, 'Đồng hồ Thụy Sĩ cao cấp, chức năng lịch tuần trăng (Moonphase).', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ/Sapphire', 'dong_ho_cao_cap_1.jpg', '15'),
    ('Omega Seamaster Diver 300M', '9', 'Omega', '99999999', NULL, 'Đồng hồ lặn Thụy Sĩ cao cấp, máy Master Chronometer.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ', 'dong_ho_cao_cap_2.jpg', '8'),
    ('Tag Heuer Carrera Calibre 5', '9', 'Tag Heuer', '75000000', NULL, 'Đồng hồ cơ Thụy Sĩ thanh lịch, kính Sapphire.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ', 'dong_ho_cao_cap_3.jpg', '12'),
    ('Longines HydroConquest Automatic', '9', 'Longines', '35000000', NULL, 'Đồng hồ lặn cao cấp, chống nước 300m, được chứng nhận COSC.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ', 'dong_ho_cao_cap_4.jpg', '18'),
    ('Cartier Tank Must SolarBeat', '9', 'Cartier', '85000000', NULL, 'Đồng hồ cao cấp, công nghệ SolarBeat (năng lượng mặt trời).', 'Nữ', 'Pháp/Thụy Sĩ', 'Solar Quartz', 'Thép không gỉ/Da', 'dong_ho_cao_cap_5.jpg', '9'),
    ('Breitling Navitimer B01 Chronograph', '9', 'Breitling', '99999999', NULL, 'Đồng hồ phi công cao cấp, chức năng Chronograph bấm giờ, máy B01 in-house.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ', 'dong_ho_cao_cap_6.jpg', '5'),
    ('Hublot Classic Fusion Titanium', '9', 'Hublot', '99999999', NULL, 'Đồng hồ cao cấp, thiết kế độc đáo với vỏ Titanium và vành Bezel Gốm.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Titanium/Gốm', 'dong_ho_cao_cap_7.jpg', '3'),
    ('Zenith El Primero Chronomaster', '9', 'Zenith', '99999999', NULL, 'Máy bấm giờ El Primero huyền thoại, tần số dao động cao.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ/Da', 'dong_ho_cao_cap_8.jpg', '7'),
    ('IWC Portugieser Chronograph', '9', 'IWC', '99999999', NULL, 'Đồng hồ cao cấp, thiết kế Chronograph cổ điển.', 'Nam', 'Thụy Sĩ', 'Automatic', 'Thép không gỉ/Da', 'dong_ho_cao_cap_9.jpg', '6'),
    ('Rolex Datejust 36mm', '9', 'Rolex', '99999999', NULL, 'Biểu tượng của đồng hồ Thụy Sĩ, vỏ thép Oystersteel, niềng khía vàng trắng.', 'Nam/Nữ', 'Thụy Sĩ', 'Automatic', 'Oystersteel/Vàng trắng', 'dong_ho_cao_cap_10.jpg', '2'),
    ('Kiddy GPS Smart Watch', '10', 'Kiddy', '1200000', NULL, 'Đồng hồ định vị GPS cho trẻ em, chức năng nghe gọi hai chiều.', 'Trẻ Em', 'Trung Quốc', 'Digital', 'Nhựa/Silicone', 'dong_ho_tre_em_1.jpg', '90'),
    ('Kidizoom Smartwatch DX2', '10', 'VTech', '1500000', NULL, 'Đồng hồ thông minh cho trẻ em, có camera, trò chơi giáo dục.', 'Trẻ Em', 'Hồng Kông', 'Digital', 'Nhựa/Cao su', 'dong_ho_tre_em_2.jpg', '80'),
    ('Disney Frozen Digital Watch', '10', 'Disney', '500000', NULL, 'Đồng hồ điện tử in hình nhân vật Elsa và Anna.', 'Nữ', 'Trung Quốc', 'Digital Quartz', 'Nhựa/Silicone', 'dong_ho_tre_em_3.jpg', '120'),
    ('Lego Movie Time Teacher', '10', 'Lego', '800000', NULL, 'Đồng hồ dạy giờ cho trẻ, mặt số rõ ràng, hình nhân vật Lego.', 'Trẻ Em', 'Đan Mạch', 'Quartz', 'Nhựa/Cao su', 'dong_ho_tre_em_4.jpg', '110'),
    ('Xiaomi Mi Bunny Kids Phone Watch', '10', 'Xiaomi', '2000000', NULL, 'Đồng hồ điện thoại định vị GPS, camera, gọi video cho trẻ.', 'Trẻ Em', 'Trung Quốc', 'Digital', 'Nhựa/Silicone', 'dong_ho_tre_em_5.jpg', '70'),
    ('Flik Flak FPNP017', '10', 'Flik Flak', '1200000', NULL, 'Thương hiệu Thụy Sĩ chuyên cho trẻ em, chống nước, có thể giặt.', 'Nam/Nữ', 'Thụy Sĩ', 'Quartz', 'Nhựa/Vải', 'dong_ho_tre_em_6.jpg', '90'),
    ('Ben 10 Kids Digital Watch', '10', 'Ben 10', '450000', NULL, 'Đồng hồ điện tử in hình Ben 10, dây nhựa bền.', 'Nam', 'Trung Quốc', 'Digital Quartz', 'Nhựa', 'dong_ho_tre_em_7.jpg', '130'),
    ('Hello Kitty Analog Watch', '10', 'Hello Kitty', '600000', NULL, 'Đồng hồ kim cho bé gái, mặt số in hình Hello Kitty.', 'Nữ', 'Nhật Bản', 'Quartz', 'Hợp kim/Da', 'dong_ho_tre_em_8.jpg', '100'),
    ('Smartwatch Kids 4G SOS', '10', 'No Brand', '1800000', NULL, 'Đồng hồ thông minh có chức năng SOS khẩn cấp, mạng 4G.', 'Trẻ Em', 'Trung Quốc', 'Digital', 'Nhựa/Silicone', 'dong_ho_tre_em_9.jpg', '65'),
    ('Timex Time Machines T7B151', '10', 'Timex', '950000', NULL, 'Đồng hồ kim dạy giờ, tính năng đèn nền Indiglo, chống nước 30m.', 'Trẻ Em', 'Mỹ', 'Quartz', 'Hợp kim/Vải', 'dong_ho_tre_em_10.jpg', '75');