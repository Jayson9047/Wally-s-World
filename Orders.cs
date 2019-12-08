/*FILE : Orders.cs
 * Project:JBWally
 * Programmer: Jayson Ovishek Biswas
 * Description: This file contains the Orders class. This class deals with all the scenarios that is related to 
 *              creating, updating or refunding an order.
 * First Version: 06-12-2019
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBWally
{
    class Orders
    {
        public struct OrderInfo
        {
            public string BranchName { get; set; }
            public string ProductName { get; set; }
            public float ProductPrice { get; set; }
            public int Quantity { get; set; } //ordered quantity
            public int TotalQuantity { get; set; }
        }

        public static bool orderPage = true; //this variable is used to indicate which page to go when "Home" button is clicked in SalesRecord page

        public static List<OrderInfo> ProductList = new List<OrderInfo>();
        public static List<string> OrderDetails = new List<string>();
        public static bool previousOrder;
        public static string OrderDesc;

        /// <summary>
        /// Calculates the price of a product according to the product ID and quantity
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static float[] CalculatePrice(int productID, int quantity)
        {
            float wPrice = Database.RetrieveWPrice(productID);
            float sPrice = wPrice * (float)1.4;
            float totalSPrice = quantity * wPrice * (float)1.4;
            float tax = totalSPrice * ((float)13 / (float)100);
            tax = (float)Math.Round(tax, 2);
            float total = totalSPrice + tax;
            float[] priceSummary = { totalSPrice, tax, total, sPrice };
            return priceSummary;
        }

        /// <summary>
        /// Adds item to the cart
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="branchName"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="totalQuantity"></param>
        public static void AddtoCart(string productName, string branchName, float price, int quantity, int totalQuantity)
        {

            var cd = new OrderInfo();
            cd.BranchName = branchName;
            cd.ProductName = productName;
            cd.ProductPrice = price;
            cd.Quantity = quantity;
            cd.TotalQuantity = totalQuantity;
            ProductList.Add(cd);

        }

        /// <summary>
        /// Creates the order and returns true if successful, returns false if the quantity is wrong 
        /// </summary>
        /// <param name="branchName"></param>
        /// <param name="productName"></param>
        /// <param name="totalPrice"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static bool CreateOrder(string branchName, string productName, float totalPrice, int quantity)
        {
            int pID = Database.GetProductID(productName);
            int bID = Database.GetBranchID(branchName);
            int qty = Database.GetQuantity(bID, pID);
            if (qty < quantity)
            {
                return false;
            }
            Database.ChangeOrderTable(1, branchName);
            Database.ChangeOrderLineTable(productName, quantity, totalPrice);
            Database.UpdateBranchQuantity(qty, quantity, bID, pID);
            Database.UpdateBranchProductQuantity(pID, quantity);
            return true;
        }
        /// <summary>
        /// Refunds and Order.
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="quantity"></param>
        /// <param name="branchName"></param>
        /// <param name="productName"></param>
        public static void RefundOrder(int orderID, int quantity, string branchName, string productName)
        {
            int bID = Database.GetBranchID(branchName);
            int pID = Database.GetProductID(productName);
            Database.Refund(orderID, quantity, bID, pID);
        }


    }
}
