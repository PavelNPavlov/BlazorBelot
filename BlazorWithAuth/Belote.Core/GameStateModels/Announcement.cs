using Belot.Core.DeckCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belot.Core.GameStateModels
{
    public class Announcement
    {
        public string AnnoucementText { get; set; }

        public List<Card> Cards { get; set; }

        public int Value { get; set; }

        public int CountingValue { get; set; }

        public string Type { get; set; }

        public List<Card> CanAnnouceWith { get; set; }

        public Guid Id { get; set; }

        public List<Guid> ConflictsWith { get; set; }

        public Announcement()
        {
            this.Id = Guid.NewGuid();
            this.Cards = new List<Card>();
            this.ConflictsWith = new List<Guid>();
        }
    }
}
