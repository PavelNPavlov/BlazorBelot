using Belot.Core.DeckCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belot.Core.GameStateModels
{
    public class TableState
    {
        public string PlayerInTurnId { get; set; }

        public int PlayerInTurnIndex { get; set; }

        public string PlayerInTurnName { get; set; }

        public int CardsInHand { get; set; }

        public string DealerId { get; set; }

        public string ActionRequied { get; set; }

        public List<string> PlayerNames { get; set; }

        public List<string> PlayerAnnThisHand { get; set; }

        public string GameId { get; set; }

        public RoundState CurrentRoundState { get; set; }

        public List<Card> CardsOnTable { get; set; }

        public Card FirstCardInRound { get { return this.CardsOnTable[this.CurrentRoundState.CurrentHandFirstPlayerIndex]; } }

        public int TeamIndexHoldingHand { get; set; }

        public List<List<Card>> TeamCardPiles { get; set; }

        public List<int> OveralScore { get; set; }

        public List<List<int>> CurrentGameScore { get; set; }

        public int TopCardOwner { get; set; }

        public int DealerIndex { get; set; }

        public int HangingPoints { get; set; }

        public TableState()
        {
            this.CardsOnTable = new List<Card>
            {
                new Card(),
                new Card(),
                new Card(),
                new Card()
            };

            this.PlayerAnnThisHand = new List<string>()
            {
                "",
                "",
                "",
                ""
            };

            //this.Team1Pile = new List<Card>();
            //this.Team2Pile = new List<Card>();

            this.TeamCardPiles = new List<List<Card>>() { new List<Card>(), new List<Card>() };

            this.OveralScore = new List<int> { 2, 0 };

            this.CurrentGameScore = new List<List<int>>
            {
                new List<int>(){ 70},
                new List<int>(){ 106}
            };

            this.TopCardOwner = -1;
        }


    }
}
