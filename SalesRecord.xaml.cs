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
    /// Interaction logic for SalesRecord.xaml
    /// </summary>
    public partial class SalesRecord : Page
    {
        /// <summary>
        /// constructor for sales record if the recently created order is requested to be shown, then it calls the ShowSalesRecord method.
        /// Otherwise, for any previous records, it calls the ShowPreviousSalesRecord() method
        /// </summary>
        public SalesRecord()
        {
            InitializeComponent();
            if (Orders.previousOrder == false)
            {
                ShowSalesRecord();
            }
            else
            {
                ShowPreviousSalesRecord();
            }
        }

        /// <summary>
        /// Show the details for the order just created.
        /// </summary>
        public void ShowSalesRecord()
        {
            paid.Visibility = Visibility.Visible;
            string lines = "";
            foreach (string s in Orders.OrderDetails)
            {
                lines = lines + s;
                Sales.Text = lines;
            }
            Orders.OrderDetails.Clear();
            string date = DateTime.Now.ToString("dd/MM/yyyy");
            thank.Text = "Thank you for Shopping at\nWallys World\nCustomer Name: " + Database.firstName + " " + Database.lastName + "\nDate: " + date;
        }

        /// <summary>
        /// show the details for any previous sales record
        /// </summary>
        public void ShowPreviousSalesRecord()
        {
            Sales.Text = Orders.OrderDesc;
            paid.Visibility = Visibility.Hidden;

        }


        /// <summary>
        /// On click go to the Order page if a new order is created, other wise go to employee page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
           if(Orders.orderPage == true)
            {
                NavigationService.Navigate(new Uri("Order.xaml", UriKind.RelativeOrAbsolute));
            }
           else
            {
                NavigationService.Navigate(new Uri("Employee.xaml", UriKind.RelativeOrAbsolute));
            }
            
           
        }
        /// <summary>
        /// Go to the employee page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Employee.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
