using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Belot
{
    public class PlayerStateVm
    {
        public bool InTurn { get; set; }

        public string ActionRequired { get; set; }

        public List<CardVm> Hand { get; set; }

        public List<string> PossibleBids { get; set; }

        public List<AnnouncementVm> PossiblePlayAnnoucements { get; set; }

    }
}
