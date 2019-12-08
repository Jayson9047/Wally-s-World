/*FILE : Database.cs
 * Project:JBWally
 * Programmer: Jayson Ovishek Biswas
 * Description: This file contains the Database class. This Database class basically does all the communications between the application 
 *              and the JBWally database
 * First Version: 06-12-2019
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace JBWally
{
    /*
     * Class Name: Database
     * Description: This class handles all the scenarios that involves the database. This class can update, delete and retrieve information
     *              from the JBWally database.
     */
    class Database
    {
        public static String firstName { get; set; }
        public static String lastName { get; set; }
        public static String phoneNo { get; set; }
        public List<string> customerOrder = new List<string>();
        /// <summary>
        /// checks if the customer exists or not
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>Returns true if customer exists, false otherwise</returns>
        public static bool CheckCustomer(String firstName, String lastName)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" USE JBWally;
                                                   SELECT FirstName,LastName FROM Customers;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    MySqlDataReader reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        //check if the customer exists (case insensitive)
                        if ((reader[0].ToString().ToLower().Trim() == firstName) && (reader[1].ToString().ToLower().Trim() == lastName))
                        {
                            return true;
                        }
                    }
                }
                myConn.Close();
            }
            return false;
        }
        /// <summary>
        /// Enrolls a new customer with first name last name and phone number 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="PhoneNo"></param>
        public static void EnrollCustomer(String firstName, String lastName, String PhoneNo)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" USE JBWally;
                                                  Insert Into Customers (FirstName,LastName,PhoneNumber)
                                                  values (@fName, @lName, @pNumber);";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myCommand.Parameters.AddWithValue("@fName", firstName);
                    myCommand.Parameters.AddWithValue("@lName", lastName);
                    myCommand.Parameters.AddWithValue("@pNumber", PhoneNo);
                    myConn.Open();

                    myCommand.ExecuteNonQuery();
                }
                myConn.Close();
            }
            return;
        }

        /// <summary>
        /// This method fills up any combo box with the name of the branches retrieved from Branch table
        /// </summary>
        /// <returns>A list of string containing the name of branches</returns>
        public static List<string> FillBranchCombo()
        {
            List<string> branch = new List<string>();
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @"Select BranchName from Branch;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    MySqlDataReader reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string branchName = reader.GetString(0);
                        branch.Add(branchName);
                    }
                }
                myConn.Close();
            }
            return branch;
        }

        /// <summary>
        /// This method fills up any combo box with the name of the products retrieved from Product table
        /// </summary>
        /// <returns>A list of string containing the name of products</returns>
        public static List<string> FillProductCombo()
        {
            List<string> products = new List<string>();
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @"Select ProductName from Product;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    MySqlDataReader reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string productName = reader.GetString(0);
                        products.Add(productName);
                    }
                }
                myConn.Close();
            }
            return products;
        }

        /// <summary>
        /// This method retrives the branch ID for a particular branch using the branch name from the Branch table
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>The branch ID is returned as Int32</returns>
        public static Int32 GetBranchID(string branchName)
        {
            string bID;
            Int32 branchID = 0;
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" SELECT BranchID FROM Branch Where BranchName = @bName;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    myCommand.Parameters.AddWithValue("@bName", branchName);
                    bID = myCommand.ExecuteScalar().ToString();
                    branchID = Convert.ToInt32(bID);
                }
                myConn.Close();
            }
            return branchID;
        }
        /// <summary>
        /// This method retrives the product ID for a particular product using the product name from the Product table
        /// </summary>
        /// <param name="productName"></param>
        /// <returns>The product ID is returned as Int32</returns>
        public static Int32 GetProductID(string productName)
        {
            Int32 bID = 0;
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" SELECT SKU FROM Product Where ProductName = @pName;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    myCommand.Parameters.AddWithValue("@pName", productName);
                    bID = (Int32)myCommand.ExecuteScalar();
                }
                myConn.Close();
            }
            return bID;
        }

        /// <summary>
        /// This method retrieves the available quantity of a product in a particular branch from BranchProduct table 
        /// using the branchID and productID 
        /// </summary>
        /// <param name="branchID"></param>
        /// <param name="productID"></param>
        /// <returns>Returns the available Quantity</returns>
        public static int GetQuantity(int branchID, int productID)
        {
            Int32 BranchID = 0;
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" SELECT Quantity FROM BranchProduct Where ProductID = @pID and BranchID = @bID;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    myCommand.Parameters.AddWithValue("@pID", productID);
                    myCommand.Parameters.AddWithValue("@bID", branchID);
                    BranchID = (Int32)myCommand.ExecuteScalar();
                }
                myConn.Close();
            }
            return BranchID;
        }

        /// <summary>
        /// This method retrieves the wholesale price of a product from the Product table
        /// </summary>
        /// <param name="productID"></param>
        /// <returns>The wholesale price is returned as float</returns>
        public static float RetrieveWPrice(int productID)
        {
            float wprice = 0;
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" SELECT wprice from Product where SKU = @prodID;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    myCommand.Parameters.AddWithValue("@prodID", productID);
                    wprice = (float)myCommand.ExecuteScalar();
                }
                myConn.Close();
            }
            return wprice;
        }


        /// <summary>
        /// This method works as a part of an order process. It changes the 'Orders' table. This method inesrts an entry in the 'Orders' for 
        /// a requested order. A new entry of CustomerID,OrderDate,OrderStatus,BranchID and BranchName is inserted into the table. OrderID is 
        /// set automatically as it is set to auto_increment 
        /// </summary>
        /// <param name="orderStatus"></param>
        /// <param name="branchName"></param>
        public static void ChangeOrderTable(int orderStatus, string branchName)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" INSERT INTO Orders(CustomerID,OrderDate,OrderStatus,BranchID,BranchName)
                                               VALUES((select CustomerID from Customers where FirstName=@fName and LastName=@lName),
			                                   curdate(),@status,(select BranchID from Branch where BranchName = @bName),@bName);";


                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    myCommand.Parameters.AddWithValue("@fName", firstName);
                    myCommand.Parameters.AddWithValue("@lName", lastName);         
                    myCommand.Parameters.AddWithValue("@status", orderStatus);
                    myCommand.Parameters.AddWithValue("@bName", branchName);
                    myCommand.ExecuteNonQuery();
                }
                myConn.Close();
            }
        }
        /// <summary>
        /// This method changes the OrderLine table while processing an order. A new entry for OrderID, ProductID,ProductName,sPrice,Quantity 
        /// and TotalPrice is inserted into the table.
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="quantity"></param>
        /// <param name="totalPrice"></param>
        public static void ChangeOrderLineTable(string productName, int quantity, float totalPrice)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @"INSERT INTO OrderLine(OrderID,ProductID,ProductName,sPrice,Quantity,TotalPrice)
                                            VALUES((select OrderID from Orders ORDER BY OrderID DESC LIMIT 1),(Select SKU from Product where ProductName = @pName),
                                            @pName,(Select wPrice from Product where ProductName = @pName)*1.4,@qty,@tPrice);";
                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    myCommand.Parameters.AddWithValue("@pName", productName);
                    myCommand.Parameters.AddWithValue("@qty", quantity);
                    myCommand.Parameters.AddWithValue("@tPrice", totalPrice);

                    myCommand.ExecuteNonQuery();
                }
                myConn.Close();
            }
        }
        /// <summary>
        /// This method changes the BranchProduct table while processing an order. This method is called when the quantity of a product in a
        /// particular branch changes. This method updates the quantity.
        /// </summary>
        /// <param name="allowedQty"></param>
        /// <param name="quantity"></param>
        /// <param name="branchID"></param>
        /// <param name="productID"></param>
        public static void UpdateBranchQuantity(int allowedQty, int quantity, int branchID, int productID)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" Update BranchProduct 
                                               Set Quantity=(@qty-@quan) where BranchID = @bID and ProductID=@pID; ";
                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    myCommand.Parameters.AddWithValue("@qty", allowedQty);
                    myCommand.Parameters.AddWithValue("@quan", quantity);
                    myCommand.Parameters.AddWithValue("@bID", branchID);
                    myCommand.Parameters.AddWithValue("@pID", productID);

                    myCommand.ExecuteNonQuery();
                }
                myConn.Close();
            }
        }

        /// <summary>
        /// Updates the total quantity in the products table while ordering
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="quantity"></param>
        public static void UpdateBranchProductQuantity(int productID, int quantity)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" Update Product
                                               Set Stock=Stock-@qty where SKU=@pID;";
                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();


                    myCommand.Parameters.AddWithValue("@qty", quantity);

                    myCommand.Parameters.AddWithValue("@pID", productID);

                    myCommand.ExecuteNonQuery();
                }
                myConn.Close();
            }
        }



        /// <summary>
        /// ProductName,Quantity,TotalPrice,Branch.BranchName,Concat(Customers.FirstName,' ',Customers.LastName)as CustomerName,Orders.OrderDate, Orders.OrderID,Orders.OrderStatus
        /// is used to create a view that is used to retrieve information for refunding 
        /// </summary>
        public static void CreateRefundView()
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @"  create or replace view custOrderInfo
                                                   as 
                                                   Select ProductName,Quantity,TotalPrice,Branch.BranchName,Concat(Customers.FirstName,' ',Customers.LastName)as CustomerName,Orders.OrderDate, Orders.OrderID,Orders.OrderStatus
                                                    from Orderline
                                                   Inner join Orders on Orders.OrderID = OrderLine.OrderID
                                                   Inner join Branch on Orders.BranchID = Branch.BranchID
                                                    Inner join Customers on Orders.CustomerID = Customers.CustomerID
                                                    Order by OrderID; ";
                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    myCommand.ExecuteNonQuery();
                }
                myConn.Close();
            }
        }

        /// <summary>
        /// Retrieves all the order details of a customer.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>List of string containing order details</returns>
        public static List<string>[] CustomerOrderHistory(string name)
        {

            CreateRefundView();

            List<string> customerHistory = new List<string>();
            List<string> custHist = new List<string>();
            List<string>[] s = new List<string>[2];
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @"SELECT ProductName,BranchName,Quantity,TotalPrice,OrderID,OrderDate,OrderStatus from custOrderInfo where CustomerName=@custName;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();
                    myCommand.Parameters.AddWithValue("@custName", name);
                    MySqlDataReader reader = myCommand.ExecuteReader();
                    string status = "";
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader[6].ToString().Trim()) == 0)
                        {
                            status = "Refunded";
                        }
                        else if (Convert.ToInt32(reader[6].ToString().Trim()) == 1)
                        {
                            status = "Paid";
                        }
                        else
                        {
                            status = "Refund Requested";
                        }
                        string orderHistory = reader[0].ToString() + "\n" + "Purchased From " +
                                          reader[1].ToString() + "\nQuantity: " + reader[2].ToString() +
                                         "\t\t\t\t\tPrice: " + reader[3].ToString() + "\nOrderID: " + reader[4].ToString() +
                                         "\nOrder Date: " + reader[5].ToString() + "\t\tOrderStatus: " + status.ToString();
                        customerHistory.Add(orderHistory);
                        orderHistory = "ProductName: " + reader[0].ToString() + " BranchName: " + reader[1].ToString() + "Quantity: " + reader[2].ToString() + " OrderID: " + reader[4].ToString() + "\n" + "OrderStatus: " + reader[6].ToString() + "Done" + " Order Date: " + reader[5].ToString() + "date";
                        custHist.Add(orderHistory);
                    }
                    s[0] = customerHistory;
                    s[1] = custHist;

                }
                myConn.Close();
            }
            return s;
        }

        /// <summary>
        /// Updates OrderLine, Orders, BranchProduct and Product table to refund
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="quantity"></param>
        /// <param name="branchID"></param>
        /// <param name="productID"></param>
        public static void Refund(int orderID, int quantity, int branchID, int productID)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @"  Update OrderLine Set Quantity=0 where OrderID=@oID;
                                                Update OrderLine Set TotalPrice=0 where OrderID=@oID;
                                                Update Orders Set OrderStatus=0 where OrderID=@oID;
                                                Update BranchProduct Set Quantity=Quantity+@qty where BranchID=@bID and ProductID=@pID;
	                                            Update Product Set Stock=Stock+@qty where SKU=@pID; ";
                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();
                    myCommand.Parameters.AddWithValue("@oID", orderID);
                    myCommand.Parameters.AddWithValue("@qty", quantity);
                    myCommand.Parameters.AddWithValue("@bID", branchID);
                    myCommand.Parameters.AddWithValue("@pID", productID);

                    myCommand.ExecuteNonQuery();
                }
                myConn.Close();
            }
        }


        /// <summary>
        /// Gets the Customer ID
        /// </summary>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <returns></returns>
        public static Int32 GetCustomerID(string fName, string lName)
        {
            string cID = "";
            int custID = 0;
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" SELECT CustomerID FROM Customers Where FirstName = @fName and LastName=@lName;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();

                    myCommand.Parameters.AddWithValue("@fName", fName);
                    myCommand.Parameters.AddWithValue("@lName", lName);
                    cID = myCommand.ExecuteScalar().ToString();
                    custID = Convert.ToInt32(cID);
                }
                myConn.Close();
            }
            return custID;
        }

        /// <summary>
        /// Shows all the information about an order according to the order status
        /// </summary>
        /// <param name="OrderStatus"></param>
        /// <returns>Returns 2 lists of String in an array containing the information</returns>
        public static List<string>[] ShowData(int OrderStatus)
        {
            CreateRefundView();

            List<string> customerHistory = new List<string>();
            List<string> custHist = new List<string>();
            List<string>[] s = new List<string>[2];
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @"SELECT ProductName,BranchName,Quantity,TotalPrice,OrderID,OrderDate,OrderStatus from custOrderInfo where OrderStatus=@oStatus;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();
                    myCommand.Parameters.AddWithValue("@oStatus", OrderStatus);
                    MySqlDataReader reader = myCommand.ExecuteReader();
                    string status = "";
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader[6].ToString().Trim()) == 0)
                        {
                            status = "Refunded";
                        }
                        else if (Convert.ToInt32(reader[6].ToString().Trim()) == 1)
                        {
                            status = "Paid";
                        }
                        else
                        {
                            status = "Refund Requested";
                        }
                        string orderHistory = reader[0].ToString() + "\n" + "Purchased From " +
                                          reader[1].ToString() + "\nQuantity: " + reader[2].ToString() +
                                         "\t\t\t\t\tPrice: " + reader[3].ToString() + "\nOrderID: " + reader[4].ToString() +
                                         "\nOrder Date: " + reader[5].ToString() + "\t\tOrderStatus: " + status.ToString();
                        customerHistory.Add(orderHistory);
                        orderHistory = "ProductName: " + reader[0].ToString() + " BranchName: " + reader[1].ToString() + "Quantity: " + reader[2].ToString() + " OrderID: " + reader[4].ToString() + "\n";
                        custHist.Add(orderHistory);
                    }
                    s[0] = customerHistory;
                    s[1] = custHist;

                }
                myConn.Close();
            }
            return s;
        }

        /// <summary>
        /// Retrieves the inventory for a particular branch 
        /// </summary>
        /// <param name="branchID"></param>
        /// <returns></returns>
        public static List<string> DisplayInventory(int branchID)
        {

            List<string> BranchInventory = new List<string>();

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @"select ProductID, Quantity,Product.ProductName from BranchProduct 
                                              inner join Product on BranchProduct.ProductID = Product.SKU
                                              where BranchID = @bID;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();
                    myCommand.Parameters.AddWithValue("@bID", branchID);
                    MySqlDataReader reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {

                        string branchInv = "Product ID: " + reader[0].ToString() + "\t\tProduct Name: " +
                                          reader[2].ToString() + "\t\tStock: " + reader[1].ToString();
                        BranchInventory.Add(branchInv);

                    }

                }
                myConn.Close();
            }
            return BranchInventory;
        }

        /// <summary>
        /// Retrieves all information about a single product using the product ID.
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static string DisplayProduct(int productID)
        {
            string Desc = "";


            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" Select ProductName, Stock, ProductDescription, ProductColour, ProductSize, ProductPattern, ProductType, wprice from Product
                                               where SKU=@pID;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();
                    myCommand.Parameters.AddWithValue("@pID", productID);
                    MySqlDataReader reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {

                        Desc = "Product Name: " + reader[0].ToString() + "\nProduct ID: " + productID.ToString() + "\nProduct Description: " +
                                         reader[2].ToString() + "\nColor: " + reader[3].ToString() + "\nSize: " + reader[4].ToString() + "\nPattern: " + reader[5].ToString() +
                                         "\nType: " + reader[6] + "\nTotal Stock: " + reader[1].ToString() + "\nUnit Price: " + reader[7];

                    }

                }
                myConn.Close();
            }
            return Desc;
        }


        /// <summary>
        /// Gets the last order ID
        /// </summary>
        /// <returns></returns>
        public static Int32 GetOrderID()
        {
            string cID = "";
            int orderID = 0;
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (var myConn = new MySqlConnection(connString))
            {

                const string sqlStatement = @" select OrderID from Orders ORDER BY OrderID DESC LIMIT 1;";

                using (var myCommand = new MySqlCommand(sqlStatement, myConn))
                {
                    myConn.Open();
                    cID = myCommand.ExecuteScalar().ToString();
                    orderID = Convert.ToInt32(cID);
                }
                myConn.Close();
            }
            return orderID;
        }
    }
}
