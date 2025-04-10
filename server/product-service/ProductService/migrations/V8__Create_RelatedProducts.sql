CREATE TABLE RelatedProducts (
    ProductId INT NOT NULL,
    RelatedProductId INT NOT NULL,
    PRIMARY KEY (ProductId, RelatedProductId),
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    FOREIGN KEY (RelatedProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

