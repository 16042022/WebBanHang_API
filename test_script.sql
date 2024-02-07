use mydb;

-- Dinh nghia loai sp --
INSERT INTO product_type (TypeName, CreateAt)
VALUE ('Đồ khô', now()), ('Đồ nước', now()), ('Tinh chế', now()), ('Sơ chế', now()), ('Đông lạnh', now());

-- Định nghĩa các thẻ trạng thái của sp --
INSERT INTO product_status (StatusName)
VALUE ('Available'), ('Out of stock');

-- Them 1 vai sp demo tinh nang trigger --
INSERT INTO roles (RoleName, CreateAt)
VALUE ('Admin', now()), ('Employee', now()), ('Customer', now());

call populate_fk('mydb', 'user', 5, 'N');
call populate_fk('mydb', 'customers', 5, 'N');
truncate mydb. user;
truncate mydb.customers;

INSERT INTO order_status (StatusName)
VALUE ('In-process');

INSERT INTO payments (Methods)
VALUE ('Card'), ('COD'), ('Cash-on-store');

INSERT INTO orders (StatusID, PaymentID, CustomerID, CreateAt)
VALUE (4, 3, 4, now()), (2, 1, 5, now()), (2, 2, 3, now());

INSERT INTO products (ProductTypeID, ProductStatusID, Name, Price, CreateAt)
VALUE (5, 1, 'Mực ống 1 nắng đông lạnh', 135000, now()),
(2, 1, 'Sữa tươi Vinamilk có đường', 39000, now()),
(1, 1, 'Mì tương đen Hàn đóng gói', 15000, now());

INSERT INTO order_detail (OrderID, ProductID, Quantites, ProductPrice)
VALUE(1, 2, 5, getPriceValue(2)), (1, 1, 15, getPriceValue(1));
truncate mydb.order_detail;

SET FOREIGN_KEY_CHECKS = 0;
truncate mydb.products;


SET FOREIGN_KEY_CHECKS = 1;

