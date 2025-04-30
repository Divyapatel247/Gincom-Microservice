-- Step 1: Drop the foreign key constraints
ALTER TABLE RelatedProducts
    DROP FOREIGN KEY relatedproducts_ibfk_1,
    DROP FOREIGN KEY relatedproducts_ibfk_2;

-- Step 2: Drop the existing composite primary key
ALTER TABLE RelatedProducts
    DROP PRIMARY KEY;

-- Step 3: Add the new auto-incrementing Id as the primary key
ALTER TABLE RelatedProducts
    ADD Id INT AUTO_INCREMENT PRIMARY KEY;

-- Step 4: Re-add the foreign key constraints
ALTER TABLE RelatedProducts
    ADD CONSTRAINT fk_relatedproducts_productid
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_relatedproducts_relatedproductid
        FOREIGN KEY (RelatedProductId) REFERENCES Products(Id) ON DELETE CASCADE;

-- Step 5: Add a unique constraint to maintain no duplicate relationships
ALTER TABLE RelatedProducts
    ADD UNIQUE KEY unique_product_related (ProductId, RelatedProductId);