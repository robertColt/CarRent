using CarRent.Helper;
using CarRent.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CarRent.Database
{
    public class BlackListDAO : IDAO
    {
        public void Insert(BlackList blackList)
        {
            string sqlCommand = String.Format("INSERT INTO public.black_list (warnings, banned, user_id)" +
                                          "VALUES ('{0}',b'{1}','{2}') RETURNING id",
                                          blackList.Warnings, blackList.Banned ? "1" : "0", blackList.UserId);

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommand, connection);

                if (reader.Read())
                {
                    int id = -1;
                    int.TryParse(reader.GetValue(0).ToString(), out id);
                    blackList.Id = id;
                }
            }
        }

        public void Delete(params int[] idsToDelete)
        {
            if (idsToDelete.Count() == 0)
            {
                return;
            }

            StringBuilder sqlCommandBuilder = new StringBuilder("DELETE FROM public.black_list WHERE id IN (");

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

        public void Update(BlackList blackList)
        {
            string sqlCommand = String.Format("UPDATE public.black_list SET warnings='{0}', banned=b'{1}', user_id='{2}' WHERE id='{3}'",
                                            blackList.Warnings, blackList.Banned ? "1" : "0", blackList.UserId, blackList.Id);

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DBUtils.ExecuteCommand(sqlCommand, connection);
            }
        }

        public List<BlackList> GetBlackLists(int? userId = null, bool? banned = null, int? id=null)
        {
            StringBuilder sqlCommandBuilder = new StringBuilder("SELECT * FROM public.black_list");

            if(id != null)
            {
                sqlCommandBuilder.Append(" WHERE id=" + id);
            }
            else if (userId != null && banned != null)
            {
                sqlCommandBuilder.Append(String.Format(" WHERE user_id='{0}'", userId));
            }
            else if (userId != null)
            {
                sqlCommandBuilder.Append(" WHERE user_id=" + userId);
            }
            else if (banned != null)
            {
                sqlCommandBuilder.Append(" WHERE banned=" + (banned.Value ? "1" : "0"));
            }


            List<BlackList> userDetailsList = new List<BlackList>();

            using (DbConnection connection = DBUtils.GetPostgreSQLDBConnection())
            {
                DbDataReader reader = DBUtils.ExecuteCommand(sqlCommandBuilder.ToString(), connection);

                BlackList blackListToAdd = null;

                try
                {
                    while (reader.Read())
                    {
                        blackListToAdd = new BlackList()
                        {
                            Id = Convert.ToInt32(reader["id"].ToString()),
                            Warnings = Convert.ToInt32(reader["warnings"].ToString()),
                            Banned = reader["banned"].ToString().Equals("1"),
                            UserId = Convert.ToInt32(reader["user_id"].ToString()),
                        };

                        userDetailsList.Add(blackListToAdd);
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

            return userDetailsList;
        }
    }
}