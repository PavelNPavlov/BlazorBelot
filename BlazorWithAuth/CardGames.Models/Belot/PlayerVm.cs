using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Belot
{
    public class PlayerVm
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public PlayerStateVm PlayerState { get; set; }
    }
}
