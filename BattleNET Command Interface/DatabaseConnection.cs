using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace BattleNET_command_interface
{
    class DatabaseConnection
    {

        public MySqlConnection database;
        public string myConnectionString;

        public DatabaseConnection (string connectionString)
        {
            myConnectionString = connectionString;
        }

        public List<DBCommandData> GetCommands(int server)
        {
            database = new MySqlConnection(myConnectionString);
            List<DBCommandData> commands = new List<DBCommandData>();
            string command = "SELECT * FROM battleyecommandqueue WHERE server =" + server + " AND executed =0";

            try
            {
                database.Open();
                MySqlCommand mysqlCommand = new MySqlCommand(command, database);

                MySqlDataReader dataReader = mysqlCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    int id = Int32.Parse(dataReader.GetValue(0).ToString());
                    int cid = Int32.Parse(dataReader.GetValue(1).ToString());
                    string cmd = dataReader.GetValue(2).ToString();
                    int svr = Int32.Parse(dataReader.GetValue(4).ToString());

                    commands.Add(new DBCommandData(id, cid, cmd, svr));
                }
                dataReader.Close();
                mysqlCommand.Dispose();
                database.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Failed to read commands from database");
            }
            return commands;   
        }

        public bool RemoveCommand(int id)
        {
            database = new MySqlConnection(myConnectionString);
            string command = "DELETE FROM battleyecommandqueue WHERE id = " + id;
            bool result = true;
            try
            {
                database.Open();
                MySqlCommand mysqlCommand = new MySqlCommand(command, database);
                mysqlCommand.ExecuteNonQuery();
                mysqlCommand.Dispose();
                database.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Failed to delete command from database");
                result = false;
            }
            return result;
        }

        public bool UpdatePlayers(List<PlayerData> players, int server)
        {
            database = new MySqlConnection(myConnectionString);
            string deleteCommand = "DELETE FROM playerList WHERE server = " + server;
            

            bool result = true;
            try
            {
                database.Open();
                MySqlCommand mysqlCommand = new MySqlCommand(deleteCommand, database);
                mysqlCommand.ExecuteNonQuery();
                mysqlCommand.Dispose();
                database.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Failed to delete player list from database");
                result = false;
            }

            foreach (PlayerData player in players)
            {
   
                string command = "INSERT INTO playerList (number, guid, ipaddress, name, ping, server) VALUES ('" + MySqlHelper.EscapeString(player.Number.ToString()) + "', '" + MySqlHelper.EscapeString(player.Guid) + "', '"+ MySqlHelper.EscapeString(player.IpAddress.ToString()) +"', '"+ MySqlHelper.EscapeString(player.PlayerName) +"', "+ MySqlHelper.EscapeString(player.Ping.ToString()) +", "+ server.ToString() +")";

                try
                {
                    database.Open();
                    MySqlCommand mysqlCommand2 = new MySqlCommand(command, database);
                    mysqlCommand2.ExecuteNonQuery();
                    mysqlCommand2.Dispose();
                    database.Close();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    Console.WriteLine("Failed to insert player list into database");
                    result = false;
                }
            }
            Console.WriteLine("Player list updated for server " + server);
            return result;
        }
    }
}
