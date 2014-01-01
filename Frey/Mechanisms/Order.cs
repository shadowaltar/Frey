using System;
using System.Security.AccessControl;
using Automata.Core;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms.Utils;

namespace Automata.Mechanisms
{
    public class Order
    {
        public Order(Security security, Side side, double quantity, DateTime orderTime)
            : this(security, side, OrderType.Market, double.NaN, quantity, 0, orderTime)
        {
        }

        public Order(Security security, Side side, double price, double quantity, double stopLossPrice, DateTime orderTime)
            : this(security, side, OrderType.Market, price, quantity, stopLossPrice, orderTime)
        {
        }

        public Order(Security security, Side side, OrderType type, double price, double quantity, double stopLossPrice, DateTime orderTime)
        {
            Security = security;
            Side = side;
            Type = type;
            Price = price;
            Quantity = quantity;
            StopLossPrice = stopLossPrice;
            Time = orderTime;
        }

        public static Order CreateToClose(Position position, DateTime orderTime)
        {
            return new Order(position.Security, position.Side.Opposite(), position.ActualQuantity, orderTime)
            {
                IsClosingPosition = true
            };
        }

        public Security Security { get; private set; }
        public Side Side { get; set; }
        public bool IsClosingPosition { get; private set; }
        public OrderType Type { get; set; }
        public double Price { get; set; }
        public double Quantity { get; private set; }
        public DateTime Time { get; private set; }
        public double StopLossPrice { get; set; }

        public double Equity { get { return Price * Quantity; } }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}{3}: {4}x{5}; SL{6}", Time.PrintBracket(),
                Security.Code, Type, Side,
                double.IsNaN(Price) ? "Any" : Price.ToString(),
                Quantity, StopLossPrice);
        }
    }

    public class CashOrder : Order
    {
        public CashOrder(Cash cash, double quantity, DateTime orderTime, bool inflow = true) :
            base(cash, inflow ? Side.Long : Side.Short, OrderType.PortfolioCashFlow, double.NaN, quantity, double.NaN, orderTime)
        {
        }

        public static CashOrder NewContribute(Portfolio portfolio, double quantity, DateTime contributeTime)
        {
            portfolio.ThrowIfNull();
            portfolio.CashPosition.ThrowIfNull();
            portfolio.CashPosition.Security.ThrowIfNull();
            var cash = portfolio.CashPosition.Security.CastOrThrow<Cash>();

            return new CashOrder(cash, quantity, contributeTime);
        }

        public static CashOrder NewWithdrawal(Portfolio portfolio, double quantity, DateTime withdrawTime)
        {
            portfolio.ThrowIfNull();
            portfolio.CashPosition.ThrowIfNull();
            portfolio.CashPosition.Security.ThrowIfNull();
            var cash = portfolio.CashPosition.Security.CastOrThrow<Cash>();

            return new CashOrder(cash, quantity, withdrawTime, false);
        }
    }
}