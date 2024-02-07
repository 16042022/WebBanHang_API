-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `mydb` DEFAULT CHARACTER SET utf8 ;
USE `mydb` ;

-- -----------------------------------------------------
-- Table `mydb`.`Roles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Roles` (
  `RoleID` TINYINT NOT NULL AUTO_INCREMENT,
  `RoleName` VARCHAR(45) NOT NULL,
  `CreateAt` DATETIME NOT NULL,
  `UpdateAt` DATETIME NULL,
  PRIMARY KEY (`RoleID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`User`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`User` (
  `UserID` INT NOT NULL AUTO_INCREMENT,
  `RoleID` TINYINT NOT NULL,
  `Username` VARCHAR(45) NOT NULL,
  `Avatar` VARCHAR(45) NULL,
  `Status` TINYINT NOT NULL,
  `Password` VARCHAR(255) NOT NULL,
  `Email` VARCHAR(100) NOT NULL,
  `ResetPwdToken` VARCHAR(255) NULL,
  `ResetPwdExpires` DATETIME NULL,
  `RefreshToken` VARCHAR(4096) NULL,
  `CreateAt` DATETIME NOT NULL,
  `UpdateAt` DATETIME NULL,
  PRIMARY KEY (`UserID`),
  CONSTRAINT `fk_User_Roles1`
    FOREIGN KEY (`RoleID`)
    REFERENCES `mydb`.`Roles` (`RoleID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
ENGINE = InnoDB;

CREATE INDEX `fk_User_Roles1_idx` ON `mydb`.`User` (`RoleID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `mydb`.`Customers`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Customers` (
  `CustomerID` INT NOT NULL AUTO_INCREMENT,
  `UserID` INT NOT NULL,
  `firstName` VARCHAR(45) NOT NULL,
  `lastName` VARCHAR(255) NOT NULL,
  `Email` VARCHAR(100) NOT NULL,
  `PhoneNo` VARCHAR(45) NOT NULL,
  `CreateAt` DATETIME NOT NULL,
  `UpdateAt` DATETIME NULL,
  PRIMARY KEY (`CustomerID`),
  CONSTRAINT `fk_Customers_User`
    FOREIGN KEY (`UserID`)
    REFERENCES `mydb`.`User` (`UserID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
ENGINE = InnoDB;

CREATE INDEX `fk_Customers_User_idx` ON `mydb`.`Customers` (`UserID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `mydb`.`Order_status`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Order_status` (
  `StatusID` TINYINT NOT NULL AUTO_INCREMENT,
  `StatusName` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`StatusID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Payments`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Payments` (
  `PaymentID` TINYINT NOT NULL AUTO_INCREMENT,
  `Methods` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`PaymentID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Orders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Orders` (
  `OrderID` INT NOT NULL AUTO_INCREMENT,
  `StatusID` TINYINT NOT NULL,
  `PaymentID` TINYINT NOT NULL,
  `CustomerID` INT NOT NULL,
  `TotalPrice` INT NULL DEFAULT 0,
  `CreateAt` DATETIME NOT NULL,
  `UpdateAt` DATETIME NULL,
  PRIMARY KEY (`OrderID`),
  CONSTRAINT `fk_Orders_Order_status1`
    FOREIGN KEY (`StatusID`)
    REFERENCES `mydb`.`Order_status` (`StatusID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Orders_Payments1`
    FOREIGN KEY (`PaymentID`)
    REFERENCES `mydb`.`Payments` (`PaymentID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Orders_Customers1`
    FOREIGN KEY (`CustomerID`)
    REFERENCES `mydb`.`Customers` (`CustomerID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
ENGINE = InnoDB;

CREATE INDEX `fk_Orders_Order_status1_idx` ON `mydb`.`Orders` (`StatusID` ASC) VISIBLE;

CREATE INDEX `fk_Orders_Payments1_idx` ON `mydb`.`Orders` (`PaymentID` ASC) VISIBLE;

CREATE INDEX `fk_Orders_Customers1_idx` ON `mydb`.`Orders` (`CustomerID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `mydb`.`ProductType`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`ProductType` (
  `ProductTypeID` SMALLINT NOT NULL AUTO_INCREMENT,
  `TypeName` VARCHAR(45) NOT NULL,
  `CreateAt` DATETIME NOT NULL,
  `UpdateAt` DATETIME NULL,
  PRIMARY KEY (`ProductTypeID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`ProductStatus`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`ProductStatus` (
  `ProductStatusID` TINYINT NOT NULL AUTO_INCREMENT,
  `StatusName` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`ProductStatusID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Products`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Products` (
  `ProductID` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(45) NOT NULL,
  `Price` INT NOT NULL,
  `Discount` DECIMAL(5,2) NULL DEFAULT 0,
  `NumberOfViews` INT NULL,
  `Title` VARCHAR(255) NULL,
  `Stock` SMALLINT NULL,
  `CreateAt` DATETIME NOT NULL,
  `UpdateAt` DATETIME NULL,
  `ProductTypeID` SMALLINT NOT NULL,
  `ProductStatusID` TINYINT NOT NULL,
  PRIMARY KEY (`ProductID`),
  CONSTRAINT `fk_Products_ProductType1`
    FOREIGN KEY (`ProductTypeID`)
    REFERENCES `mydb`.`ProductType` (`ProductTypeID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Products_ProductStatus1`
    FOREIGN KEY (`ProductStatusID`)
    REFERENCES `mydb`.`ProductStatus` (`ProductStatusID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
ENGINE = InnoDB;

CREATE INDEX `fk_Products_ProductType1_idx` ON `mydb`.`Products` (`ProductTypeID` ASC) VISIBLE;

CREATE INDEX `fk_Products_ProductStatus1_idx` ON `mydb`.`Products` (`ProductStatusID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `mydb`.`Order_detail`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Order_detail` (
  `OrderDetailID` INT NOT NULL AUTO_INCREMENT,
  `OrderID` INT NOT NULL,
  `ProductID` INT NOT NULL,
  `Quantites` SMALLINT NOT NULL,
  `ProductPrice` INT NOT NULL,
  PRIMARY KEY (`OrderDetailID`),
  CONSTRAINT `fk_Order_detail_Orders1`
    FOREIGN KEY (`OrderID`)
    REFERENCES `mydb`.`Orders` (`OrderID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Order_detail_Products1`
    FOREIGN KEY (`ProductID`)
    REFERENCES `mydb`.`Products` (`ProductID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
ENGINE = InnoDB;

CREATE INDEX `fk_Order_detail_Orders1_idx` ON `mydb`.`Order_detail` (`OrderID` ASC) VISIBLE;

CREATE INDEX `fk_Order_detail_Products1_idx` ON `mydb`.`Order_detail` (`ProductID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `mydb`.`ProductPicture`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`ProductPicture` (
  `ProductPictureID` INT NOT NULL AUTO_INCREMENT,
  `ProductID` INT NOT NULL,
  `Title` VARCHAR(45) NOT NULL,
  `Image` VARCHAR(255) NULL,
  `ProductAvatar` VARCHAR(255) NULL,
  `Status` VARCHAR(45) NULL,
  `CreateAt` DATETIME NOT NULL,
  `UpdateAt` DATETIME NULL,
  PRIMARY KEY (`ProductPictureID`),
  CONSTRAINT `fk_ProductPicture_Products1`
    FOREIGN KEY (`ProductID`)
    REFERENCES `mydb`.`Products` (`ProductID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE INDEX `fk_ProductPicture_Products1_idx` ON `mydb`.`ProductPicture` (`ProductID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `mydb`.`ReviewHub`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`ReviewHub` (
  `ReviewHubID` INT NOT NULL AUTO_INCREMENT,
  `ProductID` INT NOT NULL,
  `Customers_CustomerID` INT NOT NULL,
  PRIMARY KEY (`ReviewHubID`),
  CONSTRAINT `fk_ReviewHub_Products1`
    FOREIGN KEY (`ProductID`)
    REFERENCES `mydb`.`Products` (`ProductID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `fk_ReviewHub_Customers1`
    FOREIGN KEY (`Customers_CustomerID`)
    REFERENCES `mydb`.`Customers` (`CustomerID`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
ENGINE = InnoDB;

CREATE INDEX `fk_ReviewHub_Products1_idx` ON `mydb`.`ReviewHub` (`ProductID` ASC) VISIBLE;

CREATE INDEX `fk_ReviewHub_Customers1_idx` ON `mydb`.`ReviewHub` (`Customers_CustomerID` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `mydb`.`ReviewDetail`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`ReviewDetail` (
  `ReviewDetailID` INT NOT NULL AUTO_INCREMENT,
  `ReviewHubID` INT NOT NULL,
  `ContentRated` VARCHAR(255) NULL,
  `Evaluation` TINYINT NOT NULL,
  `ContentSeen` VARCHAR(255) NULL,
  `Status` VARCHAR(45) NULL,
  `CreateAt` DATETIME NULL,
  `UpdateAt` DATETIME NULL,
  PRIMARY KEY (`ReviewDetailID`),
  CONSTRAINT `fk_ReviewDetail_ReviewHub1`
    FOREIGN KEY (`ReviewHubID`)
    REFERENCES `mydb`.`ReviewHub` (`ReviewHubID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE INDEX `fk_ReviewDetail_ReviewHub1_idx` ON `mydb`.`ReviewDetail` (`ReviewHubID` ASC) VISIBLE;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
