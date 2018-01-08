using CarRent.Models.RentInfo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using CarRent.Helper;
using System.Text;

namespace CarRent.Database
{
    public class VehicleDAO : IDAO
    {
        public void Insert(Vehicle vehicle)
        {
            string sqlCommand = String.Format("INSERT INTO vehicle (user_id, type, price_day, price_hour, damaged,"+
                                                "description, next_revision, license_category, name)" +
                                          "VALUES ({0},'{1}','{2}','{3}',b'{4}','{5}','{6}','{7}','{8}'); SELECT last_insert_id()",
                                          vehicle.UserId == null ? "null" : vehicle.UserId.ToString(), vehicle.Type,
                                          vehicle.PriceDay, vehicle.PriceHour, vehicle.Damaged ? "1" : "0",
                                          vehicle.Description, vehicle.NextRevision.ToShortDateString(),
                                          vehicle.LicenseCategory, vehicle.Name);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommand, connection);

                if (reader.Read())
                {
                    int id = -1;
                    int.TryParse(reader.GetValue(0).ToString(), out id);
                    vehicle.Id = id;
                }
            }
        }

        public void Delete(params int[] idsToDelete)
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("DELETE FROM vehicle WHERE id IN (");

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

        public void Update(Vehicle vehicle)
        {
            string sqlCommand = String.Format("UPDATE vehicle SET user_id={0}, type='{1}', price_day='{2}', price_hour='{3}'," +
                                                "damaged=b'{4}', description='{5}', next_revision='{6}', license_category='{7}', name='{8}' WHERE id={9}",
                                                vehicle.UserId == null ? "null" : vehicle.UserId.ToString(), vehicle.Type, vehicle.PriceDay, vehicle.PriceHour,
                                                vehicle.Damaged ? "1" : "0", vehicle.Description, vehicle.NextRevision.ToShortDateString(), vehicle.LicenseCategory, vehicle.Name, vehicle.Id);

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommand, connection);
            }
        }

        public List<Vehicle> GetVehicles(int? userId = null, bool? damaged = null, int? id = null)
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("SELECT * FROM vehicle");

            if (id != null)
            {
                sqlCommandBuilder.Append(" WHERE id=" + id);
            }
            else if (userId != null & damaged != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE user_id={0} AND damaged=b'{1}'", userId, damaged.Value ? "1" : "0"));
            }
            else if(userId != null)
            {
                sqlCommandBuilder.Append(" WHERE user_id=" + userId);
            }
            else if(damaged != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE damaged=b'{0}'", damaged.Value ? "1" : "0"));
            }

            List<Vehicle> vehicleList = new List<Vehicle>();

            using (DbConnection connection = DBUtils.GetMySQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);

                Vehicle vehicleToAdd = null;
                int newUserId = -1;

                try
                {
                    while (reader.Read())
                    {
                        vehicleToAdd = new Models.RentInfo.Vehicle()
                        {
                            Id = Convert.ToInt32(reader["id"].ToString()),
                            Type = reader["type"].ToString(),
                            PriceDay = Convert.ToDouble(reader["price_day"].ToString()),
                            PriceHour = Convert.ToDouble(reader["price_hour"].ToString()),
                            Damaged = reader["damaged"].ToString().Equals("1"),
                            Description = reader["description"].ToString(),
                            NextRevision = Convert.ToDateTime(reader["next_revision"].ToString()),
                            LicenseCategory = reader["license_category"].ToString(),
                            Name = reader["name"].ToString(),
                        };
                        
                        if (int.TryParse(reader["user_id"].ToString(), out newUserId))
                        {
                            vehicleToAdd.UserId = newUserId;
                        }

                        vehicleList.Add(vehicleToAdd);
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

            return vehicleList;
        }
    }
}
