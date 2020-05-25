using Belot.Core.DeckCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belot.Core.GameStateModels
{
    public class RoundState
    {
        public string Stage { get; set; }

        public int HangingPoints { get; set; }

        public Deck Deck { get; set; }

        public string BidSuit { get; set; }

        public int BidderIndex { get; set; }

        public bool IsDouble { get; set; }

        public bool IsQuadrapul { get; set;}

        public int HandNumber { get; set; }

        public int CardsPlayedInHand { get; set; }

        public int CurrentHandFirstPlayerIndex { get; set; }

        //public List<Announcement> Team1PlayAnnouncments { get; set; }

        //public List<Announcement> Team2PlayAnnouncments { get; set; }

        public List<List<Announcement>> TeamAnn { get; set; }

        public List<int> TeamPointsFromCards { get; set; }

        public List<int> TeamPointsFromAnn { get; set; }

        public List<int> TeamPointsFromDispAnn { get; set; }

        public List<int> TeamRoundScore { get; set; }

        public List<int> TeamRoundPoints { get; set; }

        public int LastHandWinTeam { get; set; }
        //public List<Player> Players { get; set; }

        public RoundState()
        {
            this.Deck = new Deck();

            this.BidSuit = "Pass";
            this.BidderIndex = -1;
            //this.Team1PlayAnnouncments = new List<Announcement>();
            //this.Team2PlayAnnouncments = new List<Announcement>();

            this.TeamPointsFromCards = new List<int> { 0, 0 };
            this.TeamPointsFromAnn = new List<int> { 0, 0 };
            this.TeamPointsFromDispAnn = new List<int> { 0, 0 };
            this.TeamRoundScore = new List<int> { 0, 0 };
            this.TeamRoundPoints = new List<int> { 0, 0 };
            this.TeamAnn = new List<List<Announcement>>() { new List<Announcement>(), new List<Announcement>() };

        }


    }
}
