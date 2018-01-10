using CarRent.Database;
using CarRent.Helper;
using CarRent.ExtensionMethods;
using CarRent.Models;
using CarRent.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarRent.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameBox.Text.Trim()))
            {
                NotificationLabel.ShowError("Please fill in username !");
            }
            else if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                NotificationLabel.ShowError("Please fill in password !");
            }
            else
            {
                //check if username exists
                try
                {
                    UserDAO dao = new UserDAO();
                    //throws exception if user non existent
                    User currentUser = dao.GetUsers(username: UsernameBox.Text.Trim()).First();

                    string hash = Encryption.GenerateSaltedHash(PasswordBox.Password);

                    if (hash.Equals(currentUser.Password))
                    {
                        NotificationLabel.ShowSuccess("Authentication succeeded !");

                        UserManager.CurrentUser = currentUser;

                        switch (currentUser.UserFunction)
                        {
                            case User.Function.ADMIN:
                            {
                                UserControlManager.Instance.CurrentUserControl = new AdminControl();
                            }
                            break;

                            case User.Function.USER:
                            {
                                UserControlManager.Instance.CurrentUserControl = new MyUserControl();
                            }
                                break;
                            case User.Function.OPERATOR:
                                {
                                    UserControlManager.Instance.CurrentUserControl = new OperatorControl();
                                }
                            break;
                            //todo add function operator
                        }
                        
                    }
                    else
                    {
                        PasswordBox.Focus();
                        NotificationLabel.ShowError("Password invalid !");
                        PasswordBox.Focus();
                    }
                }
                catch(InvalidOperationException ie)
                {
                    DebugLog.WriteLine(ie);
                    NotificationLabel.ShowError("Username doesen't exist !");
                    UsernameBox.Focus();
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Sorry, something went wrong !");
                    UsernameBox.Focus();
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            UserControlManager.Instance.CurrentUserControl = new RegisterControl();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameBox.Focus();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            UserControlManager.Instance.CurrentUserControl = new PasswordResetControl();
        }
    }
}
