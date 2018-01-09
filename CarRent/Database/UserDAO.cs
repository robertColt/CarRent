using CarRent.Helper;
using CarRent.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;

namespace CarRent.Database
{
    public class UserDAO : IDAO
    {
        public void Insert(User user)
        {
            string sqlCommand = String.Format("INSERT INTO public.user (username, password, name, surname, user_details_id, function)" +
                                          "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}') RETURNING id",
                                          user.Username, user.Password, user.Name, user.Surname, user.UserDetailsId, user.UserFunction);

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommand, connection);

                if (reader.Read())
                {
                    int id = -1;
                    int.TryParse(reader.GetValue(0).ToString(), out id);
                    user.Id = id;
                }
            }
        }

        public void Delete(params int[] idsToDelete)
        {
            if (idsToDelete.Count() == 0)
            {
                return;
            }

            StringBuilder sqlCommandBuilder = new StringBuilder("DELETE FROM public.user WHERE id IN (");

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

        public void Update(User user)
        {
            string sqlCommand = String.Format("UPDATE public.user SET username='{0}', password='{1}', name='{2}', surname='{3}', user_details_id='{4}',"+
                                                "function='{5}' WHERE id='{6}'", user.Username, user.Password, user.Name, user.Surname,
                                                user.UserDetailsId, user.UserFunction, user.Id);

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommand, connection);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="function"></param>
        /// <param name="id"></param>
        /// <returns>empty list if nothing in database</returns>
        public List<User> GetUsers(string username = null, string function = null, int? id = null)
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("SELECT * FROM public.user");

            if (id != null)
            {
                sqlCommandBuilder.Append("WHERE id=" + id);
            }
            else if (function != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE function = '{0}'", function));
            }
            else if (username != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE username = '{0}'", username));
            }


            List<User> userList = new List<User>();

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);

                User userToAdd = null;

                try
                {
                    while (reader.Read())
                    {
                        userToAdd = new User()
                        {
                            Id = Convert.ToInt32(reader["id"].ToString()),
                            Username = reader["username"].ToString(),
                            Password = reader["password"].ToString(),
                            Name = reader["name"].ToString(),
                            Surname = reader["surname"].ToString(),
                            UserFunction = (User.Function)Enum.Parse(typeof(User.Function), reader["function"].ToString()),
                            UserDetailsId = Convert.ToInt32(reader["user_details_id"].ToString()),
                        };
                        try
                        {
                            userToAdd.UserDetails = new UserDetailsDAO().GetUserDetails(userId: userToAdd.Id).First();
                        }
                        catch (Exception)
                        {
                            DebugLog.WriteLine("Exception didnt find userdetails for user with id " + userToAdd.Id);
                        }

                        userList.Add(userToAdd);
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

            return userList;
        }
    }
}