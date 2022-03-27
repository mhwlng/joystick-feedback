using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EliteJournalReader.Events
{
    //When Written: when mining fragments are converted unto a unit of cargo by refinery
    //Parameters:
    //�	Type: cargo type
    public class MarketRefinedEvent : JournalEvent<MarketRefinedEvent.MarketRefinedEventArgs>
    {
        public MarketRefinedEvent() : base("MarketRefined") { }

        public class MarketRefinedEventArgs : JournalEventArgs
        {
            public string Type { get; set; }
            public string Type_Localised { get; set; }
        }
    }
}
