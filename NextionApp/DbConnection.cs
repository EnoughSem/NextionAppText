using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextionApp
{
    class DbConnection
    {
        static NpgsqlConnection connection = new NpgsqlConnection("Server = mamont-server.ru; Port = 5432; Database = ostankino; User Id = daniel;Password = testStudent123;");
        

        public static DateTime[] GetDate()
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT \"Time_start\", \"Time_end\" FROM \"Schedule\" ORDER BY \"Time_start\" DESC LIMIT 1", connection);
            try
            {
                connection.Open();
                NpgsqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    var dateTimeStart = dataReader.GetDateTime(0);
                    //Console.WriteLine(dateTimeStart);
                    var dateTimeEnd = dataReader.GetDateTime(1);
                    //Console.WriteLine(dateTimeEnd);
                    connection.Close();
                    return new DateTime[] { dateTimeStart, dateTimeEnd };
                }
                               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                connection.Close();
            }

            return new DateTime[0];
        }

        public static void AddTime(DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO \"Schedule\" VALUES (uuid_generate_v4(), 15, TIMESTAMP WITH TIME ZONE \'"+dateTimeStart+ "\', TIMESTAMP WITH TIME ZONE \'"+dateTimeEnd+"\')", connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
