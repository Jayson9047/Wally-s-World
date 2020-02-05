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
    /// Interaction logic for Employee.xaml
    /// </summary>
    public partial class Employee : Page
    {
        public Employee()
        {
            InitializeComponent();
            //ShowOrderInfo();
            FillBranchCombo();
            orderDetails.IsEnabled = false;
        }

        List<string> customerHistory = new List<string>();
        List<string> custHist = new List<string>();

        //private void RefundShow_Click(object sender, RoutedEventArgs e)
        //{
        //    refundButton.IsEnabled = true;
        //    const int refundRequest = -1;
        //    if (employeeBox.Items.Count != 0)
        //    {
        //        employeeBox.Items.Clear();
        //    }
        //    ShowOrderInfo(refundRequest);
        //}

        //private void ShowOrderInfo(int status = -1)
        //{
        //    if (employeeBox.Items.Count != 0)
        //    {
        //        employeeBox.Items.Clear();
        //    }
        //    customerHistory = Database.ShowData(status)[0];
        //    custHist = Database.ShowData(status)[1];
        //    foreach (string s in customerHistory)
        //    {
        //        employeeBox.Items.Add(s);
        //    }
        //}

        /// <summary>
        /// Refunds the order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefundButton_Click(object sender, RoutedEventArgs e)
        {
            AllInfo.Text = "";
            int i = 0;
            int index = employeeBox.SelectedIndex;
            
            string productName = "";
            string branchName = "";
            int qty = 0;
            int OrderID = 0;
            if (employeeBox.SelectedIndex >= 0)
            {
                //Extract all the values and refund the selected order 
                foreach (string s in custHist)
                {
                    int pFrom = s.IndexOf("ProductName:") + "ProductName:".Length;
                    int pTo = s.IndexOf(" BranchName:");
                    productName = s.Substring(pFrom, pTo - pFrom).Trim();
                    pFrom = s.IndexOf(" BranchName:") + " BranchName:".Length;
                    pTo = s.IndexOf("Quantity:");
                    branchName = s.Substring(pFrom, pTo - pFrom).Trim();
                    pFrom = s.IndexOf("Quantity:") + "Quantity:".Length;
                    pTo = s.IndexOf("OrderID:");
                    qty = Convert.ToInt32(s.Substring(pFrom, pTo - pFrom).Trim());
                    pFrom = s.IndexOf("OrderID:") + "OrderID:".Length;
                    pTo = s.IndexOf("\n");
                    OrderID = Convert.ToInt32(s.Substring(pFrom, pTo - pFrom).Trim());
                    if (i == index)
                    {
                        break;
                    }

                    i++;
                }
                Orders.RefundOrder(OrderID, qty, branchName, productName);
                employeeBox.Items.Clear();
                ShowInfo();

            }
        }


        //private void RefundedOrders_Click(object sender, RoutedEventArgs e)
        //{
        //    refundButton.IsEnabled = false;
        //    const int refundedOrderStatus = 0;
        //    if (employeeBox.Items.Count != 0)
        //    {
        //        employeeBox.Items.Clear();
        //    }
        //    ShowOrderInfo(refundedOrderStatus);
        //}

            /// <summary>
            /// Go to order page
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Order.xaml", UriKind.RelativeOrAbsolute));
        }


        private void FillBranchCombo()
        {
            //fill up the branch
            List<string> branch = Database.FillBranchCombo();
            foreach (string s in branch)
            {
                Branches.Items.Add(s);
            }
        }

        /// <summary>
        /// Shows the inventory for the selected branch in the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            AllInfo.Text = "";
            //clear list box
            employeeBox.Items.Clear();
            List<string> BranchInventory = new List<string>();
            //disable button
            refundButton.IsEnabled = false;
            //chekc which item is selected and show the details for that item
            if (Branches.SelectedItem != null)
            {
                string branchName = Branches.SelectedItem.ToString();
                int bID = Database.GetBranchID(branchName);
                BranchInventory = Database.DisplayInventory(bID);
                AllInfo.Text = "Branch Name: " + branchName + "\nBranch ID: " + bID.ToString();
                foreach (string s in BranchInventory)
                {
                    employeeBox.Items.Add(s);
                }
            }
            else
            {
                MessageBox.Show("Please Select a Branch First");
            }


        }
        /// <summary>
        /// Gets the order details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AllInfo.Text = "";
            Orders.previousOrder = true;
            
            int index = employeeBox.SelectedIndex;
            string details = "";
            int i = 0;

            //Extract all the values from the selected order and show the details of the selected order in sales record page
            if (employeeBox.SelectedIndex >= 0)
            {
                foreach (string s in custHist)
                {
                    string status = "";
                    int pFrom = s.IndexOf("OrderID:") + "OrderID:".Length;
                    int pTo = s.IndexOf("\n");
                    details = "OrderID: " + s.Substring(pFrom, pTo - pFrom).Trim() + "\n";
                    pFrom = s.IndexOf("ProductName: ") + "ProductName: ".Length;
                    pTo = s.IndexOf("BranchName:");
                    details = details + " Product Name:" + s.Substring(pFrom, pTo - pFrom).Trim() + "\n";
                    pFrom = s.IndexOf("BranchName:") + "BranchName:".Length;
                    pTo = s.IndexOf("Quantity:");
                    details = details + " Purchased From: " + s.Substring(pFrom, pTo - pFrom).Trim() + "\n";
                    pFrom = s.IndexOf("Quantity:") + "Quantity:".Length;
                    pTo = s.IndexOf("OrderID:");
                    details = details + " Quantity: " + s.Substring(pFrom, pTo - pFrom).Trim() + "\n";
                    pFrom = s.IndexOf("OrderStatus:") + "OrderStatus:".Length;
                    pTo = s.IndexOf("Done");
                    int OrderStatus = Convert.ToInt32(s.Substring(pFrom, pTo - pFrom).Trim());
                    if (OrderStatus == 0)
                        status = "refunded";
                    else if (OrderStatus == 1)
                        status = "Paid";
                    else
                        status = "Refund Requested";
                    details = details + " Order Status: " + status + "\nPurchased By: " + Database.firstName + " " + Database.lastName + "\n";
                    pFrom = s.IndexOf(" Order Date: ") + " Order Date: ".Length;
                    pTo = s.IndexOf("date");
                    details = details + " Date: " + s.Substring(pFrom, pTo - pFrom).Trim() + "\n";

                    if (i == index)
                    {
                        break;
                    }

                    i++;
                }
                Orders.OrderDesc = details;
                Orders.orderPage = false;   
                NavigationService.Navigate(new Uri("SalesRecord.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                MessageBox.Show("Please select an order first!");
            }
            
        }

        /// <summary>
        /// Searches the customer and shows the orders of the customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click_1(object sender, RoutedEventArgs e)
        {
            employeeBox.Items.Clear();
            
            string str = "";
            str = search.Text.Trim();
            string[] sr = { };
            bool valid = false;
            try
            {
                sr = str.Split(' ');
                string firstName = sr[0].Trim();
                string lastName = sr[1].Trim();
                //check if customer exists
                valid = Database.CheckCustomer(firstName.ToLower(), lastName.ToLower());
                //if exists then show all the orders of the customer in the listbox else show error message
                if (valid == true)
                {
                    customerHistory = Database.CustomerOrderHistory(firstName + " " + lastName)[0];
                    custHist = Database.CustomerOrderHistory(firstName + " " + lastName)[1];
                    foreach (string s in customerHistory)
                    {
                        employeeBox.Items.Add(s);
                    }
                    int cID = Database.GetCustomerID(firstName, lastName);
                    AllInfo.Text = "Customer Name: " + firstName + " " + lastName + "\nCustomer ID: " + cID.ToString();
                    Database.firstName = firstName;
                    Database.lastName = lastName;
                    refundButton.IsEnabled = true;
                    orderDetails.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show(firstName + " " + lastName + " is not enrolled.");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Full Name is Required");
            }

        }

        /// <summary>
        /// This method changes the text over the list box to show the searched customer's name and id
        /// </summary>
        public void ShowInfo()
        {
            string name = Database.firstName + " " + Database.lastName;

            customerHistory = Database.CustomerOrderHistory(name)[0];
            custHist = Database.CustomerOrderHistory(name)[1];
            foreach (string s in customerHistory)
            {
                employeeBox.Items.Add(s);
            }
            int cID = Database.GetCustomerID(Database.firstName, Database.lastName);
            AllInfo.Text = "Customer Name: " + name + "\nCustomer ID: " + cID.ToString();
        }

        private void Branches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orderDetails.IsEnabled = false;   //disable orderdetails button when branch selection is changed
        }

        private void Enroll_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Enroll.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
