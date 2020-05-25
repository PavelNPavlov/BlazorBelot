using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Lobbies
{
    public class LobbyData
    {

        public LobbyData()
        {
            this.LobbyUsers = new List<LobbyUser>();
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public int MaxCount { get; set; }

        public List<LobbyUser> LobbyUsers { get; set; }

        public string HostId { get; set; }

        public DateTime TimeSinceChange { get; set; }
    }
}
