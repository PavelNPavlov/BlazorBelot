using System;
using System.Collections.Generic;
using System.Text;

namespace Belot.Core.DeckCore
{
    public class Card
    {
        public string Value { get; set; }

        public string Suit { get; set; }

        public int SortValueNT { get; set; }

        public int SortValueAT { get; set; }

        public int OrderValue { get; set; }

        public int ATPoints { get; set; }

        public int NTPoints { get; set; }

        public int SortSuit { get; set; }

        public bool CanPlay { get; set; }

    }
}
