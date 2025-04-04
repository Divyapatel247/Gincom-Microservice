CREATE INDEX idx_basket_userid ON Basket(UserId);
CREATE INDEX idx_order_userid ON `Order`(UserId);
CREATE INDEX idx_payment_orderid ON Payment(OrderId);