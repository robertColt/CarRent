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
using System.Collections.ObjectModel;
using System.Linq;
using CarRent.ViewModel;

namespace CarRent.UserControls
{
    /// <summary>
    /// Interaction logic for AdminControl.xaml
    /// </summary>
    public partial class AdminControl : UserControl
    {
        private enum Action
        {
            UPDATE, INSERT
        }

        private Type selectedType;

        private int numOfRecords;

        private bool rowEdited;

        //private HashSet<Model> objectsToUpdate = new HashSet<Model>();

        private Dictionary<Model, Action> objectsToUpdate = new Dictionary<Model, Action>();

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
                ObservableCollection<Damage> obsCollection =
                    new ObservableCollection<Damage>(new DamageDAO().GetDamages());
                obsCollection.CollectionChanged += 
                    new System.Collections.Specialized.NotifyCollectionChangedEventHandler(TableDataChanged);
                this.TableView.ItemsSource = obsCollection;
                //this.DataGrid.DataSource = obsCollection;
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

        private void TableView_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            rowEdited = true;

            if (numOfRecords == ((ICollection)TableView.ItemsSource).Count)
            {
                try
                {
                    objectsToUpdate.Add(TableView.SelectedItem as Model, Action.UPDATE);
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    //Notificationlabel.ShowError("Could not edit Data");
                }
            }
            else
            {
                try
                {
                    objectsToUpdate.Add(TableView.SelectedItem as Model, Action.INSERT);
                    numOfRecords++;
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    //Notificationlabel.ShowError("Could not insert Data");
                }
            }
        }

        void TableDataChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            /// Work on e.Action here (can be Add, Move, Replace...)
            //DebugLog.WriteLine("changed");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedType == typeof(Damage))
                {
                    new DamageDAO().Delete((TableView.SelectedItem as Damage).Id);
                }
                else if (selectedType == typeof(Invoice))
                {
                    new InvoiceDAO().Delete((TableView.SelectedItem as Invoice).Id);
                }
                else if (selectedType == typeof(InvoiceDetail))
                {
                    new InvoiceDetailDAO().Delete((TableView.SelectedItem as InvoiceDetail).Id);
                }
                else if (selectedType == typeof(Rent))
                {
                    new RentDAO().Delete((TableView.SelectedItem as Rent).Id);
                }
                else if (selectedType == typeof(Vehicle))
                {
                    new VehicleDAO().Delete((TableView.SelectedItem as Vehicle).Id);
                }
                else if (selectedType == typeof(BankAccount))
                {
                    new BankAccountDAO().Delete((TableView.SelectedItem as BankAccount).Id);
                }
                else if (selectedType == typeof(BlackList))
                {
                    new BlackListDAO().Delete((TableView.SelectedItem as BlackList).Id);
                }
                else if (selectedType == typeof(User))
                {
                    new UserDAO().Delete((TableView.SelectedItem as User).Id);
                }
                else if (selectedType == typeof(UserDetails))
                {
                    new UserDetailsDAO().Delete((TableView.SelectedItem as UserDetails).Id);
                }

                TableView.Items.Remove(TableView.SelectedItem);
                TableView.Items.Refresh();
            }
            catch (Exception ex)
            {
                DebugLog.WriteLine(ex);
                Notificationlabel.ShowError("Could not delete Data");
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if(objectsToUpdate.Count == 0)
            {
                Notificationlabel.ShowSuccess("Everything up to date");
                return;
            }

            if (!rowEdited)
            {
                Notificationlabel.ShowError("Please First Press Enter to commit changes!");
                rowEdited = true;
                return;
            }
            Dictionary<Model, bool> successfulEdit = new Dictionary<Model, bool>();

            foreach (Model model in objectsToUpdate.Keys)
            {
                if(objectsToUpdate[model] == Action.UPDATE)
                {
                    successfulEdit.Add(model,Update(model));
                }
                else
                {
                    successfulEdit.Add(model, Insert(model));
                }
            }

            foreach(Model model in successfulEdit.Keys)
            {
                if (successfulEdit[model])
                {
                    objectsToUpdate.Remove(model);
                }
            }

            TableView.Items.Refresh();
        }

        private bool Update(Model model)
        {

            Type selectedType = model.GetType();
            try
            {
                if (selectedType == typeof(Damage))
                {
                    //Dispatcher.BeginInvoke(new Action(() => new DamageDAO().Update(TableView.SelectedItem as Damage)),
                    //    System.Windows.Threading.DispatcherPriority.Background);
                    new DamageDAO().Update(model as Damage);
                }
                else if (selectedType == typeof(Invoice))
                {
                    new InvoiceDAO().Update(model as Invoice);
                }
                else if (selectedType == typeof(InvoiceDetail))
                {
                    new InvoiceDetailDAO().Update(model as InvoiceDetail);
                }
                else if (selectedType == typeof(Rent))
                {
                    new RentDAO().Update(model as Rent);
                }
                else if (selectedType == typeof(Vehicle))
                {
                    new VehicleDAO().Update(model as Vehicle);
                }
                else if (selectedType == typeof(BankAccount))
                {
                    new BankAccountDAO().Update(model as BankAccount);
                }
                else if (selectedType == typeof(BlackList))
                {
                    new BlackListDAO().Update(model as BlackList);
                }
                else if (selectedType == typeof(User))
                {
                    new UserDAO().Update(model as User);
                }
                else if (selectedType == typeof(UserDetails))
                {
                    new UserDetailsDAO().Update(model as UserDetails);
                }

                return true;
            }
            catch (Exception ex)
            {
                Notificationlabel.ShowError("Could not edit Data " + model.GetType().Name + " : " + ex.Message);
                return false;
            }
        }

        private bool Insert(Model model)
        {
            Type selectedType = model.GetType();

            try
            {
                if (selectedType == typeof(Damage))
                {
                    new DamageDAO().Insert(model as Damage);
                }
                else if (selectedType == typeof(Invoice))
                {
                    new InvoiceDAO().Insert(model as Invoice);
                }
                else if (selectedType == typeof(InvoiceDetail))
                {
                    new InvoiceDetailDAO().Insert(model as InvoiceDetail);
                }
                else if (selectedType == typeof(Rent))
                {
                    new RentDAO().Insert(model as Rent);
                }
                else if (selectedType == typeof(Vehicle))
                {
                    new VehicleDAO().Insert(model as Vehicle);
                }
                else if (selectedType == typeof(BankAccount))
                {
                    new BankAccountDAO().Insert(model as BankAccount);
                }
                else if (selectedType == typeof(BlackList))
                {
                    new BlackListDAO().Insert(model as BlackList);
                }
                else if (selectedType == typeof(User))
                {
                    new UserDAO().Insert(model as User);
                }
                else if (selectedType == typeof(UserDetails))
                {
                    new UserDetailsDAO().Insert(model as UserDetails);
                }

                return true;
            }
            catch (Exception ex)
            {
                Notificationlabel.ShowError("Could not insert Data " + model.GetType().Name + " : " + ex.Message);
                return false;
            }
        }

        private void TableView_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            DebugLog.WriteLine("beginning edit");
            //rowEdited = false;
        }

        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                rowEdited = true;
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            UserControlManager.Instance.CurrentUserControl = new LoginControl();
        }
    }
}
