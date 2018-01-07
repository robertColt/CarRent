using CarRent.Database;
using CarRent.Models.RentInfo;
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
    /// Interaction logic for CurrentUserControl.xaml
    /// </summary>
    public partial class CurrentUserControl : UserControl
    {
        public CurrentUserControl()
        {
            InitializeComponent();

            List<Rent> myRentals = new RentDAO().GetRents(userId: UserManager.CurrentUser.Id);

            GridView gridView = new GridView();
            
            foreach (Rent rent in myRentals)
            {
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = property.Name,
                    DisplayMemberBinding = new Binding(property.Name),
                });
            }
        }
    }
}
