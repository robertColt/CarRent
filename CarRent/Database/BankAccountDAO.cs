using CarRent.Helper;
using CarRent.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CarRent.Database
{
    public class BankAccountDAO : IDAO
    {
        public void Insert(BankAccount bankAccount)
        {
            string sqlCommand = String.Format("INSERT INTO public.bank_account (security_number, card_type, bank_name, user_id, iban, expiry_date)" +
                                          "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}') RETURNING id",
                                          bankAccount.SecurityNumber, bankAccount.CardType, bankAccount.BankName, bankAccount.UserId,
                                          bankAccount.Iban, bankAccount.ExpiryDate.ToShortDateString());

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommand, connection);

                if (reader.Read())
                {
                    int id = -1;
                    int.TryParse(reader.GetValue(0).ToString(), out id);
                    bankAccount.Id = id;
                }
            }
        }

        public void Delete(params int[] idsToDelete)
        {
            if (idsToDelete.Count() == 0)
            {
                return;
            }

            StringBuilder sqlCommandBuilder = new StringBuilder("DELETE FROM public.bank_account WHERE id IN (");

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

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);
            }
        }

        public void Update(BankAccount bankAccount)
        {
            string sqlCommand = String.Format("UPDATE public.bank_account SET security_number='{0}', card_type='{1}', bank_name='{2}',"+
                                                " user_id='{3}', iban='{4}', expiry_date='{5}' WHERE id='{6}'",
                                               bankAccount.SecurityNumber, bankAccount.CardType, bankAccount.BankName, bankAccount.UserId,
                                               bankAccount.Iban, bankAccount.ExpiryDate.ToShortDateString(), bankAccount.Id);

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommand, connection);
            }
        }

        public List<BankAccount> GetBankAccounts(int? userId = null, int? id = null)
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("SELECT * FROM public.bank_account");

            if (id != null)
            {
                sqlCommandBuilder.Append(" WHERE id=" + id);
            }
            else if (userId != null)
            {
                sqlCommandBuilder.Append(" WHERE user_id=" + userId);
            }

            List<BankAccount> bankAccountList = new List<BankAccount>();

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);

                BankAccount bankAccountToAdd = null;

                try
                {
                    while (reader.Read())
                    {
                        bankAccountToAdd = new BankAccount()
                        {
                            Id = Convert.ToInt32(reader["id"].ToString()),
                            SecurityNumber = Convert.ToInt32(reader["security_number"].ToString()),
                            CardType = reader["card_type"].ToString(),
                            BankName = reader["bank_name"].ToString(),
                            Iban = reader["iban"].ToString(),
                            ExpiryDate = Convert.ToDateTime(reader["expiry_date"].ToString()),
                            UserId = Convert.ToInt32(reader["user_id"].ToString()),
                        };

                        bankAccountList.Add(bankAccountToAdd);
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

            return bankAccountList;
        }
    }
}