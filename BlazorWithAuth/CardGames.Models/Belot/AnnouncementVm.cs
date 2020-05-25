using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Belot
{
    public class AnnouncementVm
    {
        public string AnnoucementText { get; set; }

        public List<CardVm> Cards { get; set; }

        public int Value { get; set; }

        public string Type { get; set; }

        public List<CardVm> CanAnnouceWith { get; set; }

        public Guid Id { get; set; }

        public List<Guid> ConflictsWith { get; set; }

        public bool Selected { get; set; }
    }
}
