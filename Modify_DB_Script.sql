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

/*Modify Product-Review relation*/
ALTER TABLE mydb.reviewhub
DROP FOREIGN KEY fk_ReviewHub_Products1;
ALTER TABLE mydb.reviewhub
DROP FOREIGN KEY reviewhub_ibfk_1;
ALTER TABLE mydb.review_detail
DROP FOREIGN KEY fk_ReviewDetail_ReviewHub1;

DROP TABLE mydb.reviewhub;

ALTER TABLE mydb.review_detail
RENAME TO mydb.reviewproduct;
ALTER TABLE mydb.reviewproduct
DROP COLUMN ReviewHubID;

ALTER TABLE mydb.reviewproduct
ADD COLUMN UserID int not null;
ALTER TABLE mydb.reviewproduct
ADD FOREIGN KEY fk_reviewproduct_UserID (UserID) REFERENCES mydb.user(ID)
ON UPDATE CASCADE ON DELETE NO ACTION;
ALTER TABLE mydb.reviewproduct
ADD COLUMN ProductID int not null;
ALTER TABLE mydb.reviewproduct
ADD FOREIGN KEY fk_reviewproduct_ProductID (ProductID) REFERENCES mydb.products(ID)
ON UPDATE CASCADE ON DELETE NO ACTION;

ALTER TABLE mydb.products
ADD COLUMN Unit varchar(50) AFTER Stock;

/*Add tables to online payment process*/
DROP TABLE IF EXISTS mydb.merchant;
CREATE TABLE mydb.merchant
(
	ID varchar(50) PRIMARY KEY,
    MerchantName varchar(255),
    MerchantWebLink varchar(255),
    MerchantIpnUrl varchar(255),
    MerchantReturnUrl varchar(255),
    SecretKey varchar(255),
    IsActive bool
);

DROP TABLE IF EXISTS mydb.payment_destination;
CREATE TABLE mydb.payment_destination
(
	ID varchar(50) PRIMARY KEY,
    DesName varchar(255),
    DesShortName varchar(50),
    DesParentID varchar(50),
    DesLogo varchar(50),
    SortIndex int,
    IsActive bool
);

DROP TABLE IF EXISTS mydb.online_payment;
CREATE TABLE mydb.online_payment
(
	ID varchar(50) PRIMARY KEY,
    MerchantID varchar(50) NOT NULL,
    PaymentContent varchar(500),
    PaymentConcurency varchar(10),
    PaymentRefID varchar(50),
    RequiredAmount int,
    PaymentLanguage varchar(10),
    PaymentDate datetime,
    ExpireDate datetime,
    PaymentStatus varchar(255),
    PaymentLastMessage varchar(255),
    PaymentDestinationID varchar(50),
    PaidAmount int,
    FOREIGN KEY fk_merchant_OnlinePayment (MerchantID) 
    REFERENCES mydb.merchant (ID) ON DELETE NO ACTION ON UPDATE CASCADE,
    FOREIGN KEY fk_merchant_PayDes (PaymentDestinationID)
    REFERENCES mydb.payment_destination (ID) ON UPDATE CASCADE ON DELETE NO ACTION
);

DROP TABLE IF EXISTS mydb.payment_transaction;
CREATE TABLE mydb.payment_transaction
(
	ID varchar(50) PRIMARY KEY,
    TranAmount int,
    TranMessage varchar(500),
    TranPayload varchar(255),
    TranStatus varchar(45),
    TranDate datetime,
    TranRefID varchar(50),
    OnlinePaymentID varchar(50) NOT NULL,
    FOREIGN KEY fk_transaction_Payment (OnlinePaymentID)
    REFERENCES mydb.online_payment (ID) ON UPDATE CASCADE ON DELETE NO ACTION
);

DROP TABLE IF EXISTS mydb.payment_notification;
CREATE TABLE mydb.payment_notification
(
	ID varchar(50) PRIMARY KEY,
    PaymentRefId varchar(50),
	NotiDate datetime,
	NotiContent varchar(255),
	NotiAmount int,
	NotiMessage varchar(550),
	NotiSignature varchar(255),
	NotiPaymentId varchar(50),
	MerchantId varchar(50),
	NotiStatus varchar(50),
	NotiResDate datetime,
	NotiResMessage varchar(255),
	NotiResHttpCode varchar(255),
    FOREIGN KEY fk_PayNoti_merchant (MerchantID)
    REFERENCES mydb.merchant (ID) ON UPDATE CASCADE ON DELETE NO ACTION
);

DROP TABLE IF EXISTS mydb.payemt_signature;
CREATE TABLE mydb.payment_signature
(
	ID varchar(50) PRIMARY KEY,
    PaymentId varchar(50) NOT NULL,
	SignValue varchar(255),
	SignAlgo varchar(50),
	SignOwn varchar(50),
	SignDate datetime,
	IsValid bool,
    FOREIGN KEY fk_signature_Payment (PaymentID) 
    REFERENCES mydb.online_payment (ID) ON UPDATE CASCADE ON DELETE NO ACTION
);

