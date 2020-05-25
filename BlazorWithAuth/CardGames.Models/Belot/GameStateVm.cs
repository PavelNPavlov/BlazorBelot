using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Belot
{
    public class GameStateVm
    {
        public List<PlayerVm> Players { get; set; }

        public int DealerIndex { get; set; }

        public TableStateVm TableState { get; set; }

        public Dictionary<string, PlayerStateVm> PlayerStates { get; set; }

        public bool CanStart { get; set; }

        public string GameId { get; set; }
    }
}
