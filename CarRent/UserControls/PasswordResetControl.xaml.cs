using CarRent.Database;
using CarRent.ExtensionMethods;
using CarRent.Helper;
using CarRent.Models;
using CarRent.ViewModel;
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

namespace CarRent.UserControls
{
    /// <summary>
    /// Interaction logic for PasswordResetControl.xaml
    /// </summary>
    public partial class PasswordResetControl : UserControl
    {
        public PasswordResetControl()
        {
            InitializeComponent();
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            UserControlManager.Instance.CurrentUserControl = new LoginControl();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameBox.Text.Trim()))
            {
                NotificationLabel.ShowError("Please fill in username !");
                UsernameBox.Focus();
            }
            else if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                NotificationLabel.ShowError("Please type in new Password !");
                PasswordBox.Focus();
            }
            else if(string.IsNullOrEmpty(RepeatPasswordBox.Password) ||
                !RepeatPasswordBox.Password.Equals(PasswordBox.Password))
            {
                NotificationLabel.ShowError("Please Confirm new Password");
                RepeatPasswordBox.Focus();
            }
            else
            {
                UserDAO dao = new UserDAO();
                try
                {
                    User user = dao.GetUsers(username: UsernameBox.Text.Trim()).First();
                    user.Password = PasswordBox.Password;
                    dao.Update(user);
                    NotificationLabel.ShowSuccess("Password changed successfully");
                }
                catch(System.InvalidOperationException ie)
                {
                    DebugLog.WriteLine(ie);
                    NotificationLabel.ShowError("Username does not exist !");
                }
                catch(Exception ex)
                {
                    DebugLog.WriteLine(ex);
                    NotificationLabel.ShowError("Could not update user !");
                }    
                
            }

        }
    }
}
