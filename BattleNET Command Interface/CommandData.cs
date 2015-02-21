using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BattleNET;

namespace BattleNET_command_interface
{
    public class CommandData
    {
        public BattlEyeCommand Type { get; private set; }
        public int ID { get; private set; }

        public CommandData(int id, BattlEyeCommand type)
        {
            this.Type = type;
            this.ID = id;
        }
    }
}