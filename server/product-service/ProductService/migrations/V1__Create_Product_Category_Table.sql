CREATE TABLE Categories (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL
);

CREATE TABLE Products (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    Price DECIMAL(10,2) NOT NULL,
    DiscountPercentage DECIMAL(5,2),
    Rating DECIMAL(3,1),
    Stock INT NOT NULL,
    Tags VARCHAR(255),
    Brand VARCHAR(50),
    Sku VARCHAR(20),
    Weight DECIMAL(10,2),
    Dimensions VARCHAR(50),
    WarrantyInformation VARCHAR(255),
    ShippingInformation VARCHAR(255),
    AvailabilityStatus VARCHAR(20),
    ReturnPolicy VARCHAR(255),
    MinimumOrderQuantity INT,
    Thumbnail VARCHAR(255),
    CategoryId INT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);