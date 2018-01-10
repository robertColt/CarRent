using CarRent.Helper;
using CarRent.Models.RentInfo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;

namespace CarRent.Database
{
    public class InvoiceDAO : IDAO
    {
        public void Insert(Invoice invoice)
        {
            string sqlCommand = String.Format("INSERT INTO invoice (date, total_amount, vat_amount, user_id)" +
                                          "VALUES ('{0}','{1}','{2}','{3}'); SELECT last_insert_id()",
                                          invoice.Date.ToString(), invoice.TotalAmount, invoice.VatAmount, invoice.UserId);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommand, connection);

                if (reader.Read())
                {
                    int id = -1;
                    int.TryParse(reader.GetValue(0).ToString(), out id);
                    invoice.Id = id;
                }
            }
        }

        public void Delete(params int[] idsToDelete)
        {
            if (idsToDelete.Count() == 0)
            {
                return;
            }

            StringBuilder sqlCommandBuilder = new StringBuilder("DELETE FROM invoice WHERE id IN (");

            for (int i = 0; i < idsToDelete.Count(); i++)
            {
                sqlCommandBuilder.Append(idsToDelete[i]);
                if (i != idsToDelete.Count() - 1)
                {
                    sqlCommandBuilder.Append(", ");
                }
                else
                {
                    sqlCommandBuilder.Append(")");
                }
            }

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);
            }
        }

        public void Update(Invoice invoice)
        {
            string sqlCommand = String.Format("UPDATE invoice SET date='{0}', total_amount='{1}', vat_amount='{2}', user_id='{3}' WHERE id={4}",
                                                invoice.Date, invoice.TotalAmount, invoice.VatAmount, invoice.UserId, invoice.Id);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommand, connection);
            }
        }

        public List<Invoice> GetInvoices(int? userId = null, DateTime? date = null, int? id = null )
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("SELECT * FROM invoice");

            if(id != null)
            {
                sqlCommandBuilder.Append(" WHERE id="+id);
            }
            else if (userId != null & date != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE user_id='{0}' AND date LIKE '%{1}%'", userId, date.Value.ToShortDateString()));
            }
            else if (userId != null)
            {
                sqlCommandBuilder.Append(" WHERE user_id=" + userId);
            }
            else if (date != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE date LIKE '%{0}%'", date.Value.ToShortDateString()));
            }

            List<Invoice> invoiceList = new List<Invoice>();

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);

                Invoice invoiceToAdd = null;

                try
                {
                    while (reader.Read())
                    {
                        invoiceToAdd = new Models.RentInfo.Invoice()
                        {
                            Id = Convert.ToInt32(reader["id"].ToString()),
                            Date = Convert.ToDateTime(reader["date"].ToString()),
                            TotalAmount = Convert.ToDouble(reader["total_amount"].ToString()),
                            VatAmount = Convert.ToDouble(reader["vat_amount"].ToString()),
                            UserId = Convert.ToInt32(reader["user_id"].ToString()),
                        };
                        SetRents(invoiceToAdd);
                        invoiceList.Add(invoiceToAdd);
                    }
                }
                catch (FormatException ex)
                {
                    DebugLog.WriteLine(ex);
                }
                finally
                {
                    reader.Close();
                }
            }

            return invoiceList;
        }

        private void SetRents(Invoice invoice)
        {
            List<InvoiceDetail> invoiceDetails = new InvoiceDetailDAO().GetInvoiceDetails(invoiceId: invoice.Id);
            RentDAO rentDAO = new RentDAO();

            foreach(InvoiceDetail invoiceDetail in invoiceDetails)
            {
                try
                {
                    invoice.AddRent(rentDAO.GetRents(id: invoiceDetail.RentId).First());
                }
                catch (InvalidOperationException ex)
                {
                    DebugLog.WriteLine(ex);
                }
            }
        }
    }
}