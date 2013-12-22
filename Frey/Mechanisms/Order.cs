using Automata.Entities;

namespace Automata.Mechanisms
{
    public class Order
    {
        public Order(Security security, Side side, double price, double quantity)
            : this(security, side, OrderType.Market, price, quantity, 0)
        {
        }

        public Order(Security security, Side side, double price, double quantity, double stopLossPrice)
            : this(security, side, OrderType.Market, price, quantity, stopLossPrice)
        {
        }

        public Order(Security security, Side side, OrderType type, double price, double quantity, double stopLossPrice)
        {
            Security = security;
            Side = side;
            Type = type;
            Price = price;
            Quantity = quantity;
            StopLossPrice = stopLossPrice;
        }

        public Security Security { get; set; }
        public Side Side { get; set; }
        public OrderType Type { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }

        public double StopLossPrice { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}{2}: {3}x{4}; SL{5}", Security, Type, Side, Price, Quantity, StopLossPrice);
        }
    }
}