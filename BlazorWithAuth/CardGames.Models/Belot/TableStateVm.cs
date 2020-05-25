using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Belot
{
    public class TableStateVm
    {
        public string PlayerInTurnId { get; set; }

        public int PlayerInTurnIndex { get; set; }

        public string PlayerInTurnName { get; set; }

        public int CardsInHand { get; set; }

        public string DealerId { get; set; }

        public string ActionRequied { get; set; }

        public List<string> PlayerNames { get; set; }

        public string GameId { get; set; }

        public RoundStateVm CurrentRoundState { get; set; }

        public List<CardVm> CardsOnTable { get; set; }

        public List<int> OveralScore { get; set; }

        public List<List<int>> CurrentGameScore { get; set; }

        public List<string> PlayerAnnThisHand { get; set; }

        public int TopCardOwner { get; set; }

        public int DealerIndex { get; set; }
    }
}
