using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace BattleNET_command_interface
{
    public class PlayerData
    {
        public int Number { get; private set; }
        public string Guid { get; private set; }
        public IPAddress IpAddress { get; private set; }
        public string PlayerName { get; private set; }
        public string Ping { get; private set; }

        public PlayerData(int number, string guid, IPAddress ipaddress, string playername, string ping)
        {
            Number = number;
            Guid = guid;
            IpAddress = ipaddress;
            PlayerName = playername;
            Ping = ping;
        }
    }
}