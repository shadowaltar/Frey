using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automata.Entities;

namespace Automata.Quantitatives.Indicators
{
    public class MACD : Indicator
    {
        public MACD()
        {
            Name = "Moving Average Convergence/Divergence";
            Values = new Dictionary<string, Queue<double>>();
            Values["MACD"] = new Queue<double>();
            Values["Signal"] = new Queue<double>();
            Values["Histogram"] = new Queue<double>();
        }

        public Dictionary<string, Queue<double>> Values { get; private set; }

        public void Compute(Price price)
        {

        }
    }
}
