use mydb;

-- Detele this table --
DROP TABLE IF EXISTS mydb.stored_token;
CREATE TABLE mydb.stored_token
(
	ID int PRIMARY KEY AUTO_INCREMENT,
    UserID int NOT NULL,
    RefreshToken varchar(4096),
    ResetPwdToken varchar(500),
    ResetPwdExpires datetime,
    JwtID varchar(255),
    IssuedAt datetime,
    ExpireTime datetime,
    IsRevoked bool,
    IsUsed bool,
    FOREIGN KEY fk_storedToken(UserID) REFERENCES mydb.user(ID)
		ON DELETE NO ACTION ON UPDATE CASCADE
);
-- END --
ALTER TABLE mydb.user
ADD COLUMN ResetPwdToken varchar(500) AFTER Email;
ALTER TABLE mydb.user
ADD COLUMN ResetPwdExpires datetime AFTER ResetPwdToken;

DROP TABLE IF EXISTS mydb.refresh_token;
CREATE TABLE mydb.refresh_token
(
	ID int PRIMARY KEY AUTO_INCREMENT,
    UserID int not null,
    Token varchar(255),
    Created datetime,
    Expires datetime,
    CreateByIp varchar(255),
    Revoked datetime,
    RevokedById varchar(255),
    ReplacedByToken varchar(255),
    ReasonRevoked varchar(550),
    IsExpired bool,
    IsRevoked bool,
    FOREIGN KEY fk_storedToken(UserID) REFERENCES mydb.user(ID)
		ON DELETE NO ACTION ON UPDATE CASCADE
);

ALTER TABLE mydb.user
ADD COLUMN (VerifyToken varchar(500), VerifyDate datetime, IsVerify bool);
/*
Delete Customer entity, combine Customer -> User entity
*/

/*Delete link of Customer entity to other table*/
ALTER TABLE mydb.orders
DROP FOREIGN KEY fk_Orders_Customers1;
ALTER TABLE mydb.orders
DROP COLUMN CustomerID;

ALTER TABLE mydb.reviewhub
DROP FOREIGN KEY fk_ReviewHub_Customers1;
ALTER TABLE mydb.reviewhub
DROP COLUMN Customers_CustomerID;

-- Delete Customer entity
DROP TABLE mydb.customers;
-- Add Customer field into User entity
ALTER TABLE mydb.user
ADD COLUMN (firstName varchar(50), lastName varchar(50), phoneNo varchar(50));
-- Add back forgein key 
ALTER TABLE mydb.orders
ADD COLUMN UserID int not null after PaymentID;
ALTER TABLE mydb.orders
ADD FOREIGN KEY fk_orders_UserID (UserID) REFERENCES mydb.user(ID)
ON UPDATE CASCADE ON DELETE NO ACTION;
ALTER TABLE mydb.reviewhub
ADD COLUMN UserID int not null;
ALTER TABLE mydb.reviewhub
ADD FOREIGN KEY fk_reviewhub_UserID (UserID) REFERENCES mydb.user(ID)
ON UPDATE CASCADE ON DELETE NO ACTION;

ALTER TABLE mydb.user
DROP COLUMN IsVerifed;

ALTER TABLE mydb.refreshtoken
RENAME COLUMN CreateByIp TO CreatedByIp;
ALTER TABLE mydb.refreshtoken
DROP COLUMN IsExpired;
ALTER TABLE mydb.refreshtoken
DROP COLUMN IsRevoked;