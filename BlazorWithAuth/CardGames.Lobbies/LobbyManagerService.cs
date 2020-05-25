using CardGames.Models.Lobbies;
using System;
using System.Collections.Generic;

namespace LobbyManager
{
    public class LobbyManagerService
    {
        public bool Initialized { get; set; }


        public List<LobbyData> Lobbies { get; set; }


        public LobbyManagerService()
        {
            this.Lobbies = new List<LobbyData>();

            this.Initialized = false;
        }

        public void Initialize()
        {
            if (!Initialized)
            {
                Seed();
                this.Initialized = true;
            }
        }


        private void Seed()
        {
            var test1 = new LobbyData()
            {
                Name = "Lobby1",
                TimeSinceChange = DateTime.Now,
                MaxCount = 4,
                Type = "Belot"
            };

            var test2 = new LobbyData()
            {
                Name = "Lobby2",
                TimeSinceChange = DateTime.Now,
                MaxCount = 4,
                Type = "Belot"
            };

            this.Lobbies.Add(test1);
            this.Lobbies.Add(test2);
        }
    }
}
