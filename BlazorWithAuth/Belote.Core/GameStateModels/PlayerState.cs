using Belot.Core.DeckCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belot.Core.GameStateModels
{
    public class PlayerState
    {
        public bool InTurn { get; set; }

        public string ActionRequired { get; set; }

        public List<Card> Hand { get; set; }

        public List<string> PossibleBids { get; set; }
        public List<Announcement> PossiblePlayAnnoucements { get; set; }

        public PlayerState()
        {
            this.Hand = new List<Card>();
            this.PossiblePlayAnnoucements = new List<Announcement>();
            this.PossibleBids = new List<string>();
        }
    }
}
