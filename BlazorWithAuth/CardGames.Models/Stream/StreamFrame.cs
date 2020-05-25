using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Models.Stream
{
    public class StreamFrame
    {
        public DateTime TimeStamp { get; set; }

        public string Base64ImageString { get; set; }
    }
}
