using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Lobbies
{
    public class LobbyConnectResponse
    {
        public bool Succeded { get; set; }

        public string LobbyName { get; set; }
    }

    public class LobbyConnectRequest
    {
        public string UserName { get; set; }

        public string LobbyName { get; set; }
    }

    public class LobbyDisconnectRequest
    {
        public string LobbyName { get; set; }
    }

    public class LobbyDisconnectResponse
    {
        public bool Succeded { get; set; }

        public string LobbyName { get; set; }
    }
}
        