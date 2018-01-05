using CarRent.Database;
using CarRent.Models;
using CarRent.Models.RentInfo;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace CarRent.UserControls
{
    /// <summary>
    /// Interaction logic for AdminControl.xaml
    /// </summary>
    public partial class AdminControl : UserControl
    {

        private Type selectedType;

        public AdminControl()
        {
            InitializeComponent();

            List<Type> databaseList = new List<Type>()
            {
                typeof(Damage),
                typeof(Invoice),
                typeof(InvoiceDetail),
                typeof(Rent),
                typeof(Vehicle),
                typeof(BankAccount),
                typeof(BlackList),
                typeof(User),
                typeof(UserDetails),
            };

            DatabaseComboBox.ItemsSource = databaseList;
            DatabaseComboBox.SelectedItem = databaseList[0];
        }

        private void DatabaseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedType = (Type)DatabaseComboBox.SelectedItem;

            GridView gridView = new GridView();

            foreach(PropertyInfo property in selectedType.GetProperties())
            {
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = property.Name,
                    DisplayMemberBinding = new Binding(property.Name),
                });
            }

            if(selectedType == typeof(Damage))
            {
                this.DatabaseListView.ItemsSource = new DamageDAO().GetDamages();
            }
            else if (selectedType == typeof(Invoice))
            {
                this.DatabaseListView.ItemsSource = new InvoiceDAO().GetInvoices();
            }
            else if (selectedType == typeof(InvoiceDetail))
            {
                this.DatabaseListView.ItemsSource = new InvoiceDetailDAO().GetInvoiceDetails();
            }
            else if (selectedType == typeof(Rent))
            {
                this.DatabaseListView.ItemsSource = new RentDAO().GetRents();
            }
            else if (selectedType == typeof(Vehicle))
            {
                this.DatabaseListView.ItemsSource = new VehicleDAO().GetVehicles();
            }
            else if (selectedType == typeof(BankAccount))
            {
                this.DatabaseListView.ItemsSource = new BankAccountDAO().GetBankAccounts();
            }
            else if (selectedType == typeof(BlackList))
            {
                this.DatabaseListView.ItemsSource = new BlackListDAO().GetBlackLists();
            }
            else if (selectedType == typeof(User))
            {
                this.DatabaseListView.ItemsSource = new UserDAO().GetUsers();
            }
            else if (selectedType == typeof(UserDetails))
            {
                this.DatabaseListView.ItemsSource = new UserDetailsDAO().GetUserDetails();
            }

            this.DatabaseListView.View = gridView;
        }
    }
}
