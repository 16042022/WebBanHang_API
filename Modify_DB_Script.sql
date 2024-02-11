use mydb;

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

ALTER TABLE mydb.user
ADD COLUMN ResetPwdToken varchar(500) AFTER Email;
ALTER TABLE mydb.user
ADD COLUMN ResetPwdExpires datetime AFTER ResetPwdToken;

DROP TABLE IF EXISTS mydb.refresh_token;
CREATE TABLE mydb.refresh_token
(
	ID int PRIMARY KEY AUTO_INCREMENT,
    
);

