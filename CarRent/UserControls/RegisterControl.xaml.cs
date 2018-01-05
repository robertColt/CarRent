using System.Windows;
using System.Windows.Controls;
using CarRent.ExtensionMethods;
using System;
using CarRent.Models;
using CarRent.Helper;
using CarRent.Database;
using CarRent.ViewModel;

namespace CarRent.UserControls
{
    /// <summary>
    /// Interaction logic for RegisterControl.xaml
    /// </summary>
    public partial class RegisterControl : UserControl
    {
        public RegisterControl()
        {
            InitializeComponent();
            //BirthDatePicker.SelectedDate = DateTime.Now;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(UsernameBox.Text.Trim()) || string.IsNullOrEmpty(PasswordBox.Password)
                || string.IsNullOrEmpty(NameBox.Text.Trim()) || string.IsNullOrEmpty(SurnameBox.Text.Trim())
                || string.IsNullOrEmpty(StreetBox.Text.Trim()) || string.IsNullOrEmpty(ZipCodeBox.Text.Trim())
                || string.IsNullOrEmpty(EmailBox.Text.Trim()) || string.IsNullOrEmpty(CountryBox.Text.Trim())
                || string.IsNullOrEmpty(CityBox.Text.Trim()) || BirthDatePicker.SelectedDate == null)
            {
                NotificationLabel.ShowError("All fields required!");
            }
            else
            {
                User user = new User()
                {
                    Username = UsernameBox.Text,
                    Password = Encryption.GenerateSaltedHash(PasswordBox.Password),
                    Name = NameBox.Text,
                    Surname = SurnameBox.Text,
                    UserFunction = User.Function.USER,
                };

                UserDetails userDetails = new UserDetails()
                {
                    City = CityBox.Text,
                    Country = CountryBox.Text,
                    Email = EmailBox.Text,
                    Street = StreetBox.Text,
                    ZipCode = ZipCodeBox.Text,
                    BirthDate = BirthDatePicker.SelectedDate.Value,
                };

                UserDAO userDAO = new UserDAO();
                UserDetailsDAO userDetailsDAO = new UserDetailsDAO();

                DebugLog.WriteLine(userDAO == null);

                try
                {
                    userDAO.Insert(user);
                    userDetails.UserId = user.Id;

                    userDetailsDAO.Insert(userDetails);

                    user.UserDetails = userDetails;

                    userDAO.Update(user);

                    NotificationLabel.ShowSuccess("Registration successful!");
                }
                catch(Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Username already taken!");
                }
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            UserControlManager.Instance.CurrentUserControl = new LoginControl();
        }
    }
}
