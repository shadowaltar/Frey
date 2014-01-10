using Automata.Entities;

namespace Automata.Quantitatives.Indicators
{
    public abstract class Indicator
    {
        public string Name { get; protected set; }

        /// <summary>
        /// Compute the indicator values, when the input price is an update
        /// of previous input price. The indicator only updates current set of computed values
        /// of the latest time frame.
        /// </summary>
        /// <param name="price"></param>
        public abstract void ComputeCurrent(Price price);

        /// <summary>
        /// Compute the indicator values, when the input price is a new price in
        /// the next time frame. The indicator will generate a new set of computed values
        /// corresponding to this new time frame.
        /// </summary>
        /// <param name="price"></param>
        public abstract void ComputeNext(Price price);
    }
}