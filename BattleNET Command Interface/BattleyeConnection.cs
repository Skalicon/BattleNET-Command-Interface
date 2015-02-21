using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using BattleNET;
using System.Text.RegularExpressions;
using System.IO;

namespace BattleNET_command_interface
{
    class BattleyeConnection
    {
        public BattlEyeClient Client { get; private set; }
        public int ServerNumber { get; private set; }
        public string AddressURL { get; private set; }
        public IPAddress AddressIP { get; private set; }
        public int Port { get; private set; }
        public List<KeyValuePair<int, BattlEyeCommand>> commandQueue = new List<KeyValuePair<int, BattlEyeCommand>>();
        public List<PlayerData> PlayerList = new List<PlayerData>();
        public List<BanData> BanList = new List<BanData>();

        private string Pass;
        private DatabaseConnection Database;

        public BattleyeConnection(int serverNumber, string addressURL, int port, string pass, DatabaseConnection database)
        {
            ServerNumber = serverNumber;
            AddressURL = addressURL;
            Port = port;
            Pass = pass;
            Database = database;
            GetResolvedConnecionIPAddress();
            NewConnection();
        }

        public void RefreashPlayerList()
        {
            if (CheckConnection())
            {
                int commandNumber = Client.SendCommand(BattlEyeCommand.Players);
                commandQueue.Add(new KeyValuePair<int, BattlEyeCommand>(commandNumber, BattlEyeCommand.Players));
            }
        }

        public void RefreashBanList()
        {
            if (CheckConnection())
            {
                Console.WriteLine("Banlist command sent for server " + ServerNumber);
                int commandNumber = Client.SendCommand(BattlEyeCommand.Bans);
                commandQueue.Add(new KeyValuePair<int, BattlEyeCommand>(commandNumber, BattlEyeCommand.Bans));
            }
        }
   
        public bool SendCommand(DBCommandData command)
        {
            bool result = false;
            if (CheckConnection())
            {
                int index;

                index = Client.SendCommand(command.CommandType, command.Command);
                result = (index == 256) ? false : true;

                //Additional commands
                if (command.CommandType == BattlEyeCommand.AddBan || command.CommandType == BattlEyeCommand.RemoveBan)
                {
                    Client.SendCommand(BattlEyeCommand.LoadBans);
                }
            }
            return result;
        }

        private void NewConnection()
        {
            if (Client == null)
            {
                Client = new BattlEyeClient(new BattlEyeLoginCredentials(AddressIP, Port, Pass));
                Client.BattlEyeMessageReceived += BattlEyeMessageReceived;
            }
            CheckConnection();
        }

        private bool CheckConnection()
        {
            bool result = false;
            if (!Client.Connected)
            {
                if (GetResolvedConnecionIPAddress())
                {
                    Client.UpdateLoginCredentials(AddressIP, Port, Pass);
                    Client.Connect();
                    if (Client.Connected)
                    {
                        result = true;
                        Console.WriteLine("Server " + ServerNumber + " connected");
                    }
                }
                else
                {
                    Console.WriteLine("Server " + ServerNumber + " failed to resolve ip");
                }
            }
            else
            {
                result = true;
            }
            return result;
        }

        private void BattlEyeMessageReceived(BattlEyeMessageEventArgs args)
        {
            bool result = false;
            foreach (KeyValuePair<int, BattlEyeCommand> command in commandQueue)
            {
                if (command.Key == args.Id)
                {

                    if (command.Value == BattlEyeCommand.Players)
                    {
                        formatPlayers(args.Message);
                    }
                    else if (command.Value == BattlEyeCommand.Bans)
                    {
                        formatBans(args.Message);
                    }
                    commandQueue.Remove(command);
                    result = true;
                }
            }
            if (!result)
            {
                WriteLog(args.Message);
            }
        }

        private void formatPlayers(string p)
        {
            List<string> players = new List<string>(Regex.Split(p, @"\n"));
            PlayerList.Clear();
            players.RemoveRange(0, 3);
            players.RemoveAt(players.Count - 1);
            for (int i = 0; i < players.Count; i++)
            {
                //My regex Foo is bad, I'm certain this could be much better.
                MatchCollection mc = Regex.Matches(players[i], @"\s?(?<playerNumber>\d+)\s+(?<ipaddress>.+)\:\d+\s+(?<ping>\d+)\s+(?<guid>\w*)\(OK\)\s(?<name>.*)(?:\(Lobby\))?");
                foreach (Match match in mc)
                {
                    GroupCollection groups = match.Groups;
                    IPAddress ipaddress = IPAddress.Parse(groups[2].ToString());
                    PlayerData player = new PlayerData(Int32.Parse(groups[1].ToString()), groups[4].ToString(), ipaddress, groups[5].ToString(), groups[3].ToString());
                    PlayerList.Add(player);
                }
            }
            Database.UpdatePlayers(PlayerList, ServerNumber);
        }

        private void formatBans(string p)
        {
            List<string> bans = new List<string>(Regex.Split(p, @"\n"));
            BanList.Clear();
            bans.RemoveRange(0, 3);
            foreach (string banString in bans)
            {
                string delimStr = " ";
                char[] delimiter = delimStr.ToCharArray();
                string[] banArray = banString.Split(delimiter, 4, StringSplitOptions.RemoveEmptyEntries);

                BanList.Add(new BanData(banArray[1], banArray[2], banArray[3]));
            }
        }

        private bool GetResolvedConnecionIPAddress()
        {
            bool isResolved = false;
            IPHostEntry hostEntry = null;
            IPAddress resolvIP = null;
            try
            {
                if (!IPAddress.TryParse(AddressURL, out resolvIP))
                {
                    hostEntry = Dns.GetHostEntry(AddressURL);

                    if (hostEntry != null && hostEntry.AddressList != null && hostEntry.AddressList.Length > 0)
                    {
                        if (hostEntry.AddressList.Length == 1)
                        {
                            resolvIP = hostEntry.AddressList[0];
                            isResolved = true;
                        }
                        else
                        {
                            foreach (IPAddress var in hostEntry.AddressList)
                            {
                                if (var.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    resolvIP = var;
                                    isResolved = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    isResolved = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server " + ServerNumber + " error attempting to resolve ip");
            }
            finally
            {
                AddressIP = resolvIP;
            }

            return isResolved;
        }

        private void WriteLog(string line)
        {
            StreamWriter log = new StreamWriter("log/server" + ServerNumber + "chat.log", true);
            log.WriteLine(line);
            log.Close();
        }
    }
}
