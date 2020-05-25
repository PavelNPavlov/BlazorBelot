using Belot.Core.DeckCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belot.Core.GameStateModels
{
    public class Player
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public PlayerState PlayerState { get; set; }

        public Player()
        {
            this.PlayerState = new PlayerState()
            {
                ActionRequired = "Wait",
                InTurn = false
            };
        }
        public Player(string id)
        {
            this.Id = id;
        }      

        public void RecieveCards(IList<Card> cards)
        {
            if(this.PlayerState.Hand == null)
            {
                this.PlayerState.Hand = new List<Card>();
            }

            this.PlayerState.Hand.AddRange(cards);

            this.PlayerState.Hand = this.PlayerState.Hand.OrderBy(x => x.SortSuit).ThenBy(x => x.SortValueNT).ToList();
        }
    }
}
