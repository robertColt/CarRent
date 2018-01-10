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
using System.Threading;
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

            UserLabel.Content = "Welcome User " + UserManager.CurrentUser.Name + " "
                + UserManager.CurrentUser.Surname;

            
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
                        NotificationLabel.ShowSuccess("Payment successful !");
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLine(ex);
                        NotificationLabel.ShowError("Could not process paying");
                        selectedRent.Paid = false;
                    }
                }
            }
            else
            {
                NotificationLabel.ShowError("Nothing selected");
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
                        List<BankAccount> list = new BankAccountDAO().GetBankAccounts(userId: UserManager.CurrentUser.Id);
                        if (list.Count == 0)
                        {
                            NotificationLabel.ShowError("Please go to settings and add your bank account info");
                            return;
                        }
                        //todo retrieve bankaccount 
                        selectedDamage.Paid = true;
                        new DamageDAO().Update(selectedDamage);
                        MyFinesTable.Items.Refresh();
                        NotificationLabel.ShowSuccess("Payment successful !");
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLine(ex);
                        NotificationLabel.ShowError("Could not process paying");
                        selectedDamage.Paid = false;
                        MyFinesTable.Items.Refresh();
                    }
                }
            }
            else
            {
                NotificationLabel.ShowError("Nothing selected");
            }
        }

        private void RentCar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Vehicle selectedVehicle = VehicleTable.SelectedItem != null ? VehicleTable.SelectedItem as Vehicle : null;

            if (selectedVehicle != null)
            {
                //todo implement pay logic
                
                try
                {
                    List<BlackList> blackLists = new BlackListDAO().GetBlackLists(userId: UserManager.CurrentUser.Id);

                    if(blackLists.Count != 0)
                    {
                        if (blackLists.First().Banned)
                        {
                            NotificationLabel.ShowError("You are banned! You cannot make a rental!");
                        }
                        
                    }

                    List<BankAccount> list = new BankAccountDAO().GetBankAccounts(userId: UserManager.CurrentUser.Id);

                    if(list.Count == 0)
                    {
                        NotificationLabel.ShowError("Please go to settings and add your bank account info");
                        return;
                    }

                    new RentDAO().Insert(new Rent()
                    {
                       BeginTime = DateTime.Now,
                       Returned = false,
                       Paid = false,
                       VehicleId = selectedVehicle.Id,
                       VehicleName = selectedVehicle.Name,
                       UserId = UserManager.CurrentUser.Id 
                    });

                    selectedVehicle.UserId = UserManager.CurrentUser.Id;
                    new VehicleDAO().Update(selectedVehicle);
                    ((List<Vehicle>)VehicleTable.ItemsSource).Remove((Vehicle)VehicleTable.SelectedItem);
                    MyFinesTable.Items.Refresh();
                    NotificationLabel.ShowSuccess("Rental successful !");
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Could not process rental");
                    selectedVehicle.UserId = null;
                }
            }
            else
            {
                NotificationLabel.ShowError("Nothing selected");
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl)) return;

            NotificationLabel.Content = "";

            if (MyRentalsTab.IsSelected)
            {
                List<Rent> myRentals = new RentDAO().GetRents(userId: UserManager.CurrentUser.Id);

                if (myRentals.Count == 0)
                {
                    NotificationLabel.ShowError("You don't have any rentals");
                }
                MyRentalsTable.ItemsSource = myRentals;
            }
            else if (MyFinesTab.IsSelected)
            {
                List<Damage> myDamages = new DamageDAO().GetDamages(userId: UserManager.CurrentUser.Id);

                if (myDamages.Count == 0)
                {
                    NotificationLabel.ShowSuccess("You don't have any fines to pay");
                }
                MyFinesTable.ItemsSource = myDamages;
            }
            else if (RentACarTab.IsSelected)
            {
                IEnumerable<Vehicle> myVehicles = new VehicleDAO().GetVehicles().Where((v) => v.UserId == null);

                if (myVehicles != null & myVehicles.Count() == 0)
                {
                    NotificationLabel.ShowError("There are no more vehicles available");
                }
                VehicleTable.ItemsSource = myVehicles.ToList<Vehicle>();
            }
            else if (RentACarTab.IsSelected)
            {
                IEnumerable<Vehicle> myVehicles = new VehicleDAO().GetVehicles().Where((v) => v.UserId == 0);

                if (myVehicles != null & myVehicles.Count() == 0)
                {
                    NotificationLabel.ShowError("There are no more vehicles available");
                }
                VehicleTable.ItemsSource = myVehicles.ToList<Vehicle>();
            }
            else if (SettingsTab.IsSelected)
            {
                CardTypeComboBox.ItemsSource = Enum.GetValues(typeof(BankAccount.CardTypes));

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

                try
                {
                    BankAccount bankAccount = new BankAccountDAO().GetBankAccounts(userId: currentUser.Id).First();

                    IBANBox.Text = Encryption.Decrypt(bankAccount.Iban);
                    SecurityNumberBox.Text = bankAccount.SecurityNumber.ToString();
                    CardTypeComboBox.SelectedItem =
                        bankAccount.CardType == BankAccount.CardTypes.CREDIT ?
                        CardTypeComboBox.Items.GetItemAt(0) : CardTypeComboBox.Items.GetItemAt(1);
                    BankNameBox.Text = bankAccount.BankName;
                    ExpiryDatePicker.SelectedDate = bankAccount.ExpiryDate;
                }
                catch (InvalidOperationException ex)
                {
                    return;//no bankaccount for this user
                }
                catch (Exception ex)
                {
                    NotificationLabel.ShowError("Could not retrieve bank account information");
                }
            }
        }

        private void SecurityNumberBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void SaveSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!SaveAccountDetails())
            {
                return;
            }
            else
            {
                SaveBankAccountDetails();
            }
            NotificationLabel.ShowSuccess("Saved successfuly!");
        }

        private bool SaveAccountDetails()
        {
            if (string.IsNullOrEmpty(UsernameBox.Text.Trim())
                || string.IsNullOrEmpty(NameBox.Text.Trim()) || string.IsNullOrEmpty(SurnameBox.Text.Trim())
                || string.IsNullOrEmpty(StreetBox.Text.Trim()) || string.IsNullOrEmpty(ZipCodeBox.Text.Trim())
                || string.IsNullOrEmpty(EmailBox.Text.Trim()) || string.IsNullOrEmpty(CountryBox.Text.Trim())
                || string.IsNullOrEmpty(CityBox.Text.Trim()) || BirthDatePicker.SelectedDate == null)
            {
                NotificationLabel.ShowError("All account information fields required!");
                return false;
            }
            else
            {
                UserDAO userDAO = new UserDAO();
                UserDetailsDAO userDetailsDAO = new UserDetailsDAO();

                User user = UserManager.CurrentUser;
                user.Username = UsernameBox.Text;
                user.Name = NameBox.Text;
                user.Surname = SurnameBox.Text;

                DebugLog.WriteLine(userDAO == null);

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

                    UserLabel.Content = "Welcome User " + UserManager.CurrentUser.Name + " "
                + UserManager.CurrentUser.Surname;

                    return true;
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("A problem occured while trying to save settings!");
                    return false; 
                }
            }
        }

        private void SaveBankAccountDetails()
        {
            if (string.IsNullOrEmpty(IBANBox.Text.Trim()) && string.IsNullOrEmpty(SecurityNumberBox.Text.Trim())
                && CardTypeComboBox.SelectedItem == null && string.IsNullOrEmpty(BankNameBox.Text.Trim())
                && ExpiryDatePicker.SelectedDate== null)
            {
                return;
            }
            else if (!string.IsNullOrEmpty(IBANBox.Text.Trim()) && !string.IsNullOrEmpty(SecurityNumberBox.Text.Trim())
                && CardTypeComboBox.SelectedItem != null && !string.IsNullOrEmpty(BankNameBox.Text.Trim())
                && ExpiryDatePicker.SelectedDate != null)
            {
                try
                {
                    List<BankAccount> userBankAccounts = 
                        new BankAccountDAO().GetBankAccounts(userId: UserManager.CurrentUser.Id);
                    BankAccount account = new BankAccount()
                        {
                            Iban = Encryption.Encrypt(IBANBox.Text.Trim()),
                            SecurityNumber = Int32.Parse(SecurityNumberBox.Text.Trim()),
                            CardType = (BankAccount.CardTypes)CardTypeComboBox.SelectedItem,
                            BankName = BankNameBox.Text.Trim(),
                            ExpiryDate = ExpiryDatePicker.SelectedDate.Value,
                            UserId = UserManager.CurrentUser.Id 
                        };

                    if(userBankAccounts.Count > 0)
                    {
                        account.Id = userBankAccounts.First().Id;
                        new BankAccountDAO().Update(account);
                    }
                    else
                    {
                        new BankAccountDAO().Insert(account);
                    }
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("A problem occured while trying to save settings!");
                }
            }
            else
            {
                NotificationLabel.ShowError("All bank account information fields are mandatory");
            }
        }

        private void LogoutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserControlManager.Instance.CurrentUserControl = new LoginControl();
        }
    }
}
