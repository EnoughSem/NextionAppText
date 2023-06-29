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
        public static DateTime[] GetDate()
        {
            NpgsqlConnection connection = new NpgsqlConnection("Server = mamont-server.ru; Port = 5432; Database = ostankino; User Id = daniel;Password = testStudent123;");
            NpgsqlCommand command = new NpgsqlCommand($"SELECT \"time_start\", \"time_end\" FROM \"Schedule\" WHERE \"time_start\" <= '{DateTime.UtcNow.ToString("O")}' AND \"time_end\" >= '{DateTime.UtcNow.ToString("O")}'", connection);
            try
            {
                connection.Open();
                NpgsqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    var dateTimeStart = dataReader.GetDateTime(0);
                    var dateTimeEnd = dataReader.GetDateTime(1);
                    connection.Close();
                    return new DateTime[] { dateTimeStart, dateTimeEnd };
                }
                               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                connection.Close();
            }
            finally
            {
                connection.Close();
            }

            return null;
        }

        public static string GetGuid(int studioNumber)
        {
            NpgsqlConnection connection = new NpgsqlConnection("Server = mamont-server.ru; Port = 5432; Database = ostankino; User Id = daniel;Password = testStudent123;");
            NpgsqlCommand command = new NpgsqlCommand($"SELECT \"id\" FROM \"Studios\" WHERE \"number_studio\"={studioNumber}", connection);
            try
            {
                connection.Open();              
                NpgsqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Guid guid = dataReader.GetGuid(0);
                    connection.Close();
                    return guid.ToString();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return string.Empty;
        }

        public static bool AddTime(DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            if (checkTime(dateTimeStart, dateTimeEnd))
            {
                string guid = GetGuid(1);
                NpgsqlConnection connection = new NpgsqlConnection("Server = mamont-server.ru; Port = 5432; Database = ostankino; User Id = daniel;Password = testStudent123;");
                NpgsqlCommand command = new NpgsqlCommand($"INSERT INTO \"Schedule\" VALUES (uuid_generate_v4(),'{guid}', TIMESTAMP WITH TIME ZONE '{dateTimeStart.ToString("O")}', TIMESTAMP WITH TIME ZONE '{dateTimeEnd.ToString("O")}')", connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
                return false;
            }

            return false;
        }

        public static bool checkTime(DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            NpgsqlConnection connection = new NpgsqlConnection("Server = mamont-server.ru; Port = 5432; Database = ostankino; User Id = daniel;Password = testStudent123;");
            NpgsqlCommand command = new NpgsqlCommand($"SELECT \"time_start\", \"time_end\" FROM \"Schedule\" WHERE (\"time_start\" < '{dateTimeStart.ToString("O")}' AND \"time_start\" >'{dateTimeEnd.ToString("O")}') OR (\"time_end\" < '{dateTimeStart.ToString("O")}' AND  \"time_end\" > '{dateTimeEnd.ToString("O")}') OR (\"time_start\" = '{dateTimeStart.ToString("O")}' AND \"time_end\" = '{dateTimeEnd.ToString("O")}')", connection);
            try
            {
                connection.Open();
                NpgsqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    connection.Close();
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return true;
        }
    }
}
