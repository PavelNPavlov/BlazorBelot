using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Belot
{
    public class CardVm
    {
        public string Value { get; set; }

        public string Suit { get; set; }

        public bool CanPlay { get; set; }
    }
}
