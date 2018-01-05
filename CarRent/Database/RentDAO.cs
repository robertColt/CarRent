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
    public class RentDAO : IDAO
    {
        public void Insert(Rent rent)
        {
            string sqlCommand = String.Format("INSERT INTO rent (begin_time, end_time, returned, discount, total_amount, paid, vehicle_id, user_id)" +
                                          "VALUES ('{0}','{1}',b'{2}','{3}','{4}',b'{5}','{6}','{7}'); SELECT last_insert_id()",
                                          rent.BeginTime.ToString(), rent.EndTime != null ? rent.EndTime.Value.ToString() : "", rent.Returned ? "1" : "0",
                                          rent.Discount, rent.TotalAmount, rent.Paid ? "1" : "0", rent.VehicleId, rent.UserId);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommand, connection);

                if (reader.Read())
                {
                    int id = -1;
                    int.TryParse(reader.GetValue(0).ToString(), out id);
                    rent.Id = id;
                }
            }
        }

        public void Delete(params int[] idsToDelete)
        {
            if(idsToDelete.Count() == 0)
            {
                return;
            }

            StringBuilder sqlCommandBuilder = new StringBuilder("DELETE FROM rent WHERE id IN (");

            for (int i = 0; i < idsToDelete.Count(); i++)
            {
                sqlCommandBuilder.Append(idsToDelete[i]);
                if (i != idsToDelete.Count() - 1)
                {
                    sqlCommandBuilder.Append(", ");
                }else
                {
                    sqlCommandBuilder.Append(")");
                }
            }

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);
            }
        }

        public void Update(Rent rent)
        {
            string sqlCommand = String.Format("UPDATE rent SET begin_time='{0}', end_time='{1}', returned=b'{2}', discount='{3}'," +
                                                "total_amount='{4}', paid=b'{5}', vehicle_id='{6}', user_id='{7}' WHERE id={8}",
                                                rent.BeginTime.ToString(), rent.EndTime != null ? rent.EndTime.Value.ToString() : "", rent.Returned ? "1" : "0",
                                          rent.Discount, rent.TotalAmount, rent.Paid ? "1" : "0", rent.VehicleId, rent.UserId, rent.Id);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommand, connection);
            }
        }

        public List<Rent> GetRents(int? userId = null, int? vehicleId = null, int? id = null)
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("SELECT * FROM rent");

            if(id != null)
            {
                sqlCommandBuilder.Append("WHERE id=" + id);
            }
            else if (userId != null & vehicleId != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE user_id='{0}' AND vehicle_id='{1}'", userId, vehicleId));
            }
            else if (userId != null)
            {
                sqlCommandBuilder.Append(" WHERE user_id=" + userId);
            }
            else if (vehicleId != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE vehicle_id='{0}'", vehicleId));
            }

            List<Rent> rentList = new List<Rent>();

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);

                Rent rentToAdd = null;
                DateTime endTime = new DateTime(1, 1, 1);

                try
                {
                    while (reader.Read())
                    {
                        rentToAdd = new Models.RentInfo.Rent()
                        {
                            Id = Convert.ToInt32(reader["id"].ToString()),
                            BeginTime = Convert.ToDateTime(reader["begin_time"].ToString()),
                            Returned = reader["returned"].ToString().Equals("1"),
                            Discount = Convert.ToDouble(reader["discount"].ToString()),
                            TotalAmount = Convert.ToDouble(reader["total_amount"].ToString()),
                            Paid = reader["paid"].ToString().Equals("1"),
                            VehicleId = Convert.ToInt32(reader["vehicle_id"].ToString()),
                            UserId = Convert.ToInt32(reader["user_id"].ToString())
                        };

                        if(DateTime.TryParse(reader["end_time"].ToString(), out endTime))
                        {
                            rentToAdd.EndTime = endTime;
                        }
                        

                        rentList.Add(rentToAdd);
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

            return rentList;
        }
    }
}