using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Belot
{
    public class JoinGameReq
    {
        public string GameName { get; set; }

        public string PlayerName { get; set; }

        public int Seat { get; set; }
    }

    public class JoinGameRes
    {
        public bool Success { get; set; }
    }
}
