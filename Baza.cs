using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Telegram.Bot.Types;
using static System.Net.Mime.MediaTypeNames;

namespace tg1
{
    internal static class Baza
    {
        public static readonly string connectionString = @"Data Source = " + Environment.CurrentDirectory
            + @"\bd_telegram.db;";

        public static List<string> GetUSers()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                var users = new List<string>();
                connection.Open();
                var command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = $"select name from users";
                
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(reader.GetString(0));
                }
                connection.Close();

                return users;

                
            }

            
        }

        public static bool AddTender(string url, string numberOfApply, string dateEndReg, string nameApply, string status, string chatid )
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = new SqliteCommand();

                command.Connection = connection;
                if (IfTenderExists(url, out string none) == false)
                {
                    command.CommandText = $"insert into Apply(Url,NumberApply,DateEndReg,NameApply,status,ChatId) values('{url}','{numberOfApply}','{dateEndReg}','{nameApply}','{status}','{chatid}') ";
                    command.ExecuteNonQuery();
                    connection.Close();


                    return true;
                }
                else
                {
                    command.ExecuteNonQuery();
                    connection.Close();

                    return false;
                }
                
            }
           
        }


        public static bool UpdateDateBase(string url, string newDate)
        {

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string whatis;
                var command = new SqliteCommand();
                command.Connection = connection;

                if (IfTenderExists(url,out whatis) == true)
                {
                    command.CommandText = $"update Apply set DateEndReg = '{newDate}' where Url like '{url}'";
                    command.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
                else
                {
                    connection.Close();
                    return false;
                }
                
            }


        }



        public static List<string> AllWork(string status, int choice)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var apply = new List<string>();


                var command = new SqliteCommand();
                
                command.Connection = connection;
                if (choice == 1)
                { 
                    command.CommandText = $"select Url, NameApply from Apply where status like '{status}' ";

                }
                else if (choice == 3)
                {
                    command.CommandText = $"select Url, DateEndReg,NameApply, ChatId from Apply where status like '{status}' ";

                }
                else
                {
                    command.CommandText = $"select Url, NameApply from Apply";

                }



                var reader = command.ExecuteReader() ;
                if (choice == 3)
                {
                    while (reader.Read())
                    {
                        apply.Add(reader.GetString(0) + " " + reader.GetString(1) + "_" + reader.GetString(2) + " " + reader.GetString(3));

                    }
                }
                else 
                {
                    while (reader.Read())
                    {
                        apply.Add(reader.GetString(0) + " " + reader.GetString(1));

                    }
                }
               


                connection.Close();
                return apply;

            }



        }
        public static bool AddApply(string url, string status)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = new SqliteCommand();
                 string whatis;

                command.Connection = connection;
                if (IfTenderExists(url, out whatis) == false)
                {
                    
                    connection.Close();


                    return true;
                }
                else
                {
                    if (whatis == "url")
                    {
                        command.CommandText = $"update Apply set status = '{status}' where Url like '{url}' ";

                    }
                    else if (whatis == "number")
                    {
                        command.CommandText = $"update Apply set status = '{status}' where NumberApply like '{url}'";

                    }
                    command.ExecuteNonQuery();
                    connection.Close();

                    return false;
                }

            }

        }


        




        public static void RegisterUser(string username, string chatid)
        {
            using (var connection = new SqliteConnection(connectionString))
            { 
                connection.Open();
                var command = new SqliteCommand();
                 command.Connection = connection;
                if (IfUserExists(username) == false)
                {
                    command.CommandText = $"insert into users(Name, DateReg, MsgChatId) values('{username}','{DateTime.Now.ToString("D")}','{chatid}')";


                }
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        private static bool IfUserExists(string username)
        {
            using(var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = $"select 1 from users where name like '{username}'";
                

                return command.ExecuteScalar() != null;

                

            }

        }

        public static bool IfTenderExists(string url, out string whatis)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = new SqliteCommand();
                command.Connection = connection;

                if (url.Contains("zakupki"))
                {
                    command.CommandText = $"select 1 from Apply where Url like '{url}' ";
                    whatis = "url";
                }
                else
                {
                    command.CommandText = $"select 1 from Apply where NumberApply like '{url}'";
                    whatis = "number";
                }

              
                return command.ExecuteScalar() != null;

            }

        }

        



    }
}
