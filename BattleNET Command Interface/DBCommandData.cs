using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleNET;

namespace BattleNET_command_interface
{
    class DBCommandData
    {
        public int ID { get; private set; }
        public BattlEyeCommand CommandType { get; private set; }
        public string Command { get; private set; }
        public int Server { get; private set; }
        List<BattlEyeCommand> test = Enum.GetValues(typeof(BattlEyeCommand)).Cast<BattlEyeCommand>().ToList();

        public DBCommandData (int id, int cid, string command, int server)
        {
            ID = id;
            Command = command;
            Server = server;
            CommandType = test[cid];
        }
    }
}
