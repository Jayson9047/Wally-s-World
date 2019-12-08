	/*
		File: JBWally.sql
        Programmer: Jayson Ovishek Biswas
        Description: This sql query file creates a new database called JBWally and creates and fills up the following tables:
        Customers, Branch, Product, Orders, OrderLine, BranchProduct.
    */
    DROP DATABASE IF EXISTS JBWally;
	CREATE DATABASE JBWally;
    
   USE JBWally;
   
   -- Creates Customers Table
	Create Table if not exists Customers
    (
     CustomerID Int NOT NULL PRIMARY KEY auto_increment,
	 FirstName Varchar(25) not null,
     LastName Varchar(25) not null,
	 PhoneNumber VARCHAR(25) not null
	);
    
    -- Create Branch table where BranchID is the primary key
	Create Table if not exists Branch
    (
		BranchID INT NOT NULL PRIMARY KEY auto_Increment,
        BranchName varchar(25) not null
    );
    
    -- Create Product table where SKU is the primary key
	Create Table if not exists Product
    (
		SKU Int NOT NULL PRIMARY KEY auto_increment,
        ProductName Varchar(80) not null,
        Stock Int,
        ProductDescription Varchar(1024),
        ProductColour varchar(10),
        ProductSize int,
        ProductPattern varchar(30), 
        ProductType varchar(30),
        wprice float Not NULL 
    );
   
   -- Create Orders Table where OrderID is the primary key
	Create Table if not exists Orders
    (
		OrderID int not null Primary key auto_increment,
        CustomerID int not null,
		OrderDate date not null,
        OrderStatus int not null,
        BranchID int not null,
        BranchName varchar(25) not null,
        CONSTRAINT OrderStatus_Ck CHECK (OrderStatus BETWEEN -1 AND 1), -- -1 means refund requested 0 means refund, 1 means paid
        Foreign key (CustomerID) references Customers(CustomerID),
        Foreign key (BranchID) references Branch(BranchID)
    );
    
    -- create orderLine table. This table shows all the details about any particular order
	Create Table if not exists OrderLine
    (
		OrderID int not null,
		OrderLineID int not null Primary key auto_increment,
		ProductID int not null,
        ProductName varchar(40),
        sPrice float not null,
        Quantity int not null,
        TotalPrice float not null,
        Foreign key(OrderID) references Orders(OrderID),
        Foreign key(ProductID) references Product (SKU)
    );

-- Insert data into the Customers table
insert into Customers(FirstName, LastName, PhoneNumber)
values("Carlo","Sgro","4562389563"),
	  ("Norbert", "Mika", "5612348967"),
      ("Russell", "foubert", "8623596741"),
      ("Sean", "Clarke","5195553333");
      

insert into Branch(BranchName)
values("Sport World"),
	  ("Waterloo"),
      ("Cambridge Mall");

insert into Product(ProductName, Stock, wPrice,ProductDescription,ProductColour,ProductSize,ProductPattern,ProductType)
values("Disco Queen Wallpaper (roll)", 56, 7.50, 'Pictures Disco Queen','MultiColor',30, 'Square','Hardware'),
	  ("Countryside Wallpaper (roll)", 24 , 8.65,'A Roll wallpaper', 'Multicolor', 30, 'Square', 'Vinyl'),
      ("Victorian Lace Wallpaper (roll)", 44 , 14.95,'Professionally Designed Wallpaper','Dark Red', 20, 'Square','Hardware'),
      ("Drywall Tape (roll)", 120, 3.95, 'A roll Tape', 'Blue', '5','Circle','Hardware'),
      ("Drywall Repair Compound (tube)", 90,6.95,'A tube','Unknown','6','Tube','Hardware');
      
insert into Orders(CustomerID,OrderDate,OrderStatus, BranchName, BranchID)
values(2,'2019-11-14',1, 'Waterloo',(Select BranchID from Branch where BranchName='Waterloo')),
	  (3,'2019-10-05',0,'Sport World',(Select BranchID from Branch where BranchName='Sport World')),
      (1,'2019-06-23',1,'Cambridge Mall',(Select BranchID from Branch where BranchName='Cambridge Mall'));

insert into OrderLine(OrderID, ProductID,ProductName, sPrice, Quantity,TotalPrice)
values('1',(Select SKU from Product where ProductName='Disco Queen Wallpaper (roll)'),'Disco Queen Wallpaper (roll)', 12.65, 5,71.47),
	  ('2',(Select SKU from Product where ProductName='Victorian Lace Wallpaper (roll)'),'Victorian Lace Wallpaper (roll)', 14.65, 8,132.44),
      ('3',(Select SKU from Product where ProductName='Countryside Wallpaper (roll)'),'Countryside Wallpaper (roll)', 9.50, 13,139.5);




insert into Product(ProductName,Stock,wprice,ProductDescription,ProductColour,ProductSize,ProductPattern,ProductType)
    Values ('Dark Forest Wallpaper', '163',13.50,'Pictures a Dark Forest','Dark', 20, 'Square','Hardware' );
    
-- create BranchProduct table
Create table if not exists BranchProduct
(
	branchProductID int not null primary key auto_increment,
    BranchID int not null,
    BranchName varchar(40) not null,
    ProductID int not null,
    Quantity int,
    Foreign key (BranchID) references Branch (BranchID),
    Foreign key (ProductID) references Product(SKU)
);

insert into BranchProduct (BranchID,BranchName,ProductID,Quantity)
values(1,(Select BranchName from Branch where BranchID=1),1,27),
	  (2,(Select BranchName from Branch where BranchID=2),1,20),
      (3,(Select BranchName from Branch where BranchID=3),1,9),
      (1,(Select BranchName from Branch where BranchID=1),2,15),
      (2,(Select BranchName from Branch where BranchID=2),2,5),
      (3,(Select BranchName from Branch where BranchID=3),2,4),
      (1,(Select BranchName from Branch where BranchID=1),3,20),
      (2,(Select BranchName from Branch where BranchID=2),3,15),
      (3,(Select BranchName from Branch where BranchID=3),3,9),
      (1,(Select BranchName from Branch where BranchID=1),4,50),
      (2,(Select BranchName from Branch where BranchID=2),4,45),
      (3,(Select BranchName from Branch where BranchID=3),4,25),
	  (1,(Select BranchName from Branch where BranchID=1),5,70),
      (2,(Select BranchName from Branch where BranchID=2),5,53),
      (3,(Select BranchName from Branch where BranchID=3),5,40);

    
    -- Create a view tha shows ProductName,Quantity, TotalPrice, BranchName,CCustomerName, rderDate, OrderID and OrderStatus for each orderLine entry
    create or replace view custOrderInfo
    as 
    Select ProductName,Quantity,TotalPrice,Branch.BranchName,Concat(Customers.FirstName," ",Customers.LastName)as CustomerName,Orders.OrderDate, Orders.OrderID,Orders.OrderStatus
    from Orderline 
    Inner join Orders on Orders.OrderID = OrderLine.OrderID
    Inner join Branch on Orders.BranchID = Branch.BranchID
    Inner join Customers on Orders.CustomerID = Customers.CustomerID
    Order by OrderID;
	

    