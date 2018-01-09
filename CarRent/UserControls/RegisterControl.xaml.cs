using System.Windows;
using System.Windows.Controls;
using CarRent.ExtensionMethods;
using System;
using CarRent.Models;
using CarRent.Helper;
using CarRent.Database;
using CarRent.ViewModel;
using System.Windows.Media;

namespace CarRent.UserControls
{
    /// <summary>
    /// Interaction logic for RegisterControl.xaml
    /// </summary>
    public partial class RegisterControl : UserControl
    {
        private Label AccessKeyLabel = new Label() { Content = "Access Key(*)" , Foreground = Brushes.DarkRed};
        private PasswordBox AccessKeyBox = new PasswordBox() { Margin = new Thickness() { Top = 9, Bottom = 8 } };

        //access key = 123456 in plaintext
        private string AccessKey = "UYB1a1HDztgJG4PkThu8by5lmEDPsl3ZRdKsvLSJyZk/lGZwLNat5aeT3JNNoalOr2DOrcIOmXbZF6EDiQzp5w==";

        private User.Function selectedFunction;

        public RegisterControl()
        {
            InitializeComponent();
            //BirthDatePicker.SelectedDate = DateTime.Now;
            FunctionComboBox.ItemsSource = Enum.GetValues(typeof(User.Function));
            FunctionComboBox.SelectedItem = FunctionComboBox.Items[0];
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(UsernameBox.Text.Trim()) || string.IsNullOrEmpty(PasswordBox.Password)
                || string.IsNullOrEmpty(NameBox.Text.Trim()) || string.IsNullOrEmpty(SurnameBox.Text.Trim())
                || string.IsNullOrEmpty(StreetBox.Text.Trim()) || string.IsNullOrEmpty(ZipCodeBox.Text.Trim())
                || string.IsNullOrEmpty(EmailBox.Text.Trim()) || string.IsNullOrEmpty(CountryBox.Text.Trim())
                || string.IsNullOrEmpty(CityBox.Text.Trim()) || BirthDatePicker.SelectedDate == null || 
                FunctionComboBox.SelectedItem == null)
            {
                NotificationLabel.ShowError("All fields required!");
            }
            else
            {
                if(selectedFunction == User.Function.ADMIN || selectedFunction == User.Function.OPERATOR 
                    && string.IsNullOrEmpty(AccessKeyBox.Password))
                {
                    NotificationLabel.ShowError("Please enter access key to register as " + selectedFunction + " !");
                    return;
                }

                if (! Encryption.GenerateSaltedHash(AccessKeyBox.Password).Equals(AccessKey))
                {
                    NotificationLabel.ShowError("Access key incorrect");
                    return;
                }

                User user = new User()
                {
                    Username = UsernameBox.Text,
                    Password = Encryption.GenerateSaltedHash(PasswordBox.Password),
                    Name = NameBox.Text,
                    Surname = SurnameBox.Text,
                    UserFunction = selectedFunction,
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

        private void FunctionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedFunction = (User.Function)FunctionComboBox.SelectedItem;

            if (selectedFunction == User.Function.ADMIN || selectedFunction == User.Function.OPERATOR)
            {
                if( !LabelStackPanel.Children.Contains(AccessKeyLabel))
                {
                    LabelStackPanel.Children.Add(AccessKeyLabel);
                }
                if ( !TextBoxStackPanel.Children.Contains(AccessKeyBox))
                {
                    TextBoxStackPanel.Children.Add(AccessKeyBox);
                }
            }
            else
            {
                if (LabelStackPanel.Children.Contains(AccessKeyLabel))
                {
                    LabelStackPanel.Children.Remove(AccessKeyLabel);
                }
                if (TextBoxStackPanel.Children.Contains(AccessKeyBox))
                {
                    TextBoxStackPanel.Children.Remove(AccessKeyBox);
                }
            }
        }
    }
}
