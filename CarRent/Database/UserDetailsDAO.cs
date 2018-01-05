using CarRent.Helper;
using CarRent.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CarRent.Database
{
    public class UserDetailsDAO : IDAO
    {
        public void Insert(UserDetails userDetails)
        {
            string sqlCommand = String.Format("INSERT INTO user_details (email, street, city, zipcode, country, premium, user_id, birth_date)" +
                                          "VALUES ('{0}','{1}','{2}','{3}','{4}',b'{5}','{6}','{7}') RETURNING id",
                                          userDetails.Email, userDetails.Street, userDetails.City, userDetails.ZipCode, userDetails.Country,
                                          userDetails.Premium ? "1" : "0", userDetails.UserId, userDetails.BirthDate.ToShortDateString());

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommand, connection);

                if (reader.Read())
                {
                    int id = -1;
                    int.TryParse(reader.GetValue(0).ToString(), out id);
                    userDetails.Id = id;
                }
            }
        }

        public void Delete(params int[] idsToDelete)
        {
            if (idsToDelete.Count() == 0)
            {
                return;
            }

            StringBuilder sqlCommandBuilder = new StringBuilder("DELETE FROM user_details WHERE id IN (");

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

        public void Update(UserDetails userDetails)
        {
            string sqlCommand = String.Format("UPDATE user_details SET email='{0}', street='{1}', city='{2}', zipcode='{3}', country='{4}'," +
                                                "premium=b'{5}', user_id='{6}', birth_date='{7}' WHERE id='{8}'", 
                                                userDetails.Email, userDetails.Street, userDetails.City, userDetails.ZipCode, userDetails.Country,
                                                userDetails.Premium ? "1" : "0", userDetails.UserId, userDetails.BirthDate.ToShortDateString(), userDetails.Id);

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommand, connection);
            }
        }

        public List<UserDetails> GetUserDetails(int? userId = null, int? id = null)
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("SELECT * FROM user_details");

            if (id != null)
            {
                sqlCommandBuilder.Append("WHERE id=" + id);
            }
            else if (userId != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE user_id='{0}'", userId));
            }

            List<UserDetails> damageList = new List<UserDetails>();

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);

                UserDetails userdetailsToAdd = null;

                try
                {
                    while (reader.Read())
                    {
                        userdetailsToAdd = new UserDetails()
                        {
                            Id = Convert.ToInt32(reader["id"].ToString()),
                            Email = reader["email"].ToString(),
                            Street = reader["street"].ToString(),
                            City = reader["city"].ToString(),
                            ZipCode = reader["zipcode"].ToString(),
                            Country = reader["country"].ToString(),
                            Premium = reader["premium"].ToString().Equals("1"),
                            UserId = Convert.ToInt32(reader["user_id"].ToString()),
                            BirthDate = Convert.ToDateTime(reader["birth_date"].ToString())
                        };

                        damageList.Add(userdetailsToAdd);
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