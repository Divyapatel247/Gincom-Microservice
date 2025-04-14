-- Add createdAt to Basket table
ALTER TABLE Basket
ADD COLUMN CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP;

-- Update existing records with current timestamp
UPDATE Basket
SET CreatedAt = CURRENT_TIMESTAMP
WHERE CreatedAt IS NULL;

-- Add createdAt to BasketItems table
ALTER TABLE BasketItems
ADD COLUMN CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP;

-- Update existing records with current timestamp
UPDATE BasketItems
SET CreatedAt = CURRENT_TIMESTAMP
WHERE CreatedAt IS NULL;

-- Add createdAt to Order table
ALTER TABLE `Order`
ADD COLUMN CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP;

-- Update existing records with current timestamp
UPDATE `Order`
SET CreatedAt = CURRENT_TIMESTAMP
WHERE CreatedAt IS NULL;

-- Add createdAt to OrderItems table
ALTER TABLE OrderItems
ADD COLUMN CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP;

-- Update existing records with current timestamp
UPDATE OrderItems
SET CreatedAt = CURRENT_TIMESTAMP
WHERE CreatedAt IS NULL;

-- Add createdAt to Payment table
ALTER TABLE Payment
ADD COLUMN CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP;

-- Update existing records with current timestamp
UPDATE Payment
SET CreatedAt = CURRENT_TIMESTAMP
WHERE CreatedAt IS NULL;