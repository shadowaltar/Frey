using System;

namespace Trading.Common.Entities
{
    public class Position
    {
        public Security Security { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get { return Price * Quantity; } }
        public DateTime Time { get; set; }
    }
}