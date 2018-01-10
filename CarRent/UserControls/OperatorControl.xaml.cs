using CarRent.Database;
using CarRent.ExtensionMethods;
using CarRent.Helper;
using CarRent.Models;
using CarRent.Models.RentInfo;
using CarRent.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace CarRent.UserControls
{
    /// <summary>
    /// Interaction logic for OperatorControl.xaml
    /// </summary>
    public partial class OperatorControl : UserControl
    {

        private Damage currentDamage = null;
        private Vehicle currentVehicle = null;

        public OperatorControl()
        {
            InitializeComponent();

            OperatorLabel.Content = "Welcome Operator " + UserManager.CurrentUser.Name + " "
                + UserManager.CurrentUser.Surname;

            PopulateComboBoxes();

            PaidCheckBox.IsChecked = false;
            DamagedCheckBox.IsChecked = false;
        }

        private void PopulateComboBoxes()
        {
            try
            {
                List<User> users = new UserDAO().GetUsers(function: User.Function.USER.ToString());
                users.Add(new User() { Id = -1, Username = "no-user"});
                FineUserBox.ItemsSource = users;
                UserBox.ItemsSource = users;
                LicenseCategoryComboBox.ItemsSource = Enum.GetValues(typeof(Vehicle.LicenseCategories));
                VehicleBox.ItemsSource = new VehicleDAO().GetVehicles();
            }
            catch (Exception ex)
            {
                NotificationLabel.ShowError("Something went wrong with retrieving data");
                DebugLog.WriteLine(ex);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl)) return;

            NotificationLabel.Content = "";

            if (AllRentalsTab.IsSelected)
            {
                List<Rent> allRentals = new RentDAO().GetRents();

                if (allRentals.Count == 0)
                {
                    NotificationLabel.ShowError("There aren't any rentals");
                }
                AllRentalsTable.ItemsSource = allRentals;
            }
            else if (AllFinesTab.IsSelected)
            {
                List<Damage> allDamages = new DamageDAO().GetDamages();

                if (allDamages.Count == 0)
                {
                    NotificationLabel.ShowSuccess("There are no fines");
                }

                try
                {
                    List<User> users = new UserDAO().GetUsers(function: User.Function.USER.ToString());
                    FineUserBox.ItemsSource = users;
                    LicenseCategoryComboBox.ItemsSource = Enum.GetValues(typeof(Vehicle.LicenseCategories));
                }
                catch (Exception ex)
                {
                    NotificationLabel.ShowError("Something went wrong with retrieving data");
                    DebugLog.WriteLine(ex);
                }

                AllDamagesTable.ItemsSource = allDamages;
            }
            else if (AllCarsTab.IsSelected)
            {
                List<Vehicle> myVehicles = new VehicleDAO().GetVehicles();

                if (myVehicles != null & myVehicles.Count() == 0)
                {
                    NotificationLabel.ShowError("No vehicles to display");
                }

                try
                {
                    List<User> users = new UserDAO().GetUsers(function: User.Function.USER.ToString());
                    users.Add(new User() { Id = -1, Username = "no-user" });
                    UserBox.ItemsSource = users;
                    LicenseCategoryComboBox.ItemsSource = Enum.GetValues(typeof(Vehicle.LicenseCategories));
                }
                catch (Exception ex)
                {
                    NotificationLabel.ShowError("Something went wrong with retrieving data");
                    DebugLog.WriteLine(ex);
                }

                VehicleTable.ItemsSource = myVehicles;
            }
            else if (SettingsTab.IsSelected)
            {
                User currentUser = UserManager.CurrentUser;
                try
                {
                    UserDetails details = new UserDetailsDAO().GetUserDetails(userId: currentUser.Id).First();

                    UsernameBox.Text = currentUser.Username;
                    NameBox.Text = currentUser.Name;
                    SurnameBox.Text = currentUser.Surname;
                    EmailBox.Text = details.Email;
                    StreetBox.Text = details.Street;
                    CityBox.Text = details.City;
                    ZipCodeBox.Text = details.ZipCode;
                    CountryBox.Text = details.Country;
                    BirthDatePicker.SelectedDate = details.BirthDate;
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("A problem occured! Could not retrieve your data!");
                    return;
                }
            }
            else if (AllUsersTab.IsSelected)
            {
                try
                {
                    AllUsersTable.ItemsSource = new UserDAO().GetUsers();
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Could not retrieve all users");
                }
            }
        }

        private void SetReturnedButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AllRentalsTable.SelectedItem == null)
            {
                NotificationLabel.ShowError("Please select a rental first !");
            }

            Rent rent = (Rent)AllRentalsTable.SelectedItem;

            if (rent.Returned)
            {
                NotificationLabel.ShowError("Car already returned");
                return;
            }
            else
            {
                try
                {
                    Vehicle vehicle = new VehicleDAO().GetVehicles(id: rent.VehicleId).First();

                    rent.Returned = true;
                    rent.EndTime = DateTime.Now;
                    double totalAmount = (rent.EndTime.Value - rent.BeginTime).TotalHours * vehicle.PriceHour;
                    rent.TotalAmount = totalAmount = totalAmount * rent.Discount;
                    new RentDAO().Update(rent);
                    NotificationLabel.ShowSuccess("Set returned successful !");
                    AllRentalsTable.Items.Refresh();

                    vehicle.UserId = null;

                    new VehicleDAO().Update(vehicle);
                }
                catch (InvalidOperationException ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("A problem occured vehicle userid could not be modified!");
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("A problem occured !");
                }
            }
        }

        private void SetPaidButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AllRentalsTable.SelectedItem == null)
            {
                NotificationLabel.ShowError("Please select a rental first !");
            }

            Rent rent = (Rent)AllRentalsTable.SelectedItem;

            if (rent.Paid)
            {
                NotificationLabel.ShowError("Rental already paid");
                return;
            }
            else
            {
                try
                {
                    rent.Paid = true;
                    new RentDAO().Update(rent);
                    NotificationLabel.ShowSuccess("Set paid successful !");
                    AllRentalsTable.Items.Refresh();
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("A problem occured !");
                }
            }
        }

        private void VehicleTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentVehicle = (Vehicle)VehicleTable.SelectedItem;
        }

        private void AddFine_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FineAmountBox.Text.Trim())
                || FineUserBox.SelectedItem == null || VehicleBox.SelectedItem == null || DatePicker.SelectedDate == null)
            {
                NotificationLabel.ShowError("All fields are required");
            }
            else
            {
                try
                {
                    Damage damage = new Damage()
                    {
                        FineAmount = Double.Parse(FineAmountBox.Text),
                        UserId = ((User)FineUserBox.SelectedItem).Id,
                        VehicleId = ((Vehicle)VehicleBox.SelectedItem).Id,
                        Date = DatePicker.SelectedDate.Value,
                        Paid = PaidCheckBox.IsChecked.Value
                    };

                    new DamageDAO().Insert(damage);

                    ((List<Damage>)AllDamagesTable.ItemsSource).Add(damage);
                    AllDamagesTable.Items.Refresh();
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Could not add fine");
                }
            }
        }

        private void SaveDamageChanges_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FineAmountBox.Text.Trim())
                || FineUserBox.SelectedItem == null || VehicleBox.SelectedItem == null || DatePicker.SelectedDate == null)
            {
                NotificationLabel.ShowError("All fields are required");
            }
            else
            {
                if (currentDamage == null)
                {
                    NotificationLabel.ShowError("Please double click on Fine first");
                    return;
                }

                try
                {
                    currentDamage.FineAmount = Double.Parse(FineAmountBox.Text);
                    currentDamage.UserId = ((User)FineUserBox.SelectedItem).Id;
                    currentDamage.VehicleId = ((Vehicle)VehicleBox.SelectedItem).Id;
                    currentDamage.Date = DatePicker.SelectedDate.Value;
                    currentDamage.Paid = PaidCheckBox.IsChecked.Value;

                    new DamageDAO().Update(currentDamage);

                    AllDamagesTable.Items.Refresh();
                    NotificationLabel.ShowSuccess("Fine updated successfully");
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Could not update fine");
                }
            }
        }

        private void AllFinesTable_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (AllDamagesTable.SelectedItem == null)
            {
                return;
            }

            currentDamage = (Damage)AllDamagesTable.SelectedItem;
            FineAmountBox.Text = currentDamage.FineAmount.ToString();
            IEnumerable<User> users = ((IEnumerable<User>)FineUserBox.ItemsSource);
            IEnumerable<Vehicle> vehicles = ((IEnumerable<Vehicle>)VehicleBox.ItemsSource);

            IEnumerable<User> singleUser = users.Where((u) => u.Id == currentDamage.UserId);
            IEnumerable<Vehicle> singleVehicle = vehicles.Where((v) => v.Id == currentDamage.VehicleId);

            FineUserBox.SelectedItem = singleUser.Count() > 0 ? users.First() : null;
            VehicleBox.SelectedItem = singleVehicle.Count() > 0 ? vehicles.First() : null;
            DatePicker.SelectedDate = currentDamage.Date;
            PaidCheckBox.IsChecked = currentDamage.Paid;
        }

        private void VehicleTable_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (VehicleTable.SelectedItem == null)
            {
                return;
            }

            currentVehicle = (Vehicle)VehicleTable.SelectedItem;
            PriceDayBox.Text = currentVehicle.PriceDay.ToString();
            PriceHourBox.Text = currentVehicle.PriceHour.ToString();

            IEnumerable<User> users = ((IEnumerable<User>)UserBox.ItemsSource);
            IEnumerable<Vehicle.LicenseCategories> licenseCategories =
                ((IEnumerable<Vehicle.LicenseCategories>)LicenseCategoryComboBox.ItemsSource);

            IEnumerable<User> singleUser = users.Where((u) => u.Id == currentVehicle.UserId);

            UserBox.SelectedItem = singleUser.Count() > 0 ? users.First() : null;
            LicenseCategoryComboBox.SelectedItem = licenseCategories.Where((lc) => lc == currentVehicle.LicenseCategory).First();
            RevisionDatePicker.SelectedDate = currentVehicle.NextRevision;
            DamagedCheckBox.IsChecked = currentVehicle.Damaged;
            DescriptionBox.Text = currentVehicle.Description;
        }

        private void SaveVehicleEdits_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TypeBox.Text.Trim()) || string.IsNullOrEmpty(PriceDayBox.Text.Trim()) ||
                string.IsNullOrEmpty(PriceHourBox.Text.Trim()) || LicenseCategoryComboBox.SelectedItem == null ||
                RevisionDatePicker.SelectedDate == null || string.IsNullOrEmpty(DescriptionBox.Text.Trim()))
            {
                NotificationLabel.ShowError("All fields are required");
            }
            else
            {
                if (currentVehicle == null)
                {
                    NotificationLabel.ShowError("Please double click on Vehicle first");
                    return;
                }

                try
                {
                    currentVehicle.Type = TypeBox.Text.Trim();
                    currentVehicle.PriceDay = Double.Parse(PriceDayBox.Text);
                    currentVehicle.PriceHour = double.Parse(PriceHourBox.Text);
                    if(UserBox.SelectedItem == null)
                    {
                        currentVehicle.UserId = null;
                    }
                    else if(((User)UserBox.SelectedItem).Id == -1)
                    {
                        currentVehicle.UserId = null;
                    }
                    else
                    {
                        currentVehicle.UserId = ((User)UserBox.SelectedItem).Id;
                    }

                    currentVehicle.LicenseCategory = (Vehicle.LicenseCategories)LicenseCategoryComboBox.SelectedItem;
                    currentVehicle.NextRevision = RevisionDatePicker.SelectedDate.Value;
                    currentVehicle.Damaged = DamagedCheckBox.IsChecked.Value;
                    currentVehicle.Description = DescriptionBox.Text;

                    new VehicleDAO().Update(currentVehicle);

                    VehicleTable.Items.Refresh();
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Could not update vehicle");
                }
            }
        }

        private void AddVehicle_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TypeBox.Text.Trim()) || string.IsNullOrEmpty(PriceDayBox.Text.Trim()) ||
                string.IsNullOrEmpty(PriceHourBox.Text.Trim()) || LicenseCategoryComboBox.SelectedItem == null ||
                RevisionDatePicker.SelectedDate == null || string.IsNullOrEmpty(DescriptionBox.Text.Trim()))
            {
                NotificationLabel.ShowError("All fields are required");
            }
            else
            {

                try
                {
                    Vehicle newVehicle = new Vehicle();

                    newVehicle.Type = TypeBox.Text.Trim();
                    newVehicle.PriceDay = Double.Parse(PriceDayBox.Text);
                    newVehicle.PriceHour = double.Parse(PriceHourBox.Text);
                    if (UserBox.SelectedItem == null)
                    {
                        newVehicle.UserId = null;
                    }
                    else if (((User)UserBox.SelectedItem).Id == -1)
                    {
                        newVehicle.UserId = null;
                    }
                    else
                    {
                        newVehicle.UserId = ((User)UserBox.SelectedItem).Id;
                    }

                    newVehicle.LicenseCategory = (Vehicle.LicenseCategories)LicenseCategoryComboBox.SelectedItem;
                    newVehicle.NextRevision = RevisionDatePicker.SelectedDate.Value;
                    newVehicle.Damaged = DamagedCheckBox.IsChecked.Value;
                    newVehicle.Description = DescriptionBox.Text;

                    new VehicleDAO().Insert(newVehicle);

                    ((List<Vehicle>)VehicleTable.ItemsSource).Add(newVehicle);
                    VehicleTable.Items.Refresh();
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Could not update vehicle");
                }
            }
        }

        private void ClearVehicleFields_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TypeBox.Text = "";
            PriceDayBox.Text = "";
            PriceHourBox.Text = "";
            IEnumerable<User> users = ((IEnumerable<User>)UserBox.ItemsSource);
            IEnumerable<User> singleUser = users.Where((u) => u.Id == -1);
            if(singleUser.Count() > 0)
            {
                UserBox.SelectedItem = singleUser.First();
            }
            RevisionDatePicker.SelectedDate = null;
            DamagedCheckBox.IsChecked = false;
            DescriptionBox.Text = "";
        }

        private void LogoutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserControlManager.Instance.CurrentUserControl = new LoginControl();
        }

        private void SaveSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameBox.Text.Trim())
                || string.IsNullOrEmpty(NameBox.Text.Trim()) || string.IsNullOrEmpty(SurnameBox.Text.Trim())
                || string.IsNullOrEmpty(StreetBox.Text.Trim()) || string.IsNullOrEmpty(ZipCodeBox.Text.Trim())
                || string.IsNullOrEmpty(EmailBox.Text.Trim()) || string.IsNullOrEmpty(CountryBox.Text.Trim())
                || string.IsNullOrEmpty(CityBox.Text.Trim()) || BirthDatePicker.SelectedDate == null)
            {
                NotificationLabel.ShowError("All account information fields required!");
            }
            else
            {
                UserDAO userDAO = new UserDAO();
                UserDetailsDAO userDetailsDAO = new UserDetailsDAO();

                User user = UserManager.CurrentUser;
                user.Username = UsernameBox.Text;
                user.Name = NameBox.Text;
                user.Surname = SurnameBox.Text;

                try
                {
                    UserDetails userDetails = user.UserDetails == null ?
                    user.UserDetails : new UserDetailsDAO().GetUserDetails(userId: user.Id).First();

                    userDetails.City = CityBox.Text;
                    userDetails.Country = CountryBox.Text;
                    userDetails.Email = EmailBox.Text;
                    userDetails.Street = StreetBox.Text;
                    userDetails.ZipCode = ZipCodeBox.Text;
                    userDetails.BirthDate = BirthDatePicker.SelectedDate.Value;

                    userDAO.Update(user);

                    userDetailsDAO.Update(userDetails);

                    NotificationLabel.ShowSuccess("Account Information has been updated !");
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("A problem occured while trying to save settings!");
                }
            }
        }

        private void NumberBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }


        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void ClearFineFields_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FineAmountBox.Text = "";
            DatePicker.SelectedDate = null;
            PaidCheckBox.IsChecked = false;
        }

        private void RefreshCars_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                List<Vehicle> vehicles = new VehicleDAO().GetVehicles();
                VehicleTable.ItemsSource = vehicles;
            }
            catch (Exception ex)
            {
                DebugLog.WriteLine(ex);
                NotificationLabel.ShowError("Could not refresh entries");
            }
        }

        private void RefreshFines_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                List<Damage> damages = new DamageDAO().GetDamages();
                AllDamagesTable.ItemsSource = damages;
            }
            catch (Exception ex)
            {
                DebugLog.WriteLine(ex);
                NotificationLabel.ShowError("Could not refresh entries");
            }
        }

        private void RefreshRentals_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                List<Rent> allRentals = new RentDAO().GetRents();
                
                AllRentalsTable.ItemsSource = allRentals;
            }
            catch (Exception ex)
            {
                NotificationLabel.ShowError("Could not refresh entries");
                DebugLog.WriteLine(ex);            }
        }

        private void IssueInvoice_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(AllRentalsTable.SelectedItem == null)
            {
                NotificationLabel.ShowError("Please first select a rental!");
            }
            else
            {
                try
                {
                    Rent rent = (Rent)AllRentalsTable.SelectedItem;
                    Invoice invoice = new Invoice()
                    {
                        Date = DateTime.Now,
                        TotalAmount = rent.TotalAmount,
                        VatAmount = rent.TotalAmount * Invoice.VAT,
                        UserId = rent.UserId
                    };

                    new InvoiceDAO().Insert(invoice);
                    NotificationLabel.ShowSuccess("Invoice has been stored in database");
                }
                catch(Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Problem occured! Could not issue invoice");
                }
            }
        }

        private void AddToBlackList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AllUsersTable.SelectedItem == null)
            {
                NotificationLabel.ShowError("Please select a User first");
                return;
            }

            User selectedUser = (User)AllUsersTable.SelectedItem;

            try
            {
                List<BlackList> blackLists = new BlackListDAO().GetBlackLists(userId:selectedUser.Id);

                if(blackLists.Count == 0)
                {
                    new BlackListDAO().Insert(new BlackList() { UserId = selectedUser.Id, Warnings = 1 });
                    NotificationLabel.ShowSuccess("User added to blacklist!");
                }
                else
                {
                    string additionalMessage = "";

                    BlackList blackList = blackLists[0];
                    blackList.Warnings++;
                    new BlackListDAO().Update(blackList);

                    if (blackList.Banned)
                    {
                        additionalMessage = " He is now banned";
                    }

                    NotificationLabel.ShowSuccess("User warnings increased to " +blackList.Warnings + " !"  +  additionalMessage);
                }

                
            }
            catch (Exception ex)
            {
                DebugLog.WriteLine(ex);
                NotificationLabel.ShowError("Could not add User to Blacklist");
            }
        }
    }
}
