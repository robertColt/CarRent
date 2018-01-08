using CarRent.Database;
using CarRent.ExtensionMethods;
using CarRent.Helper;
using CarRent.Models.RentInfo;
using CarRent.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CarRent.UserControls
{
    /// <summary>
    /// Interaction logic for CurrentUserControl.xaml
    /// </summary>
    public partial class MyUserControl : UserControl
    {
        public MyUserControl()
        {
            InitializeComponent();

            UserLabel.Content = "Welcome " + UserManager.CurrentUser.Name + " "
                + UserManager.CurrentUser.Surname;

            
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyRentalsTab.IsSelected)
            {
                List<Rent> myRentals = new RentDAO().GetRents(userId: UserManager.CurrentUser.Id);

                if (myRentals.Count == 0)
                {
                    Notificationlabel.ShowError("You don't have any rentals");
                }
                MyRentalsTable.ItemsSource = myRentals;
            }
            if (MyFinesTab.IsSelected)
            {
                List<Damage> myDamages = new DamageDAO().GetDamages(userId: UserManager.CurrentUser.Id);

                if (myDamages.Count == 0)
                {
                    Notificationlabel.ShowSuccess("You don't have any fines to pay");
                }
                MyFinesTable.ItemsSource = myDamages;
            }
        }

        private void RentPayButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Rent selectedRent = MyRentalsTable.SelectedItem != null ? MyRentalsTable.SelectedItem as Rent : null ; 

            if(selectedRent != null)
            {
                //todo implement pay logic

                if(!selectedRent.Paid)
                {
                    try
                    {
                        //todo retrieve bankaccount 
                        selectedRent.Paid = true;
                        new RentDAO().Update(selectedRent);
                        MyRentalsTable.Items.Refresh();
                        Notificationlabel.ShowSuccess("Payment successful !");
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLine(ex);
                        Notificationlabel.ShowError("Could not process paying");
                        selectedRent.Paid = false;
                    }
                }
            }
            else
            {
                Notificationlabel.ShowError("Nothing selected");
            }
        }

        private void FinePay_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Damage selectedDamage = MyFinesTable.SelectedItem != null ? MyFinesTable.SelectedItem as Damage : null;

            if (selectedDamage != null)
            {
                //todo implement pay logic

                if (!selectedDamage.Paid)
                {
                    try
                    {
                        //todo retrieve bankaccount 
                        selectedDamage.Paid = true;
                        new DamageDAO().Update(selectedDamage);
                        MyFinesTable.Items.Refresh();
                        Notificationlabel.ShowSuccess("Payment successful !");
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLine(ex);
                        Notificationlabel.ShowError("Could not process paying");
                        selectedDamage.Paid = false;
                    }
                }
            }
            else
            {
                Notificationlabel.ShowError("Nothing selected");
            }
        }
    }
}
