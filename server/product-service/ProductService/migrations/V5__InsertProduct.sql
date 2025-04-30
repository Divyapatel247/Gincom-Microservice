INSERT INTO Products (
    Id, Title, Description, Price, DiscountPercentage, Rating, Stock, Tags, Brand, Sku, 
    Weight, Dimensions, WarrantyInformation, ShippingInformation, AvailabilityStatus, Review, 
    ReturnPolicy, MinimumOrderQuantity, Images, Thumbnail, CategoryId
) VALUES (
    1, -- Id
    'Essence Mascara Lash Princess', -- Title
    'The Essence Mascara Lash Princess is a popular mascara known for its volumizing and lengthening effects. Achieve dramatic lashes with this long-lasting and cruelty-free formula.', -- Description
    9.99, -- Price
    7.17, -- DiscountPercentage
    4.94, -- Rating
    5, -- Stock
    '["beauty", "mascara"]', -- Tags (JSON)
    'Essence', -- Brand
    'RCH45Q1A', -- Sku
    2.0, -- Weight
    '{"width": 23.17, "height": 14.43, "depth": 28.01}', -- Dimensions (JSON)
    '1 month warranty', -- WarrantyInformation
    'Ships in 1 month', -- ShippingInformation
    'Low Stock', -- AvailabilityStatus
    '[{"reviewerName": "John Doe", "comment": "Very unhappy with my purchase!"}, {"reviewerName": "Nolan Gonzalez", "comment": "Not as described!"}, {"reviewerName": "Scarlett Wright", "comment": "Very satisfied!"}]', -- Reviews (JSON, simplified)
    '30 days return policy', -- ReturnPolicy
    24, -- MinimumOrderQuantity
    '["https://cdn.dummyjson.com/products/images/beauty/Essence%20Mascara%20Lash%20Princess/1.png"]', -- Images (JSON)
    'https://cdn.dummyjson.com/products/images/beauty/Essence%20Mascara%20Lash%20Princess/thumbnail.png', -- Thumbnail
    1 -- CategoryId (maps to 'beauty' from Categories table)
);