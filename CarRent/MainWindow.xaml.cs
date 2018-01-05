using CarRent.Database;
using CarRent.Helper;
using CarRent.Models;
using CarRent.Models.RentInfo;
using CarRent.UserControls;
using CarRent.ViewModel;
using Microsoft.Win32;
using Npgsql;
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

namespace CarRent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //      
            //          VEHICLE
            //
            //Vehicle vehicle = new Vehicle()
            //{
            //    Id = 30,
            //    Description = "asx",
            //    NextRevision = DateTime.UtcNow,
            //    LicenseCategory = "B",
            //    PriceDay = 20,
            //    PriceHour = 40,
            //    Type = "newType",
            //    Damaged = false,
            //    UserId = 5
            //};

            //VehicleDAO dao = new VehicleDAO();
            //dao.Insert(vehicle);
            //List<Vehicle> vehicleList = dao.GetVehicles(damaged: true);

            //foreach (Vehicle oneVehicle in vehicleList)
            //{
            //    DebugLog.WriteLine(oneVehicle.Id + "~~~~~~~~~~~" + oneVehicle.Damaged);
            //}
            //DebugLog.WriteLine(vehicle.Id.ToString());
            //dao.Update(vehicle);
            //dao.Delete(new int[] { 25, 26 });



            //
            //           RENT
            //
            //Rent rent = new Rent()
            //{
            //    Id = 10,
            //    BeginTime = DateTime.Now,
            //    EndTime = DateTime.Now,
            //    Returned = true,
            //    Discount = 25,
            //    TotalAmount = 300,
            //    Paid = false,
            //    VehicleId = 29,
            //    UserId = 5
            //};

            //RentDAO rentDAO = new RentDAO();
            //rentDAO.Delete(8,9);
            //rentDAO.Insert(rent);
            //rentDAO.Insert(rent);
            //rentDAO.Update(rent);

            //List<Rent> rentsList = rentDAO.GetRents(userId : 5, vehicleId : 29);

            //foreach (Rent element in rentsList)
            //{
            //    Debug.WriteLine(element.Id + "~~~~~~~~~~~" + element.BeginTime + " - " + element.EndTime);
            //}


            //
            //    Invoice
            //
            //Invoice invoice = new Invoice()
            //{
            //    Id = 1,
            //    Date = DateTime.Now,
            //    TotalAmount = 22222,
            //    VatAmount = 23.4,
            //    UserId = 23
            //};

            //InvoiceDAO dao = new InvoiceDAO();
            ////dao.Insert(invoice);
            //List<Invoice> invoiceList = dao.GetInvoices(date : DateTime.Parse("12/29/2017"));

            //foreach (Invoice oneInvoice in invoiceList)
            //{
            //    DebugLog.WriteLine(oneInvoice.Id + "~~~~~~~~~~~" + oneInvoice.VatAmount);
            //}
            //DebugLog.WriteLine(invoice.Id.ToString());
            //dao.Update(invoice);
            //dao.Delete(new int[] { 3 });



            //
            //    InvoiceDetails
            //
            //InvoiceDetail entry = new InvoiceDetail()
            //{
            //    InvoiceId = 1,
            //    RentId = 10
            //};

            //InvoiceDetailDAO dao = new InvoiceDetailDAO();
            //dao.Insert(entry);
            //List<InvoiceDetail> list = dao.GetInvoiceDetails();

            //foreach (InvoiceDetail element in list)
            //{
            //    DebugLog.WriteLine(entry.Id + " =?= " + element.Id + "~~~~~~~~~~~" + element.InvoiceId);
            //}
            //DebugLog.WriteLine(entry.Id.ToString());
            //entry.InvoiceId = 2;
            //dao.Update(entry);
            //DebugLog.WriteLine(entry.Id.ToString());
            //dao.Delete(new int[] { 5, 6 });


            //
            //    Damage
            //
            //Damage entry = new Damage()
            //{
            //    FineAmount = 234,
            //    Date = DateTime.Now,
            //    Paid = true,
            //    VehicleId = 28,
            //    UserId = 5
            //};

            //DamageDAO dao = new DamageDAO();
            //dao.Insert(entry);
            //dao.Insert(entry);
            //dao.Insert(entry);

            //List<Damage> list = dao.GetDamages(paid: false);

            //foreach (Damage element in list)
            //{
            //    DebugLog.WriteLine(entry.Id + " =?= " + element.Id + "~~~~~~~~~~~" + element.VehicleId);
            //}
            //DebugLog.WriteLine(entry.Id.ToString());
            //entry.VehicleId = 22;
            //entry.Id = 2;
            //dao.Update(entry);
            //DebugLog.WriteLine(entry.Id.ToString());
            //dao.Delete(new int[] { });



            //
            //    User
            //
            //User entry = new User()
            //{
            //    Name = "rob",
            //    Surname = "bob",
            //    Password = "a;smx",
            //    Username = "rooter",
            //    Function = "Function",
            //};

            //UserDAO dao = new UserDAO();
            ////dao.Insert(entry);

            //List<User> list = dao.GetUsers();

            //foreach (User element in list)
            //{
            //    DebugLog.WriteLine(entry.Id + " =?= " + element.Id + "~~~~~~~~~~~" + element.Username);
            //}

            //DebugLog.WriteLine(entry.Id.ToString());
            ////entry.Surname = "bobe";
            ////dao.Update(entry);
            //dao.Delete(new int[] { 3 });

            //
            //    User
            //
            //User entry = new User()
            //{
            //    Name = "rob",
            //    Surname = "bob",
            //    Password = "a;smx",
            //    Username = "root",
            //    Function = "Function",
            //};

            //UserDAO dao = new UserDAO();
            //dao.Insert(entry);

            //List<User> list = dao.GetUsers();

            //foreach (User element in list)
            //{
            //    DebugLog.WriteLine(entry.Id + " =?= " + element.Id + "~~~~~~~~~~~" + element.Username);
            //}

            //DebugLog.WriteLine(entry.Id.ToString());
            ////entry.Surname = "bobe";
            ////dao.Update(entry);
            //dao.Delete(new int[] { 3 });

            //
            //    UserDetails
            //
            //UserDetails entry = new UserDetails()
            //{
            //    Email = "kjansdc@kdsc",
            //    BirthDate = DateTime.Now,
            //    City = "Brasov",
            //    ZipCode = "lasx23124",
            //    Country = "romania",
            //    Premium = false,
            //    Street = "grivitei",
            //    UserId = 1
            //};

            //UserDetailsDAO dao = new UserDetailsDAO();
            //dao.Insert(entry);

            //List<UserDetails> list = dao.GetUserDetails();

            //foreach (UserDetails element in list)
            //{
            //    DebugLog.WriteLine(entry.Id + " =?= " + element.Id + "~~~~~~~~~~~" + element.Email);
            //}

            //DebugLog.WriteLine(entry.Id.ToString());

            ////dao.Update(entry);
            //dao.Delete(new int[] {  });


            //
            //    BankAccount
            //
            //BankAccount entry = new BankAccount()
            //{
            //    BankName = "Unicredit",
            //    CardType = "Debit",
            //    ExpiryDate = new DateTime(2020, 11, 1),
            //    Iban = "msdcsd",
            //    SecurityNumber = 234,
            //    UserId = 1
            //};

            //BankAccountDAO dao = new BankAccountDAO();
            //dao.Insert(entry);

            //List<BankAccount> list = dao.GetBankAccounts();

            //foreach (BankAccount element in list)
            //{
            //    DebugLog.WriteLine(entry.Id + " =?= " + element.Id + "~~~~~~~~~~~" + element.Iban);
            //}

            //DebugLog.WriteLine(entry.Id.ToString());
            //entry.Iban = "ascadjhbj";
            //dao.Update(entry);
            //dao.Delete(new int[] { 2 });

            //
            //    BankAccount
            //
            //BlackList entry = new BlackList()
            //{
            //    Warnings = 1,
            //    UserId = 2
            //};

            //BlackListDAO dao = new BlackListDAO();
            //dao.Insert(entry);

            //List<BlackList> list = dao.GetBlackLists();

            //foreach (BlackList element in list)
            //{
            //    DebugLog.WriteLine(entry.Id + " =?= " + element.Id + "~~~~~~~~~~~" + element.Warnings);
            //}

            //DebugLog.WriteLine(entry.Id.ToString());
            //entry.Warnings = 3;
            //dao.Update(entry);
            //dao.Delete(new int[] { 2 });

            //ContentController.Content = new LoginControl();
            //this.Height = ((UserControl)ContentController.Content).Height;
            //this.Width = ((UserControl)ContentController.Content).Width;

            DebugLog.WriteLine(this.Height + " " + this.Width);

            Binding myBinding = new Binding();
            myBinding.Source = UserControlManager.Instance;
            myBinding.Path = new PropertyPath("CurrentUserControl");
            myBinding.Mode = BindingMode.TwoWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(ContentController, ContentControl.ContentProperty, myBinding);
        }
    }
}
