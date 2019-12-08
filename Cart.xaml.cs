using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace JBWally
{
    /// <summary>
    /// Interaction logic for Cart.xaml
    /// </summary>
    public partial class Cart : Page
    {
        private string branchName { get; set; }
        private string productName { get; set; }
        private float price { get; set; }
        private float sTotal { get; set; }
        private int Quantity { get; set; }
        private int totalQuantity { get; set; }
        private int selectionIndex { get; set; }

        /// <summary>
        /// constructor for cart. Calculates subtotal from Order page orders and shows on pageload.
        /// </summary>
        public Cart()
        {
            InitializeComponent();
            GetInfo();
            LBox.SelectionChanged += LBox_SelectionChanged;
            CalculateSubTotal();
        }
        /// <summary>
        /// Gets all the information about the orders 
        /// </summary>
        public void GetInfo()
        {

            foreach (Orders.OrderInfo s in Orders.ProductList)
            {
                this.branchName = s.BranchName;
                this.productName = s.ProductName;
                this.price = s.ProductPrice;
                this.Quantity = s.Quantity;
                this.totalQuantity = s.TotalQuantity;

                LBox.Items.Add(productName + "\n" + "Purchased From " + branchName + "\nQuantity: " + this.Quantity.ToString() + "\t\t\t\t\tPrice: " + this.price.ToString());
            }
        }

        /// <summary>
        /// if listBox selection is changed disable or enable increase or decrease button according to the price of the order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = LBox.SelectedIndex;
            int i = 0;
            string bName = "";
            string pName = "";
            float pri;
            int QTY = 0;
            int totalQTY = 0;
            if (LBox.SelectedIndex != selectionIndex)
            {

                foreach (Orders.OrderInfo s in Orders.ProductList)
                {
                    bName = s.BranchName;
                    pName = s.ProductName;
                    pri = s.ProductPrice;
                    QTY = s.Quantity;
                    totalQTY = s.TotalQuantity;
                    if (i == index)
                        break;
                    else
                        i++;
                }
                if (QTY >= totalQTY)        //if the order quantity is greater than or equal to the stock disable the increase button. Else enable
                {
                    increase.IsEnabled = false;
                }
                else
                {
                    increase.IsEnabled = true;
                }


            }

        }
        /// <summary>
        /// increases the quantity of an order by one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = LBox.SelectedIndex;
            int i = 0;
            decrease.IsEnabled = true;
            foreach (Orders.OrderInfo s in Orders.ProductList)
            {
                this.branchName = s.BranchName;
                this.productName = s.ProductName;
                this.price = s.ProductPrice;
                this.Quantity = s.Quantity;
                this.totalQuantity = s.TotalQuantity;
                if (i == index)
                    break;
                else
                    i++;

            }
            if (index >= 0)
            {
                Orders.OrderInfo temp = Orders.ProductList[index];
                float newPrice = this.price;
                float[] allPrice;
                LBox.Items.RemoveAt(index);
                this.Quantity++;
                temp.BranchName = this.branchName;
                temp.ProductName = this.productName;

                temp.Quantity = this.Quantity;
                temp.TotalQuantity = this.totalQuantity;

                int pID = Database.GetProductID(this.productName);
                allPrice = Orders.CalculatePrice(pID, this.Quantity);

                this.price = allPrice[2];
                temp.ProductPrice = this.price;
                newPrice = this.price - newPrice;
                Orders.ProductList[index] = temp;
                LBox.Items.Insert(index, productName + "\n" + "Purchased From " + branchName + "\nQuantity: " + this.Quantity.ToString() + "\t\t\t\t\tPrice: " + this.price.ToString());
                if (this.Quantity == this.totalQuantity)
                {
                    increase.IsEnabled = false;
                }
                CalculateSubTotal(newPrice);
            }
            decrease.IsEnabled = true;
            LBox.SelectedIndex = index;
            selectionIndex = index;
        }

        /// <summary>
        /// decreases the quantity of an order by one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void decrease_Click(object sender, RoutedEventArgs e)
        {
            int index = LBox.SelectedIndex;
            int i = 0;
            bool check = true;
            increase.IsEnabled = true;
            foreach (Orders.OrderInfo s in Orders.ProductList)
            {
                this.branchName = s.BranchName;
                this.productName = s.ProductName;
                this.price = s.ProductPrice;
                this.Quantity = s.Quantity;
                if (i == index)
                    break;
                else
                    i++;

            }
            if (index >= 0)
            {
                Orders.OrderInfo temp = Orders.ProductList[index];
                float newPrice = this.price;
                float[] allPrice;
                this.Quantity--;
                if (this.Quantity == 0)
                {
                    if (MessageBox.Show("Do you want to remove the item from your cart?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        check = false;
                    }
                    else
                    {
                        this.Quantity = 1;
                    }
                }
                LBox.Items.RemoveAt(index);

                if (check == true)
                {
                    temp.BranchName = this.branchName;
                    temp.ProductName = this.productName;

                    temp.Quantity = this.Quantity;

                    int pID = Database.GetProductID(this.productName);
                    allPrice = Orders.CalculatePrice(pID, this.Quantity);

                    this.price = allPrice[2];
                    temp.ProductPrice = this.price;
                    newPrice = this.price - newPrice;
                    Orders.ProductList[index] = temp;
                    LBox.Items.Insert(index, productName + "\n" + "Purchased From " + branchName + "\nQuantity: " + this.Quantity.ToString() + "\t\t\t\t\tPrice: " + this.price.ToString());
                }
                else
                {
                    newPrice = this.price - newPrice;
                    Orders.ProductList.RemoveAt(index);
                }
                CalculateSubTotal(newPrice);
            }


            LBox.SelectedIndex = index;
            selectionIndex = index;
        }

        /// <summary>
        /// Calculates the total price including tax.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private float CalculateSubTotal(float p = 0)
        {
            sTotal = 0;
            String str = subTotal.Text;
            if (p == 0)
            {
                foreach (Orders.OrderInfo s in Orders.ProductList)
                {
                    this.branchName = s.BranchName;
                    this.productName = s.ProductName;
                    this.price = s.ProductPrice;
                    this.Quantity = s.Quantity;

                    sTotal = sTotal + this.price;
                }
            }
            else
            {
                float total = float.Parse(str);
                sTotal = total + p;
            }

            subTotal.Text = sTotal.ToString();
            return sTotal;
        }

        /// <summary>
        /// Orders the items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Orders.previousOrder = false;
            
            bool valid = false;
            int i = 1;
            int j = 0;
            int count = 0;
            int qty = 0;
            float tax = 0;
            float total = 0;
            float subTotal = 0;
            int orderID = 0;
            foreach (Orders.OrderInfo s in Orders.ProductList)
            {
                this.branchName = s.BranchName;
                this.productName = s.ProductName;
                this.price = s.ProductPrice;
                this.Quantity = s.Quantity;
                float[] allPrices = Orders.CalculatePrice(Database.GetProductID(this.productName), this.Quantity);


                qty = Database.GetQuantity(Database.GetBranchID(this.branchName), Database.GetProductID(this.productName));
                valid = Orders.CreateOrder(this.branchName, this.productName, this.price, this.Quantity);

                //check if the store has enough quantity of the product
                if (valid == false)
                {
                    //if not show error
                    MessageBox.Show("Sorry, Order No." + i.ToString() + " Couldn't be done.\n" +
                    this.productName + " has " + qty + " items in the inventory." + " You Ordered " + this.Quantity + MessageBoxImage.Information);

                }
                else
                {
                    //else do order
                    orderID = Database.GetOrderID();

                    string line = "Order ID:" + orderID.ToString() + "\n" + this.productName + " " + this.Quantity + " \tX\t$" + allPrices[3].ToString() + "\t=" + allPrices[0].ToString() + "\n";
                    subTotal = subTotal + allPrices[0];
                    tax = tax + allPrices[1];
                    total = total + allPrices[2];
                    Orders.OrderDetails.Add(line);
                }
                i++;
                count++;

            }
            string totalSale = "SubTotal:\t" + subTotal.ToString() + "\nHST(13%):\t  " + tax.ToString() + "\nSaleTotal:\t" + total.ToString();
            Orders.OrderDetails.Add(totalSale);

            //clear the ordered Item from the list
            LBox.Items.RemoveAt(j);


            Orders.ProductList.Clear();
            if ((valid == false) && (count == 1))
            {
                NavigationService.Navigate(new Uri("Order.xaml", UriKind.RelativeOrAbsolute));
                Orders.OrderDetails.Clear();
            }
            else
            {
                Orders.orderPage = true;
                NavigationService.Navigate(new Uri("SalesRecord.xaml", UriKind.RelativeOrAbsolute));
            }


        }
        /// <summary>
        /// Go to Order page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Order.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
