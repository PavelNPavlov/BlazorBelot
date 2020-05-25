using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Shared
{
    public static class ChannelConstants
    {
        public const string LobbiesDataChannel = "Lobbies";

        public const string LobbyConnectionAnswer = "LobbyConnection";
        public const string LobbyDisconectAnswer = "LobbyDisconnect";

        public const string GameStarting = "GameStarting";

        public const string JoinGameAnswer = "JoinGameResult";


        public const string GameChange = "GameChange";

        public const string TableUpdate = "UpdateToTable";

        public const string PlayerStateChange = "PlayerStateChange";
    }
}
