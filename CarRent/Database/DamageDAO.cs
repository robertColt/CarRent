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
    public class DamageDAO : IDAO
    {
        public void Insert(Damage damage)
        {
            string sqlCommand = String.Format("INSERT INTO damage (fine_amount, paid, date, vehicle_id, user_id)" +
                                          "VALUES ('{0}',b'{1}','{2}','{3}','{4}'); SELECT last_insert_id()",
                                          damage.FineAmount, damage.Paid ? "1" : "0", damage.Date.ToShortDateString(), damage.VehicleId, damage.UserId);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommand, connection);

                if (reader.Read())
                {
                    int id = -1;
                    int.TryParse(reader.GetValue(0).ToString(), out id);
                    damage.Id = id;
                }
            }
        }

        public void Delete(params int[] idsToDelete)
        {
            if (idsToDelete.Count() == 0)
            {
                return;
            }

            StringBuilder sqlCommandBuilder = new StringBuilder("DELETE FROM damage WHERE id IN (");

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

        public void Update(Damage damage)
        {
            string sqlCommand = String.Format("UPDATE damage SET fine_amount='{0}', paid=b'{1}', date='{2}', vehicle_id='{3}', user_id='{4}' WHERE id='{5}'",
                                                damage.FineAmount, damage.Paid ? "1" : "0", damage.Date.ToShortDateString(), damage.VehicleId, damage.UserId, damage.Id);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommand, connection);
            }
        }

        public List<Damage> GetDamages(int? userId = null, bool? paid = null, int? vehicleId = null)
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("SELECT * FROM damage");

            if (userId != null && paid != null && vehicleId != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE vehicle_id='{0}' AND user_id='{1}' AND paid=b'{2}'", vehicleId, userId, paid.Value ? "1" : "0"));
            }
            else if (userId != null & paid != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE user_id='{0}' AND paid = b'{1}'", userId, paid.Value ? "1" : "0"));
            }
            else if (userId != null && vehicleId != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE user_id='{0}' AND vehicle_id = '{1}'", userId, vehicleId));
            }
            else if (vehicleId != null && paid != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE vehicle_id='{0}' AND paid = b'{1}'", vehicleId, paid.Value ? "1" : "0"));
            }
            else if(userId != null)
            {
                sqlCommandBuilder.Append(" WHERE user_id="+userId);
            }
            else if (vehicleId != null)
            {
                sqlCommandBuilder.Append(" WHERE vehicle_id=" + userId);
            }
            else if (paid != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE paid='{0}'", paid.Value ? "1" : "0"));
            }


            List<Damage> damageList = new List<Damage>();

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);

                Damage damageToAdd = null;

                try
                {
                    while (reader.Read())
                    {
                        damageToAdd = new Models.RentInfo.Damage()
                        {
                            Id = Convert.ToInt32(reader["id"].ToString()),
                            FineAmount = Convert.ToDouble(reader["fine_amount"].ToString()),
                            Paid = reader["paid"].ToString().Equals("1"),
                            Date = Convert.ToDateTime(reader["date"].ToString()),
                            VehicleId = Convert.ToInt32(reader["vehicle_id"].ToString()),
                            UserId = Convert.ToInt32(reader["user_id"].ToString()),
                        };
                        damageList.Add(damageToAdd);
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

            return damageList;
        }
    }
}