using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Belot.Core.Loggers.LoggerModels
{
    public class ScoreLoggerModel
    {
        public List<string> CardsT1 { get; set; }

        public List<string> CardsT2 { get; set; }

        public int Team1CP { get; set; }

        public int Team1AP { get; set; }

        public int Team1S { get; set; }

        public int Team2CP { get; set; }

        public int Team2AP { get; set; }

        public int Team2S { get; set; }

        public string AnnoucedSuite { get; set; }

        public int AnnoucerIndex { get; set; }

        public DateTime TimeStamp { get; set; }

    }
}
