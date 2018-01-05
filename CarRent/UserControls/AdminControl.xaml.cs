using CarRent.Database;
using CarRent.Helper;
using CarRent.ExtensionMethods;
using CarRent.Models;
using CarRent.Models.RentInfo;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;

namespace CarRent.UserControls
{
    /// <summary>
    /// Interaction logic for AdminControl.xaml
    /// </summary>
    public partial class AdminControl : UserControl
    {

        private Type selectedType;

        private int numOfRecords;

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

            foreach (PropertyInfo property in selectedType.GetProperties())
            {
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = property.Name,
                    DisplayMemberBinding = new Binding(property.Name),
                });
            }

            if (selectedType == typeof(Damage))
            {
                this.TableView.ItemsSource = new DamageDAO().GetDamages();
            }
            else if (selectedType == typeof(Invoice))
            {
                this.TableView.ItemsSource = new InvoiceDAO().GetInvoices();
            }
            else if (selectedType == typeof(InvoiceDetail))
            {
                this.TableView.ItemsSource = new InvoiceDetailDAO().GetInvoiceDetails();
            }
            else if (selectedType == typeof(Rent))
            {
                this.TableView.ItemsSource = new RentDAO().GetRents();
            }
            else if (selectedType == typeof(Vehicle))
            {
                this.TableView.ItemsSource = new VehicleDAO().GetVehicles();
            }
            else if (selectedType == typeof(BankAccount))
            {
                this.TableView.ItemsSource = new BankAccountDAO().GetBankAccounts();
            }
            else if (selectedType == typeof(BlackList))
            {
                this.TableView.ItemsSource = new BlackListDAO().GetBlackLists();
            }
            else if (selectedType == typeof(User))
            {
                this.TableView.ItemsSource = new UserDAO().GetUsers();
            }
            else if (selectedType == typeof(UserDetails))
            {
                this.TableView.ItemsSource = new UserDetailsDAO().GetUserDetails();
            }

            numOfRecords = ((ICollection)TableView.ItemsSource).Count;
        }

        private void TableView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((ICollection)TableView.ItemsSource).Count != numOfRecords)
            {
                DebugLog.WriteLine("Items more");
            }

            DebugLog.WriteLine(TableView.SelectedItem);
        }

        private void TableView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (numOfRecords == ((ICollection)TableView.ItemsSource).Count)
            {
                try
                {
                    if (selectedType == typeof(Damage))
                    {
                        new DamageDAO().Update(TableView.SelectedItem as Damage);
                    }
                    else if (selectedType == typeof(Invoice))
                    {
                        new InvoiceDAO().Update(TableView.SelectedItem as Invoice);
                    }
                    else if (selectedType == typeof(InvoiceDetail))
                    {
                        new InvoiceDetailDAO().Update(TableView.SelectedItem as InvoiceDetail);
                    }
                    else if (selectedType == typeof(Rent))
                    {
                        new RentDAO().Update(TableView.SelectedItem as Rent);
                    }
                    else if (selectedType == typeof(Vehicle))
                    {
                        new VehicleDAO().Update(TableView.SelectedItem as Vehicle);
                    }
                    else if (selectedType == typeof(BankAccount))
                    {
                        new BankAccountDAO().Update(TableView.SelectedItem as BankAccount);
                    }
                    else if (selectedType == typeof(BlackList))
                    {
                        new BlackListDAO().Update(TableView.SelectedItem as BlackList);
                    }
                    else if (selectedType == typeof(User))
                    {
                        new UserDAO().Update(TableView.SelectedItem as User);
                    }
                    else if (selectedType == typeof(UserDetails))
                    {
                        new UserDetailsDAO().Update(TableView.SelectedItem as UserDetails);
                    }
                }
                catch (Exception ex)
                {
                    Notificationlabel.ShowError("Could not edit Data");
                } 
            }
            else
            {
                try
                {
                    if (selectedType == typeof(Damage))
                    {
                        new DamageDAO().Insert(TableView.SelectedItem as Damage);
                    }
                    else if (selectedType == typeof(Invoice))
                    {
                        new InvoiceDAO().Insert(TableView.SelectedItem as Invoice);
                    }
                    else if (selectedType == typeof(InvoiceDetail))
                    {
                        new InvoiceDetailDAO().Insert(TableView.SelectedItem as InvoiceDetail);
                    }
                    else if (selectedType == typeof(Rent))
                    {
                        new RentDAO().Insert(TableView.SelectedItem as Rent);
                    }
                    else if (selectedType == typeof(Vehicle))
                    {
                        new VehicleDAO().Insert(TableView.SelectedItem as Vehicle);
                    }
                    else if (selectedType == typeof(BankAccount))
                    {
                        new BankAccountDAO().Insert(TableView.SelectedItem as BankAccount);
                    }
                    else if (selectedType == typeof(BlackList))
                    {
                        new BlackListDAO().Insert(TableView.SelectedItem as BlackList);
                    }
                    else if (selectedType == typeof(User))
                    {
                        new UserDAO().Insert(TableView.SelectedItem as User);
                    }
                    else if (selectedType == typeof(UserDetails))
                    {
                        new UserDetailsDAO().Insert(TableView.SelectedItem as UserDetails);
                    }
                }
                catch (Exception ex)
                {
                    Notificationlabel.ShowError("Could not insert Data");
                }
            }
            Notificationlabel.ShowSuccess("Data edited successfully!");
        }

        private void TableView_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            DebugLog.WriteLine("ended editing row");
        }
    }
}
