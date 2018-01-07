using CarRent.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CarRent.ViewModel
{
    public class UserControlManager : INotifyPropertyChanged
    {
        private static UserControlManager instance;

        public static UserControlManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new UserControlManager();
                    instance.CurrentUserControl = new CurrentUserControl();
                }
                return instance;
            }
            private set { }
        }

        private UserControl currentUserControl;

        private UserControlManager() { }

        public UserControl CurrentUserControl
        {
            get { return currentUserControl; }
            set
            {
                currentUserControl = value;
                OnPropertyChanged("CurrentUserControl");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
