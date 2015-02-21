using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleNET_command_interface
{
    class BanData
    {
        public string Guid { get; private set; }
        public string Duration { get; private set; }
        public string Reason { get; private set; }

        public BanData(string guid, string duration, string reason)
        {
            Guid = guid;
            Duration = duration;
            Reason = reason;
        }
    }
}
