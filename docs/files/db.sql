CREATE TABLE [dbo].[branch] 
  ( 
     [branchid]       INT IDENTITY (1, 1) NOT NULL, 
     [branchlocation] [sys].[GEOGRAPHY] NULL, 
     [branchname]     NCHAR (10) NULL, 
     PRIMARY KEY CLUSTERED ([branchid] ASC) 
  ); 

CREATE TABLE [dbo].[product] 
  ( 
     [productid]        INT IDENTITY (1, 1) NOT NULL, 
     [productname]      NVARCHAR (50) NULL, 
     [productbarcode]   NCHAR (6) NOT NULL, 
     [productunitprice] MONEY NULL, 
     PRIMARY KEY CLUSTERED ([productid] ASC) 
  ); 

CREATE TABLE [dbo].[inventory] 
  ( 
     [productid]   INT NOT NULL, 
     [branchid]    INT NOT NULL, 
     [branchunits] INT DEFAULT ((0)) NOT NULL, 
     CONSTRAINT [AK_Inventory_Column] PRIMARY KEY CLUSTERED ([productid] ASC, 
     [branchid] ASC), 
     CONSTRAINT [FK_Inventory_Branch] FOREIGN KEY ([branchid]) REFERENCES 
     [branch]([branchid]) ON DELETE CASCADE, 
     CONSTRAINT [FK_Inventory_Product] FOREIGN KEY ([productid]) REFERENCES 
     [product]([productid]) ON DELETE CASCADE 
  ); 

CREATE TRIGGER [createEmptyInventory] 
ON [dbo].[product] 
after INSERT 
AS 
  BEGIN 
      DECLARE @branch_id INT 
      DECLARE @inserted_id INT 

      SELECT @inserted_id = productid 
      FROM   inserted 

      SET ROWCOUNT 0 

      SELECT * 
      INTO   #mytemp 
      FROM   [branch] 

      SET ROWCOUNT 1 

      SELECT @branch_id = branchid 
      FROM   #mytemp 

      WHILE @@rowcount <> 0 
        BEGIN 
            SET ROWCOUNT 0 

            INSERT INTO [inventory] 
                        (productid, 
                         branchid) 
            VALUES      (@inserted_id, 
                         @branch_id) 

            DELETE #mytemp 
            WHERE  branchid = @branch_id 

            SET ROWCOUNT 1 

            SELECT @branch_id = branchid 
            FROM   #mytemp 
        END 

      SET ROWCOUNT 0 
  END 

CREATE TRIGGER [addEmptyBranch] 
ON [dbo].[branch] 
after INSERT 
AS 
  BEGIN 
      DECLARE @product_id INT 
      DECLARE @inserted_id INT 

      SELECT @inserted_id = branchid 
      FROM   inserted 

      SET ROWCOUNT 0 

      SELECT * 
      INTO   #mytemp 
      FROM   [product] 

      SET ROWCOUNT 1 

      SELECT @product_id = productid 
      FROM   #mytemp 

      WHILE @@rowcount <> 0 
        BEGIN 
            SET ROWCOUNT 0 

            INSERT INTO [inventory] 
                        (productid, 
                         branchid) 
            VALUES      (@product_id, 
                         @inserted_id) 

            DELETE #mytemp 
            WHERE  productid = @product_id 

            SET ROWCOUNT 1 

            SELECT @product_id = productid 
            FROM   #mytemp 
        END 

      SET ROWCOUNT 0 
  END 