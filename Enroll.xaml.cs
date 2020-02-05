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
    /// Interaction logic for Enroll.xaml
    /// </summary>
    public partial class Enroll : Page
    {
        public Enroll()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Checks if the given name contains all alpha characters or not
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <returns>1 if first name is not all alpha, 2 if last name is not all alpha, -1 if firtname is null, -2 if last name is null
        /// 0 if all valid</returns>
        private int CheckName(String FirstName, String LastName)
        {
            if (FirstName == "")
                return -1;
            if (LastName == "")
                return -2;
            foreach (char c in FirstName)
            {
                if (char.IsDigit(c))
                    return 1;
            }
            foreach (char c in LastName)
            {
                if (char.IsDigit(c))
                    return 2;
            }
            return 0;
        }

        /// <summary>
        /// Checks if the phone number contains all numbers
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns>true if all numbers, false for invalid input</returns>
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

        /// <summary>
        /// Enrolls a customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String firstName = FirstName.Text.ToLower().Trim();
            String lastName = LastName.Text.ToLower().Trim();
            String PhoneNo = PhoneNumber.Text.Trim();
            bool check = true;
            bool checkCustomer = true;
            //check if customer already exists
            checkCustomer = Database.CheckCustomer(firstName, lastName);
            if (checkCustomer == true)
            {
                MessageBox.Show("Customer Already Exists!");
            }
            else                            //check validity
            {

                if (CheckNumber(PhoneNo) == false)  
                {
                    PhoneError.Visibility = Visibility.Visible;
                    check = false;
                }
                else
                {
                    PhoneError.Visibility = Visibility.Hidden;
                }


                if (CheckName(firstName, lastName) == 1)
                {
                    firstNameError.Visibility = Visibility.Visible;
                    check = false;
                }
                else
                {
                    firstNameError.Visibility = Visibility.Hidden;
                }
                if (CheckName(firstName, lastName) == 2)
                {
                    lastNameError.Visibility = Visibility.Visible;
                    check = false;
                }
                else
                {
                    lastNameError.Visibility = Visibility.Hidden;
                }
                if (CheckName(firstName, lastName) == -1)
                {
                    firstNameError.Visibility = Visibility.Visible;
                    check = false;
                }
                if (CheckName(firstName, lastName) == -2)
                {
                    lastNameError.Visibility = Visibility.Visible;
                    check = false;
                }
                if (check == true)
                {
                    Database.firstName = firstName;
                    Database.lastName = lastName;
                    Database.phoneNo = PhoneNo;
                    // enroll customer
                    Database.EnrollCustomer(FirstName.Text.ToString().Trim(), LastName.Text.ToString().Trim(), PhoneNumber.Text.ToString().Trim());
                    success.Visibility = Visibility.Visible;
                    FirstName.Text = "";
                    LastName.Text = "";
                    PhoneNumber.Text = "";
                }
            }

        }

        /// <summary>
        /// Goes back to Order page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            success.Visibility = Visibility.Hidden;
            NavigationService.Navigate(new Uri("Order.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
