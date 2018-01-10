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
    public class InvoiceDetailDAO : IDAO
    {
        public void Insert(InvoiceDetail invoiceDetail)
        {
            string sqlCommand = String.Format("INSERT INTO invoice_detail (invoice_id, rents_id)" +
                                          "VALUES ('{0}','{1}'); SELECT last_insert_id()",
                                          invoiceDetail.InvoiceId, invoiceDetail.RentId);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommand, connection);

                if (reader.Read())
                {
                    int id = -1;
                    int.TryParse(reader.GetValue(0).ToString(), out id);
                    invoiceDetail.Id = id;
                }
            }
        }

        public void Delete(params int[] idsToDelete)
        {
            if (idsToDelete.Count() == 0)
            {
                return;
            }

            StringBuilder sqlCommandBuilder = new StringBuilder("DELETE FROM invoice_detail WHERE id IN (");

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

        public void Update(InvoiceDetail invoiceDetail)
        {
            string sqlCommand = String.Format("UPDATE invoice_detail SET invoice_id='{0}', rents_id='{1}' WHERE id={2}",
                                                invoiceDetail.InvoiceId, invoiceDetail.RentId, invoiceDetail.Id);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommand, connection);
            }
        }

        public List<InvoiceDetail> GetInvoiceDetails(int? invoiceId = null, int? rentId = null, int? id = null)
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("SELECT * FROM invoice_detail");

            if (id != null)
            {
                sqlCommandBuilder.Append(" WHERE id=" + id);
            }
            else if (invoiceId != null & rentId != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE invoice_id='{0}' AND rentId='{1}'", invoiceId, rentId));
            }
            else if (invoiceId != null)
            {
                sqlCommandBuilder.Append(" WHERE invoice_id=" + invoiceId);
            }
            else if (rentId != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE rentId='{0}'", rentId));
            }

            List<InvoiceDetail> invoiceDetailList = new List<InvoiceDetail>();

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);

                InvoiceDetail invoiceDetailToAdd = null;

                try
                {
                    while (reader.Read())
                    {
                        invoiceDetailToAdd = new Models.RentInfo.InvoiceDetail()
                        {
                            Id = Convert.ToInt32(reader["id"].ToString()),
                            InvoiceId = Convert.ToInt32(reader["invoice_id"].ToString()),
                            RentId = Convert.ToInt32(reader["rents_id"].ToString()),
                        };
                        invoiceDetailList.Add(invoiceDetailToAdd);
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

            return invoiceDetailList;
        }
    }
}