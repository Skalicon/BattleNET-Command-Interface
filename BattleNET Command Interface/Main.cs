using System;
using System.Collections.Generic;
using System.Linq;
using BattleNET;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace BattleNET_command_interface
{
    internal class main
    {
        private static void Main(string[] args)
        {
            // ------- Setup -------

            //Each battle eye command is associated with a number, here i just populate it into a list and print it so you can see what they are.
            List<BattlEyeCommand> CommandsList = Enum.GetValues(typeof(BattlEyeCommand)).Cast<BattlEyeCommand>().ToList();
            Console.WriteLine("List of commands");
            int i = 0;
            foreach (BattlEyeCommand command in CommandsList)
            {
                Console.WriteLine(i + " - " + command.ToString() + " ");
                i++;
            }

            //Database connection information here, could add this in an external file but I'm lazy.
            string databaseAddress = "localhost";
            string databaseUser = "root";
            string databasePassword = "";
            string databaseName = "";

            DatabaseConnection database = new DatabaseConnection("server=" + databaseAddress + ";uid=" + databaseUser + ";pwd=" + databasePassword + ";database=" + databaseName + ";");


            //This will be our array of battle eye clients
            List<BattleyeConnection> battleEyeClients = new List<BattleyeConnection>();

            //I have setup a text file servers.txt with the connection data for each server. Just go in a change the default values to those used for your server.
            //This could be populated from the database if you wanted.
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader("servers.txt");
            while ((line = file.ReadLine()) != null)
            {
                string[] serverData = line.Split(',');

                battleEyeClients.Add(new BattleyeConnection(Int32.Parse(serverData[0]), serverData[1], Int32.Parse(serverData[2]), serverData[3], database));
            }
            file.Close();

            // ------- Main code -------

            //Loop every 15 seconds forever
            while (true)
            {
                //For each battle eye connection
                foreach (BattleyeConnection client in battleEyeClients)
                {
                    //Check db, this would be more efficient if we just did a single get for all commands, and then went through them for each server.
                    List<DBCommandData> commands = database.GetCommands(client.ServerNumber);
                    //Run any queued commands
                    foreach (DBCommandData command in commands)
                    {
                        bool result = client.SendCommand(command);
                        if (result)
                        {
                            database.RemoveCommand(command.ID);
                        }
                    }
                }
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(15));
            }
        }
    }
}