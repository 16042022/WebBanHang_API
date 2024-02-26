-- Create function and triggers:--
/*
- 1. Auto fill product's price (contain discount) when a row of order-detail table is added
- 2. Auto caculate sum of order for each row of order detail is added
- 3. Auto refresh value of sum price if any row of order detail is update or delete 
*/

DELIMITER $$
USE mydb $$
/*Call this function each time added infor to order_details table*/
create function getPriceValue (productID INT)
RETURNS INTEGER
READS SQL DATA
begin
	
    declare resValue INT DEFAULT 0;

    select pr.Price - (pr.Price * (pr.Discount * 0.01))
    into resValue
    from mydb.products pr
    where pr.ProductID = productID;
    
return resValue;
end$$
DELIMITER ;

DELIMITER $$
drop trigger if exists after_added_order $$

CREATE TRIGGER after_added_order
	AFTER INSERT ON mydb.order_detail
    FOR EACH ROW
	BEGIN
    
    DECLARE tlPrice INT DEFAULT 0;
    SET tlPrice = getPriceValue(NEW.ProductID);
    UPDATE mydb.orders ord
    SET ord.TotalPrice = ord.TotalPrice + (tlPrice * NEW.Quantites), ord.UpdateAt = now()
    WHERE NEW.OrderID = ord.OrderID;
    
	END$$
DELIMITER ;

DELIMITER $$
DROP TRIGGER IF EXISTS after_update_prdprice $$

CREATE TRIGGER after_update_prdprice
	AFTER UPDATE ON mydb.products
    FOR EACH ROW
    BEGIN
    
    -- Lay ra OrderID cua record co SP bi thay doi gia thanh --
    SET @order_row = 0, @finalPrice = 0;
    SELECT ordl.OrderID
    INTO @order_row
    FROM mydb.order_detail ordl
    WHERE ordl.ProductID = OLD.ProductID;
    
	-- Cap nhat lai gia thanh phan--
    UPDATE mydb.order_detail ordt
    SET ordt.ProductPrice = getPriceValue(OLD.ProductID)
    WHERE ordt.ProductID = OLD.ProductID;
    
    -- Tinh toan tong tien cuoi cung can tra
    SELECT SUM(ordl.ProductPrice * ordl.Quantites) INTO @finalPrice
    FROM mydb.order_detail ordl
    WHERE ordl.OrderID = @order_row
    GROUP BY ordl.OrderID;
    
	-- Gia cuoi cung sau thay doi gia thanh SP la gia cu cong/tru voi phan chenh lech phat sinh--
    UPDATE mydb.orders ord
    SET ord.TotalPrice = @finalPrice, ord.UpdateAt = now()
    WHERE ord.OrderID = @order_row;
    
    END$$
DELIMITER ;

DELIMITER $$
DROP TRIGGER IF EXISTS after_delete_orderItems $$

CREATE TRIGGER  after_delete_orderItems
	AFTER DELETE ON mydb.order_detail
    FOR EACH ROW
    BEGIN
		DECLARE oldPrice INT DEFAULT 0;
		SET oldPrice = getPriceValue(OLD.ProductID) * OLD.Quantites;
        
        UPDATE mydb.orders ord
        SET ord.TotalPrice = ord.TotalPrice - oldPrice, ord.UpdateAt = now()
        WHERE ord.OrderID = OLD.OrderID;
    END $$
DELIMITER ;

-- Trigger when update Quantities in Order --
DELIMITER $$
DROP TRIGGER IF EXISTS after_update_quantities $$

CREATE TRIGGER after_update_quantities
	AFTER UPDATE ON mydb.order_detail
    FOR EACH ROW
    BEGIN
		-- Get the old value and subtract it from the totalPrice --
		DECLARE oldPrice INT DEFAULT 0;
		DECLARE newPrice INT DEFAULT 0;
		SET oldPrice = OLD.ProductPrice * OLD.Quantites, newPrice = OLD.ProductPrice * NEW.Quantites;
		
		UPDATE mydb.orders ord
		SET ord.TotalPrice = ord.TotalPrice - oldPrice
		WHERE ord.OrderID = OLD.OrderID;
		-- Sum-up the new value --
		UPDATE mydb.orders ord
		SET ord.TotalPrice = ord.TotalPrice + newPrice, ord.UpdateAt = now()
		WHERE ord.OrderID = OLD.OrderID;
    END$$
DELIMITER ;


