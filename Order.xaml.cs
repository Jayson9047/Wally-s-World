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
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class Order : Page
    {
        private int QTY = 0;
        private int BID = 0;    //branch id
        private int PID = 0;    // product id
        private float Price = 0;
        /// <summary>
        /// constructor for Order. Fills up the combo boxes for branches and products. 
        /// </summary>
        public Order()
        {
            InitializeComponent();
            FillBranchCombo();
            FillProductsCombo();
            plus.IsEnabled = false;
            minus.IsEnabled = false;

            AddToCart.IsEnabled = false;
            viewCart.IsEnabled = false;
            Greetings();

        }

        private void FillBranchCombo()
        {
            List<string> branch = Database.FillBranchCombo();
            foreach (string s in branch)
            {
                Branches.Items.Add(s);
            }
        }

        private void FillProductsCombo()
        {
            List<string> products = Database.FillProductCombo();
            foreach (string s in products)
            {
                ProductsCombo.Items.Add(s);
            }
        }

        /// <summary>
        /// Increases the order quantity by 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QtyUp_click(object sender, RoutedEventArgs e)
        {
            string qty = Quantity.Text.ToString();
            if(qty == "")
            {
                qty = "0";
            }
            int quantity = Convert.ToInt32(qty);
            AddToCart.IsEnabled = true;

            if (quantity == QTY)
            {
                plus.IsEnabled = false;
                error.Text = "This Branch only have " + QTY + " of the selected item \n If you want to order more try choosing a different branch";
                error.Visibility = Visibility.Visible;
            }
            else
            {
                quantity++;
                qty = quantity.ToString();
                Quantity.Text = qty;
            }

            minus.IsEnabled = true;
            GetPrice(PID);
        }

        /// <summary>
        /// decreases the order quantity by 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QtyDown_click(object sender, RoutedEventArgs e)
        {

            string qty = Quantity.Text.ToString();
            int quantity = Convert.ToInt32(qty);
            error.Visibility = Visibility.Collapsed;

            //quantity can't be less than zero
            if (quantity == 0)
            {
                minus.IsEnabled = false;
            }
            else
            {
                quantity--;
                qty = quantity.ToString();
                Quantity.Text = qty;
            }
            if (quantity == 0)
            {
                AddToCart.IsEnabled = false;

            }

            plus.IsEnabled = true;
            GetPrice(PID);
        }

        // branch changed
        private void selectionChangeClick(object sender, SelectionChangedEventArgs e)
        {
            error.Visibility = Visibility.Hidden;
            
            AddToCart.IsEnabled = false;
            string branchName = Branches.SelectedItem.ToString();
            Int32 branchID = Database.GetBranchID(branchName);
            BID = branchID;
            //check if product is selected or not.
            if (PID != 0)
            {
                GetQuantity(BID, PID);
                BranchID.Text = "This branch has " + QTY.ToString() + " of the selected items left";
                plus.IsEnabled = true;
                minus.IsEnabled = true;
                AddToCart.IsEnabled = true;
                Quantity.Text = "1";
            }

        }

        /// <summary>
        /// On Product selection chage Get product id, get branch id, get product description and show it in the textbox;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            error.Visibility = Visibility.Hidden;
            
            AddToCart.IsEnabled = false;
            string productName = ProductsCombo.SelectedItem.ToString();
            Int32 productID = Database.GetProductID(productName);
            PID = productID;
            string Description = Database.DisplayProduct(PID);
            ProductDesc.Text = Description;
            //if branch is selected enable increment,decrement and add to cart button and get the price for the product ID 
            if (BID != 0)
            {
                GetQuantity(BID, PID);
                BranchID.Text = "This branch has " + QTY.ToString() + " of the selected items left";
                plus.IsEnabled = true;
                minus.IsEnabled = true;
                AddToCart.IsEnabled = true;
                Quantity.Text = "1";
                GetPrice(PID);
            }
        }

        /// <summary>
        /// Gets the quantity of a product in a particular branch
        /// </summary>
        /// <param name="branchID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        private int GetQuantity(int branchID, int productID)
        {
            try
            {
                QTY = Database.GetQuantity(branchID, productID);
                return QTY;
            }
            catch (Exception)
            {
                BranchID.Text = "Select branch and product";
                return -1;
            }

        }

        /// <summary>
        /// Gets the price of the product
        /// </summary>
        /// <param name="productID"></param>
        private void GetPrice(int productID)
        {
            string qty = Quantity.Text;
            int quantity = Convert.ToInt32(qty);
            float[] price = Orders.CalculatePrice(productID, quantity);
            PriceCheck.Text = price[0].ToString();
            TaxCheck.Text = price[1].ToString();
            Price = price[2];
            Total.Text = price[2].ToString();
        }

        /// <summary>
        /// adds the item in the cart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCart_Click(object sender, RoutedEventArgs e)
        {
            bool valid = false;
            string qty = Quantity.Text;
            int quantity = Convert.ToInt32(qty);
            string str = custName.Text.Trim();
            string[] sr = { };
            sr = str.Split(' ');

            bool numValid = CheckNumber(qty);
            try
            {
                valid = CheckCustomerName(custName.Text.Trim());
            }
            catch (Exception)
            {
                MessageBox.Show("Full Name is Required");
                return;
            }
            //check if the given quantity is valid or not. if valid then add to cart. else show error
            if((valid == true) && (numValid == true) && (quantity<this.QTY) &&(quantity!=0))
            {               
                viewCart.IsEnabled = true;
                Database.firstName = sr[0];
                Database.lastName = sr[1];
                Orders.AddtoCart(ProductsCombo.SelectedItem.ToString(), Branches.SelectedItem.ToString(), Price, quantity, QTY);
                QTY = QTY - quantity;
                BranchID.Text = "This branch has " + QTY.ToString() + " of the selected items left";
                Price = 0;
                Total.Text = "0.0";
                PriceCheck.Text = "0.0";
                TaxCheck.Text = "0.0";
                AddToCart.IsEnabled = false;
                Quantity.Text = "1";
            }
            else
            {
               if(numValid == false)
                {
                    MessageBox.Show("Invalid Quantity");
                }
               else if(quantity>QTY)
                {
                    MessageBox.Show("This Branch has only "+QTY+" of the selected item in stock. Try another branch.");
                }
               else if(quantity<=0)
                {
                    MessageBox.Show("Can't Order 0 or less items");
                }
               else
                {
                    //if the name format is correct and it doesn't exist in the database ask to enroll first. On okay, go to Enroll page.
                    if (MessageBox.Show("This Customer Doesn't exist in in our Database. Please Enroll first") == MessageBoxResult.OK)
                    {
                        NavigationService.Navigate(new Uri("Enroll.xaml", UriKind.RelativeOrAbsolute));
                    }
                }
               

            }

        }
        /// <summary>
        /// Checks if number is valid
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        private bool CheckNumber(String PhoneNumber)
        {
            if (PhoneNumber == "")
                return false;
            foreach (char c in PhoneNumber)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void Greetings()
        {
            greetings.Text = "Hello " + Database.firstName;

        }

        /// <summary>
        /// go to the Cart page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Cart.xaml", UriKind.RelativeOrAbsolute));
        }


        /// <summary>
        /// Go to the Employee page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefundButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Employee.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Check if the name id valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckCustomerName(string name)
        {
            string str = "";
            str = name.Trim();
            string[] sr = { };
            bool valid = false;

                sr = str.Split(' ');
                string firstName = sr[0].Trim();
                string lastName = sr[1].Trim();
                valid = Database.CheckCustomer(firstName.ToLower(), lastName.ToLower());
                if (valid == true)
                {
                    return true;
                }
                else
                {
                    return false;
                } 
        
        }


    }
}
