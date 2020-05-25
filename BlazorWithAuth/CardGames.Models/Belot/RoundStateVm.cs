using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Belot
{
    public class RoundStateVm
    {
        public string Stage { get; set; }

        public int HangingPoints { get; set; }

        //public Deck Deck { get; set; }

        public string BidSuit { get; set; }

        public int BidderIndex { get; set; }

        public bool IsDouble { get; set; }

        public bool IsQuadrapul { get; set; }

        public List<int> TeamPointsFromCards { get; set; }

        public List<int> TeamPointsFromAnn { get; set; }

        public List<int> TeamRoundScore { get; set; }

        public List<int> TeamRoundPoints { get; set; }

        public List<int> TeamPointsFromDispAnn { get; set; }
    }
}
