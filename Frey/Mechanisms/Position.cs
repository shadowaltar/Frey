using System.Threading;
using Automata.Core;
using Automata.Entities;
using System;
using Automata.Mechanisms.Utils;

namespace Automata.Mechanisms
{
    public class Position
    {
        public Position(Order order, double actualPrice, double actualQuantity, DateTime executionTime)
        {
            Order = order;
            Security = order.Security;
            Side = order.Side;
            ActualEntryPrice = actualPrice;
            ActualQuantity = actualQuantity;
            ExecutionTime = executionTime;
        }

        public Order Order { get; protected set; }
        public Security Security { get; protected set; }
        public Side Side { get; protected set; }

        public double ActualEntryPrice { get; protected set; }
        public double ActualQuantity { get; protected set; }
        public virtual double Equity { get { return ActualEntryPrice * ActualQuantity; } }

        public double TransactionCost { get; set; }
        public DateTime ExecutionTime { get; set; }

        public bool IsCashContribution { get; protected set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}: {3}x{4}",
                ExecutionTime.PrintBracket(), Security.Code, Side, ActualEntryPrice, ActualQuantity);
        }

        public static CashPosition NewCash(double quantity, Currency currency, DateTime contributeTime)
        {
            var cp = new CashPosition(quantity, currency, contributeTime)
            {
                Side = Side.Long,
                IsCashContribution = true
            };
            return cp;
        }
    }

    public class CashPosition : Position
    {
        private readonly ReaderWriterLockSlim quantityLock = new ReaderWriterLockSlim();

        public CashPosition(double quantity, Currency currency, DateTime contributeTime)
            : base(new CashOrder(new Cash { Code = currency.Code() }, quantity, contributeTime),
            1, quantity, contributeTime)
        {
            IsCashContribution = true;
        }

        public void Add(double quantity)
        {
            try
            {
                quantityLock.EnterWriteLock();
                ActualQuantity += quantity;
            }
            finally
            {
                quantityLock.ExitWriteLock();
            }
        }

        public override double Equity
        {
            get
            {
                try
                {
                    quantityLock.EnterReadLock();
                    return ActualQuantity;
                }
                finally
                {
                    quantityLock.ExitReadLock();
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}",
                Security.Code, Equity);
        }
    }
}