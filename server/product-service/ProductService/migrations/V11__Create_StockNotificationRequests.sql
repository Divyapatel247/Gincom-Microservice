create table StockNotificationRequests(
    Id int AUTO_INCREMENT Primary key,
    userId int not null,
    productId int not null,
    RequestedAt datetime default CURRENT_TIMESTAMP,
    IsNotified Boolean default False,
    unique key (userId,productId),
    FOREIGN KEY (productId) references products(Id)
);