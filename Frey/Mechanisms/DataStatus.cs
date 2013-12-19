using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automata.Mechanisms
{
    public enum DataStatus
    {
        Initializing,
        WaitingForData,
        HasDataInQueue,
        ReachTimeEnd,
    }
}
